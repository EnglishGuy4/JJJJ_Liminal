using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

[System.Serializable]
public class Wave
{
    public float waveTime = 30f;        // Duration of this wave
    public float spawnRate = 2f;        // How often enemies spawn
    public int maxEnemies = 10;         // Maximum enemies for this wave

    [Header("Enemy Variations")]
    public List<GameObject> enemyPrefabs = new List<GameObject>();  // Different enemy types
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

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioSource musicSource;
    public AudioClip countdownSFX;
    public AudioClip waveStartSFX;
    public AudioClip waveEndSFX;
    public AudioClip allWavesCompleteSFX;
    public AudioClip waveMusic;

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
            if (waveMode == WaveMode.Timed)
            {
                currentWaveIndex++;
                PlaySFX(waveEndSFX);
                BeginIntermission();
            }
            else
            {
                // Endless mode: increase difficulty
                endlessCurrentWave.waveTime = endlessCurrentWave.waveTime; // keep user-defined time
                endlessCurrentWave.spawnRate = Mathf.Max(minSpawnRate, endlessCurrentWave.spawnRate - spawnRateDecrease);
                endlessCurrentWave.maxEnemies = Mathf.Min(maxEnemiesCap, endlessCurrentWave.maxEnemies + maxEnemiesIncrease);

                currentWaveIndex++; // 🔹 Increment wave counter for Endless mode
                PlaySFX(waveEndSFX);
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
            endlessCurrentWave.enemyPrefabs = new List<GameObject>(waves[0].enemyPrefabs); // 🔹 copy enemy list
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

    private void SpawnEnemy()
    {
        if (spawnPoints.Count == 0) return;

        Wave currentWave = (waveMode == WaveMode.Timed && currentWaveIndex < waves.Count)
            ? waves[currentWaveIndex]
            : endlessCurrentWave;

        if (currentWave.enemyPrefabs == null || currentWave.enemyPrefabs.Count == 0) return;

        // 🔹 Pick a random enemy prefab from the wave’s list
        GameObject chosenPrefab = currentWave.enemyPrefabs[UnityEngine.Random.Range(0, currentWave.enemyPrefabs.Count)];

        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        GameObject enemy = PoolManager.current.GetPooledObject(chosenPrefab.name);
        if (enemy == null) return;

        enemy.transform.position = spawnPoints[spawnIndex].position;
        enemy.transform.rotation = spawnPoints[spawnIndex].rotation;
        enemy.GetComponent<EnemyHealth>().health = 1;
        enemy.GetComponent<EnemyMovement>().isDead = false;
        enemy.SetActive(true);
        enemy.GetComponent<EnemyShoot>().enabled = true;
        enemy.GetComponent<NavMeshAgent>().enabled = true;

        EnemyMovement enemyMovement = enemy.GetComponent<EnemyMovement>();
        enemyMovement.waypoints = waypoints;

        EnemyHealth enemyHealth = enemy.GetComponent<EnemyHealth>();
        enemyHealth.enemySpawnerScript = this;

        enemiesFromThisSpawnerList.Add(enemy);
        gameManager.enemies.Add(enemy);
    }


    private void BeginIntermission(bool startingWave = false)
    {
        if (waveMode == WaveMode.Timed && currentWaveIndex >= waves.Count)
        {
            EndAllWaves();
            return;
        }

        inIntermission = true;
        intermissionTimer = timeBetweenWaves;

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

        PlaySFX(countdownSFX);
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

    private void EndAllWaves()
    {
        allWavesComplete = true;
        inIntermission = false;
        waveText.text = "ALL WAVES COMPLETE!";
        if (waveTimerText != null)
            waveTimerText.text = "";

        PlaySFX(allWavesCompleteSFX);

        if (musicSource != null && musicSource.isPlaying)
        {
            musicSource.Stop();
        }

        OnAllWavesComplete?.Invoke();
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
}
