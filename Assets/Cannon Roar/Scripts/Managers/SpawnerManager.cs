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
    [Header("Spawner Settings")]
    public GameObject enemyPrefab;
    public List<Transform> spawnPoints = new List<Transform>();
    public List<Transform> waypoints = new List<Transform>();

    [Header("Wave Settings")]
    public List<Wave> waves = new List<Wave>();
    public float timeBetweenWaves = 5f; // Countdown before next wave starts (applies to first too)

    [Header("UI")]
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;  // Timer display

    [Header("Audio")]
    public AudioSource audioSource;
    public AudioClip countdownSFX;
    public AudioClip waveStartSFX;
    public AudioClip waveEndSFX;
    public AudioClip allWavesCompleteSFX;

    private int currentWaveIndex = 0;
    private float waveTimer = 0f;
    private float spawnTimer = 0f;
    private float intermissionTimer = 0f;

    private bool inIntermission = false;
    private bool allWavesComplete = false;

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
        // Start with an intermission countdown before wave 1
        BeginIntermission(startingWave: true);
    }

    private void Update()
    {
        if (allWavesComplete) return; // Stop everything once all waves are done

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

        if (currentWaveIndex >= waves.Count) return; // Safety check

        Wave currentWave = waves[currentWaveIndex];
        waveTimer += Time.deltaTime;
        spawnTimer += Time.deltaTime;

        UpdateWaveTimerText(currentWave.waveTime - waveTimer);

        if (waveTimer >= currentWave.waveTime)
        {
            // When a wave finishes, increment index and start intermission
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
