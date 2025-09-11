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
}

public class SpawnerManager : MonoBehaviour
{
    public event System.Action BeginSpawningEvent;

    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> waypoints = new List<Transform>();

    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>();
    public float timeBetweenWaves = 5f;

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;

    [Header("Audio")]
    public AudioSource audioSource; // For SFX
    public AudioSource musicSource; // 🔹 New dedicated source for wave music
    public AudioClip countdownSFX;
    public AudioClip waveStartSFX;
    public AudioClip waveEndSFX;
    public AudioClip allWavesCompleteSFX;
    public AudioClip waveMusic;     // 🔹 Background track for waves

    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private float spawnTimer = 0f;
    private float intermissionTimer = 0f;

    private bool inIntermission = false;
    private bool allWavesComplete = false;
    private bool wavesStarted = false;   // 🔹 New flag

    [HideInInspector]
    public List<GameObject> enemiesFromThisSpawnerList = new List<GameObject>();

    private GameManager gameManager;
    public event System.Action OnAllWavesComplete;

    private void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        SetUpChildObjects();
    }

    private void Start()
    {
        // 🔹 Show "Get Ready" message until cannon is grabbed
        if (waveText != null)
            waveText.text = "Man the turret, they're coming...";
        if (waveTimerText != null)
            waveTimerText.text = "";
    }

    private void Update()
    {
        if (!wavesStarted || allWavesComplete) return; // 🔹 Wait until cannon grabbed

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

        if (currentWaveIndex >= waves.Count) return;

        Wave currentWave = waves[currentWaveIndex];
        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        UpdateWaveTimerText(currentWave.waveTime - waveTimer);

        if (waveTimer >= currentWave.waveTime)
        {
            currentWaveIndex++;
            PlaySFX(waveEndSFX);
            BeginIntermission();
            return;
        }

        if (spawnTimer >= currentWave.spawnRate && enemiesFromThisSpawnerList.Count < currentWave.maxEnemies)
        {
            SpawnEnemy();
            spawnTimer = 0f;
        }
    }

    // 🔹 Public method for Cannon to trigger waves
    public void BeginSpawning()
    {
        BeginSpawningEvent?.Invoke();

        if (wavesStarted) return; // Prevent duplicate start
        wavesStarted = true;
        BeginIntermission(startingWave: true);

        // 🔹 Start the background music when waves begin
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

        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        GameObject enemy = PoolManager.current.GetPooledObject(enemyPrefab.name);
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
        // If no more waves are left, stop and show completion
        if (currentWaveIndex >= waves.Count)
        {
            EndAllWaves();
            return;
        }

        inIntermission = true;
        intermissionTimer = timeBetweenWaves;

        if (startingWave)
        {
            waveText.text = "Wave 1 starting soon...";
        }
        else
        {
            waveText.text = "Next Wave Incoming...";
        }

        // Play countdown SFX
        PlaySFX(countdownSFX);
    }

    private void StartNextWave()
    {
        inIntermission = false;
        waveTimer = 0f;
        spawnTimer = 0f;

        if (currentWaveIndex < waves.Count)
        {
            UpdateWaveText();
            PlaySFX(waveStartSFX);
        }
        else
        {
            EndAllWaves();
        }
    }

    private void UpdateWaveText()
    {
        if (waveText != null && currentWaveIndex < waves.Count)
        {
            waveText.text = "Wave: " + (currentWaveIndex + 1);
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
        waveText.text = "All Waves Complete!";
        if (waveTimerText != null)
            waveTimerText.text = "";

        PlaySFX(allWavesCompleteSFX);

        // 🔹 Stop the background music when waves finish
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
