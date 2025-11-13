using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;
using Liminal.Core.Fader;

[System.Serializable]
public class Wave
{
    public float waveTime = 30f;        // Duration of this wave
    public float spawnRate = 2f;        // How often enemies spawn
    public int maxEnemies = 10;         // Maximum enemies for this wave

    [Header("Enemy Variations")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();  // Different enemy types

    [Header("Timing")]
    [Tooltip("Extra delay (in seconds) to add after this wave before the next wave begins.")]
    public float extraDelayAfterWave = 0f;

    [Header("Dialogue")]
    [Tooltip("Optional dialogue GameObject to activate when this wave ends.")]
    public GameObject waveDialogueObject;

}



public enum WaveMode
{
    Timed,
    Endless
}

public class SpawnerManager : MonoBehaviour
{
    public event System.Action BeginSpawningEvent;

    [Header("Spawner Settings")]
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> waypoints = new List<Transform>();

    [Header("Wave Settings")]
    public WaveMode waveMode = WaveMode.Timed;  // 🔹 Choose Timed or Endless
    public List<Wave> waves = new List<Wave>();
    public float timeBetweenWaves = 5f;

    [Header("Endless Settings")]
    public float spawnRateDecrease = 0.1f;   // 🔹 How much faster enemies spawn each wave
    public int maxEnemiesIncrease = 2;       // 🔹 How many more enemies per wave
    public float minSpawnRate = 0.5f;        // 🔹 Clamp so it doesn’t get too fast
    public int maxEnemiesCap = 200;          // 🔹 Clamp max enemies

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;

    [Header("ScatterShot Settings")]
    [Tooltip("Wave number (1-based) that grants the cannon its scatter shot powerup when that wave starts. Set 0 to disable.")]
    public int scatterShotStartWave = 2;
    public int scatterShotEndWave = 3;
    [Tooltip("Assign the cannon to grant the powerup to (drag the Cannon GameObject here).")]
    public Cannon cannon;

    [Header("FullAuto Settings")]
    [Tooltip("Wave number (1-based) that grants the cannon its full auto powerup when that wave starts. Set 0 to disable.")]
    public int fullAutoStartWave = 4;
    public int fullAutoEndWave = 5;


    [Header("Audio")]
    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioClip countdownSFX;
    public AudioClip waveStartSFX;
    public AudioClip waveEndSFX;
    public AudioClip allWavesCompleteSFX;
    public AudioClip waveMusic;

    [Header("PowerUp Feedback")]
    public GameObject powerUpVFX;           // The VFX GameObject to show on powerup activation
    public AudioClip powerUpSFX;            // The sound played when a powerup activates
    public float powerUpVFXDuration = 2f;   // How long the VFX stays active

    [Header("PowerUp Animation Settings")]
    public Vector3 startScale = Vector3.one;        // Base/original scale
    public Vector3 popScale = new Vector3(1.3f, 1.3f, 1.3f); // Target "pop" scale
    public float scaleTransitionTime = 0.15f;       // Time for each scaling phase
    public float holdTimeAtPeak = 0.1f;             // Optional hold at peak scale

    [Header("End Sequence Settings")]
    [Tooltip("1-based index of the final wave. When this wave completes, the end dialogue triggers.")]
    public int finalWaveNumber = 5;

    [Tooltip("Dialogue GameObject to show at the end of the final wave.")]
    public GameObject endDialogue;

    [Tooltip("How long to show the end dialogue before ending the experience.")]
    public float endDialogueDuration = 5f;

    [Tooltip("Delay (in seconds) before starting the End Dialogue sequence after the final wave completes.")]
    public float endDialogueStartDelay = 2f;



    [Header("Debug")]
    [Tooltip("Enable to print verbose spawn debug information")]
    public bool debugSpawning = false;

    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private float spawnTimer = 0f;
    private float intermissionTimer = 0f;

    private bool inIntermission = false;
    private bool allWavesComplete = false;
    private bool wavesStarted = false;

    [HideInInspector]
    public List<GameObject> enemiesFromThisSpawnerList = new List<GameObject>();

    private GameManager gameManager;
    public event System.Action OnAllWavesComplete;

    private Wave endlessCurrentWave = new Wave(); // 🔹 Track the "current" endless wave

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetUpChildObjects();
    }

    private void Start()
    {
        if (waveText != null)
            waveText.text = "GET READY, THEY'RE COMING...";
        if (waveTimerText != null)
            waveTimerText.text = "";
    }

    private void Update()
    {
        if (!wavesStarted || allWavesComplete) return;

        if (inIntermission)
        {
            intermissionTimer -= Time.deltaTime;
            UpdateWaveTimerText(intermissionTimer);

            if (intermissionTimer <= 0f)
            {
                StartNextWave();
            }
            return;
        }

        Wave currentWave;
        if (waveMode == WaveMode.Timed)
        {
            if (currentWaveIndex >= waves.Count) return;
            currentWave = waves[currentWaveIndex];
        }
        else
        {
            currentWave = endlessCurrentWave;
        }

        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        UpdateWaveTimerText(currentWave.waveTime - waveTimer);

        if (waveTimer >= currentWave.waveTime)
        {
            PlaySFX(waveEndSFX);

            int nextWaveNumber = currentWaveIndex + 1; // 1-based number of the *wave that just ended*

            // 🔹 Activate dialogue for the completed wave (if assigned)
            Wave completedWave = waves[currentWaveIndex];
            if (completedWave.waveDialogueObject != null)
            {
                // Optional: deactivate any previously active dialogue
                foreach (var w in waves)
                {
                    if (w.waveDialogueObject != null)
                        w.waveDialogueObject.SetActive(false);
                }

                // Activate this wave's dialogue
                completedWave.waveDialogueObject.SetActive(true);
            }


            // 🔹 If this was the final wave, handle end sequence
            if (nextWaveNumber == finalWaveNumber)
            {
                EndAllWaves();
                return;
            }

            // 🔹 Otherwise, continue as normal
            if (waveMode == WaveMode.Timed)
            {
                currentWaveIndex++;
                BeginIntermission();
            }
            else
            {
                // Endless mode: increase difficulty
                endlessCurrentWave.spawnRate = Mathf.Max(minSpawnRate, endlessCurrentWave.spawnRate - spawnRateDecrease);
                endlessCurrentWave.maxEnemies = Mathf.Min(maxEnemiesCap, endlessCurrentWave.maxEnemies + maxEnemiesIncrease);

                currentWaveIndex++;
                BeginIntermission();
            }
            return;
        }


        if (spawnTimer >= currentWave.spawnRate && enemiesFromThisSpawnerList.Count < currentWave.maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    public void BeginSpawning()
    {
        BeginSpawningEvent?.Invoke();

        if (wavesStarted) return;
        wavesStarted = true;
        BeginIntermission(startingWave: true);

        if (waveMode == WaveMode.Endless)
        {
            endlessCurrentWave.waveTime = (waves.Count > 0 ? waves[0].waveTime : 30f);
            endlessCurrentWave.spawnRate = (waves.Count > 0 ? waves[0].spawnRate : 2f);
            endlessCurrentWave.maxEnemies = (waves.Count > 0 ? waves[0].maxEnemies : 10);
            // defensive copy so inspector changes later won't mutate the runtime list unexpectedly
            endlessCurrentWave.enemyPrefabs = new List<GameObject>(waves.Count > 0 ? waves[0].enemyPrefabs : new List<GameObject>());
        }


        if (musicSource != null && waveMusic != null)
        {
            musicSource.clip = waveMusic;
            musicSource.loop = true;
            musicSource.Play();
        }
    }



    private void SetUpChildObjects()
    {
        if (spawnPoints.Count == 0 || waypoints.Count == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("SpawnPoint"))
                    spawnPoints.Add(child);
                else if (child.CompareTag("Waypoint"))
                    waypoints.Add(child);
            }
        }
    }

    /// <summary>
    /// Robust SpawnEnemy: picks one random prefab, one random spawn point, tries pool lookup by name,
    /// falls back to Instantiate if pooling returns null. Defensive checks for required components.
    /// </summary>
    private void SpawnEnemy()
    {
        if (spawnPoints.Count == 0)
        {
            if (debugSpawning) Debug.LogWarning("[SpawnerManager] No spawn points assigned.");
            return;
        }

        Wave currentWave = (waveMode == WaveMode.Timed && currentWaveIndex < waves.Count)
            ? waves[currentWaveIndex]
            : endlessCurrentWave;

        if (currentWave == null)
        {
            if (debugSpawning) Debug.LogWarning("[SpawnerManager] currentWave is null.");
            return;
        }

        if (currentWave.enemyPrefabs == null || currentWave.enemyPrefabs.Count == 0)
        {
            if (debugSpawning) Debug.LogWarning("[SpawnerManager] No enemy prefabs assigned for current wave.");
            return;
        }

        // pick a random enemy prefab (single pick)
        int prefabIndex = UnityEngine.Random.Range(0, currentWave.enemyPrefabs.Count);
        GameObject chosenPrefab = currentWave.enemyPrefabs[prefabIndex];
        if (chosenPrefab == null)
        {
            if (debugSpawning) Debug.LogWarning("[SpawnerManager] chosenPrefab is null at index " + prefabIndex);
            return;
        }

        // pick one random spawn point (single pick)
        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject enemy = null;

        // Try to get a pooled object. Using name lookup can be fragile because of "(Clone)" suffix.
        // Try a sanitized name first, then raw name. If your PoolManager supports GetPooledObject(GameObject) prefer that.
        if (PoolManager.current != null)
        {
            string sanitized = chosenPrefab.name.Replace("(Clone)", "").Trim();
            enemy = PoolManager.current.GetPooledObject(sanitized);
            if (enemy == null)
            {
                // try with the raw name if sanitized failed
                enemy = PoolManager.current.GetPooledObject(chosenPrefab.name);
            }
        }

        // If pooling failed or no PoolManager, instantiate a fresh copy as a fallback
        if (enemy == null)
        {
            if (debugSpawning) Debug.Log("[SpawnerManager] Pool lookup failed for '" + chosenPrefab.name + "'. Instantiating fallback.");
            enemy = Instantiate(chosenPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            // place pooled object at the spawn point
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
            enemy.SetActive(true);
        }

        // Defensive: ensure required components exist before using them
        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        if (enemyHealth != null)
        {
            enemyHealth.enemySpawnerScript = this;
        }
        else
        {
            if (debugSpawning) Debug.LogWarning("[SpawnerManager] Spawned enemy missing EnemyHealth component: " + enemy.name);
        }

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            enemyMovement.isDead = false;
            enemyMovement.waypoints = waypoints;
        }
        else
        {
            if (debugSpawning) Debug.LogWarning("[SpawnerManager] Spawned enemy missing EnemyMovement component: " + enemy.name);
        }

        EnemyShoot enemyShoot = enemy.GetComponent<EnemyShoot>();
        if (enemyShoot != null) enemyShoot.enabled = true;

        NavMeshAgent nav = enemy.GetComponent<NavMeshAgent>();
        if (nav != null) nav.enabled = true;

        // Track spawned enemy
        enemiesFromThisSpawnerList.Add(enemy);
        if (gameManager != null)
            gameManager.enemies.Add(enemy);

        if (debugSpawning) Debug.LogFormat("[SpawnerManager] Spawned '{0}' at spawnIndex {1}. PoolUsed={2}", chosenPrefab.name, spawnIndex, (PoolManager.current != null).ToString());
    }


    private void BeginIntermission(bool startingWave = false)
    {
        if (waveMode == WaveMode.Timed && currentWaveIndex >= waves.Count)
        {
            EndAllWaves();
            return;
        }

        float extraDelay = 0f;

        // If we're in Timed mode and there’s a valid wave, add that wave’s custom delay
        if (waveMode == WaveMode.Timed && currentWaveIndex < waves.Count)
        {
            extraDelay = waves[currentWaveIndex].extraDelayAfterWave;
        }

        // Combine global and per-wave delay
        intermissionTimer = timeBetweenWaves + extraDelay;
        inIntermission = true;

        // 🟢 OPTIONAL: Display total wait time in wave text
        if (waveText != null)
            waveText.text = $"NEXT WAVE INCOMING... ({intermissionTimer:F1}s)";

        if (startingWave)
        {
            waveText.text = "WAVE 1 STARTING SOON...";
        }
        else
        {
            if (waveMode == WaveMode.Timed)
                waveText.text = "NEXT WAVE INCOMING...";
            else
                waveText.text = "ANOTHER WAVE INCOMING...";
        }
        // 🕒 Start delayed countdown sound

        StartCoroutine(PlayCountdownSFX(intermissionTimer));

    }

    private IEnumerator PlayCountdownSFX(float totalDelay)
    {
        // Wait until 6 seconds remain before the next wave
        float waitTime = Mathf.Max(0f, totalDelay - 6f);
        yield return new WaitForSeconds(waitTime);

        // Only play if we’re still in intermission (not interrupted early)
        if (inIntermission)
        {
            PlaySFX(countdownSFX);
        }
    }


    private void StartNextWave()
    {
        inIntermission = false;
        waveTimer = 0f;
        spawnTimer = 0f;

        if (waveMode == WaveMode.Timed && currentWaveIndex >= waves.Count)
        {
            EndAllWaves();
        }
        else
        {
            UpdateWaveText();
            PlaySFX(waveStartSFX);

            int currentWaveNumber = currentWaveIndex + 1;

            // Turns on scatter shot powerup if within the specified wave range or off
            if (scatterShotStartWave > 0 && cannon != null)
            {
                // currentWaveIndex is zero-based; scatterShotPowerupWaveNumber is 1-based
                if (((currentWaveNumber) >= scatterShotStartWave) && ((currentWaveNumber) < scatterShotEndWave))
                {
                    cannon.ActivateScatterShot();
                    TriggerPowerUpFeedback();
                }
                else if ((currentWaveNumber) >= (scatterShotEndWave))
                {
                    cannon.DeactivateScatterShot();
                }

            }

            if (fullAutoStartWave > 0 && cannon != null)
            {
                // currentWaveIndex is zero-based; fullAutoPowerupWaveNumber is 1-based
                if (((currentWaveNumber) >= fullAutoStartWave) && ((currentWaveNumber) < fullAutoEndWave))
                {
                    cannon.ActivateFullAutoShot();
                    TriggerPowerUpFeedback();
                }
                else if ((currentWaveNumber) >= (fullAutoEndWave))
                {
                    cannon.DeactivateFullAutoShot();
                }

            }
        }
    }

    private void UpdateWaveText()
    {
        if (waveText != null)
        {
            waveText.text = "WAVE: " + (currentWaveIndex + 1); // 🔹 Same UI for both modes
        }
    }


    private void UpdateWaveTimerText(float timeRemaining)
    {
        if (waveTimerText != null)
        {
            timeRemaining = Mathf.Max(0f, timeRemaining);
            int minutes = Mathf.FloorToInt(timeRemaining / 60f);
            int seconds = Mathf.FloorToInt(timeRemaining % 60f);
            waveTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        }
    }

    private void TriggerPowerUpFeedback()
    {
        if (powerUpVFX != null)
        {
            powerUpVFX.SetActive(true);
            powerUpVFX.transform.localScale = startScale;

            StopAllCoroutines(); // prevent overlapping animations
            StartCoroutine(AnimatePowerUpVFX());
        }

        PlaySFX(powerUpSFX);
    }

    private IEnumerator AnimatePowerUpVFX()
    {
        float timer = 0f;

        // 🔹 Scale up (startScale → popScale)
        while (timer < scaleTransitionTime)
        {
            float t = timer / scaleTransitionTime;
            powerUpVFX.transform.localScale = Vector3.Lerp(startScale, popScale, t);
            timer += Time.deltaTime;
            yield return null;
        }
        powerUpVFX.transform.localScale = popScale;

        // 🔹 Hold at peak
        yield return new WaitForSeconds(holdTimeAtPeak);

        // 🔹 Scale down (popScale → startScale)
        timer = 0f;
        while (timer < scaleTransitionTime)
        {
            float t = timer / scaleTransitionTime;
            powerUpVFX.transform.localScale = Vector3.Lerp(popScale, startScale, t);
            timer += Time.deltaTime;
            yield return null;
        }
        powerUpVFX.transform.localScale = startScale;

        // 🔹 Keep visible for the remainder of duration
        yield return new WaitForSeconds(powerUpVFXDuration);
        powerUpVFX.SetActive(false);
    }



    private IEnumerator DeactivatePowerUpVFXAfterDelay()
    {
        yield return new WaitForSeconds(powerUpVFXDuration);
        if (powerUpVFX != null)
            powerUpVFX.SetActive(false);
    }


    private void EndAllWaves()
    {
        allWavesComplete = true;
        inIntermission = false;

        waveText.text = "ALL WAVES COMPLETE!";
        if (waveTimerText != null)
            waveTimerText.text = "";

        PlaySFX(allWavesCompleteSFX);

        if (musicSource != null && musicSource.isPlaying)
            musicSource.Stop();

        OnAllWavesComplete?.Invoke();

        // 🔹 Start final dialogue + end experience
        StartCoroutine(StartEndDialogueWithDelay());
    }



    public void RemoveEnemyFromList(GameObject enemy)
    {
        if (enemiesFromThisSpawnerList.Contains(enemy))
            enemiesFromThisSpawnerList.Remove(enemy);
    }

    private void PlaySFX(AudioClip clip)
    {
        if (audioSource != null && clip != null)
        {
            audioSource.PlayOneShot(clip);
        }
    }

    private IEnumerator StartEndDialogueWithDelay()
    {
        // Wait for the configured delay before starting the end dialogue sequence
        yield return new WaitForSeconds(endDialogueStartDelay);

        // Then continue with the existing end sequence
        StartCoroutine(HandleEndDialogueSequence());
    }


    private IEnumerator HandleEndDialogueSequence()
    {
        if (endDialogue != null)
            endDialogue.SetActive(true);

        // Wait for the dialogue to finish
        yield return new WaitForSeconds(endDialogueDuration);

        // Find GameManager and trigger fade to results
        GameManager gm = FindObjectOfType<GameManager>();
        if (gm != null)
        {
            gm.FadeAndLoadResults();
        }
        else
        {
            Debug.LogWarning("GameManager not found – could not trigger FadeAndLoadResults.");
        }
    }

}
