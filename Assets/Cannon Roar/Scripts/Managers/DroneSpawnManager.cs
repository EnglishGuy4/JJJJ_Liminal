using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

/// <summary>
/// DroneSpawnManager
/// - Starts spawning drone prefabs when the main SpawnerManager reaches the configured wave.
/// - Keeps its own spawn points and drone waypoints separate from the main spawner.
/// - Integrates with PoolManager.current when available (name-based lookup, sanitized of "(Clone)").
/// - Uses reflection to read `SpawnerManager.currentWaveIndex` (private) so we don't have to change SpawnerManager.
/// </summary>
public class DroneSpawnManager : MonoBehaviour
{
    [Header("Wave Trigger")]
    [Tooltip("1-based wave number when drone spawning should start (e.g. set 3 to start on wave 3)")]
    public int startWave = 3;

    [Tooltip("If true, spawn only while the spawner is on the configured wave. If false, start at that wave and continue spawning.")]
    public bool runDuringWaveOnly = true;

    [Header("Spawning")]
    public float spawnRate = 5f;
    public int maxConcurrent = 3;
    public List<GameObject> dronePrefabs = new List<GameObject>();

    [Header("Spawn Points")]
    [Tooltip("Assign drone spawn Transforms. If empty, child transforms tagged 'DroneSpawnPoint' will be used.")]
    public List<Transform> spawnPoints = new List<Transform>();

    [Header("Movement Waypoints")]
    [Tooltip("Waypoints assigned to Drone Enemy movement; leave empty to use default EnemyMovement behavior.")]
    public List<Transform> droneWaypoints = new List<Transform>();

    [Header("Options")]
    public bool usePooling = true; // try PoolManager.current first
    public bool debug = false;

    // runtime
    private SpawnerManager spawnerManager;
    private FieldInfo currentWaveField; // reflection access to private currentWaveIndex
    private bool monitoring = false;
    private bool spawningActive = false;
    private Coroutine spawnLoopCoroutine;
    private List<GameObject> spawnedList = new List<GameObject>();

    private void Awake()
    {
        spawnerManager = FindObjectOfType<SpawnerManager>();
        if (spawnerManager != null)
        {
            // Try to get private field currentWaveIndex via reflection
            currentWaveField = typeof(SpawnerManager).GetField("currentWaveIndex", BindingFlags.NonPublic | BindingFlags.Instance);

            // Start monitoring immediately; SpawnerManager will trigger BeginSpawningEvent when it starts
            StartCoroutine(MonitorWaveRoutine());

            // Also subscribe so Monitor knows spawning started
            spawnerManager.BeginSpawningEvent += OnSpawnerBeginSpawning;
        }
        else
        {
            Debug.LogWarning("[DroneSpawnManager] SpawnerManager not found in scene.");
        }

        SetUpChildSpawnPoints();
    }

    private void OnDestroy()
    {
        if (spawnerManager != null)
            spawnerManager.BeginSpawningEvent -= OnSpawnerBeginSpawning;
    }

    private void OnSpawnerBeginSpawning()
    {
        // ensure monitor is running
        if (!monitoring)
            StartCoroutine(MonitorWaveRoutine());
    }

    private void SetUpChildSpawnPoints()
    {
        if (spawnPoints.Count == 0)
        {
            foreach (Transform child in transform)
            {
                if (child.CompareTag("DroneSpawnPoint") || child.name.ToLower().Contains("dronespawn"))
                {
                    spawnPoints.Add(child);
                }
            }

            // fallback: if still empty, add all children
            if (spawnPoints.Count == 0)
            {
                foreach (Transform child in transform)
                    spawnPoints.Add(child);
            }
        }
    }

    private IEnumerator MonitorWaveRoutine()
    {
        monitoring = true;

        while (true)
        {
            int current = GetCurrentWaveIndex(); // -1 if unknown

            if (current >= 0)
            {
                int currentWaveNumber = current + 1; // convert to 1-based for comparison

                if (runDuringWaveOnly)
                {
                    // Only spawn while the main spawner is on the configured wave
                    if (currentWaveNumber == startWave)
                    {
                        if (!spawningActive)
                        {
                            StartSpawning();
                        }
                    }
                    else
                    {
                        if (spawningActive)
                        {
                            StopSpawning();
                        }
                    }
                }
                else
                {
                    // Start spawning once the configured wave is reached and continue
                    if (currentWaveNumber >= startWave && !spawningActive)
                    {
                        StartSpawning();
                    }
                }
            }

            yield return new WaitForSeconds(0.5f);
        }
    }

    private int GetCurrentWaveIndex()
    {
        if (spawnerManager == null) return -1;

        if (currentWaveField != null)
        {
            object val = currentWaveField.GetValue(spawnerManager);
            if (val is int)
                return (int)val;
        }

        // fallback: try to parse wave number from public waveText (UI) if available
        try
        {
            var waveTextField = typeof(SpawnerManager).GetField("waveText", BindingFlags.Public | BindingFlags.Instance);
            if (waveTextField != null)
            {
                var textObj = waveTextField.GetValue(spawnerManager) as TMPro.TextMeshProUGUI;
                if (textObj != null)
                {
                    string txt = textObj.text ?? string.Empty;
                    // look for "WAVE: X"
                    int idx = txt.IndexOf("WAVE:");
                    if (idx >= 0)
                    {
                        string tail = txt.Substring(idx + 5).Trim();
                        int parsed;
                        if (int.TryParse(tail.Split(' ')[0], out parsed))
                            return parsed - 1; // convert to zero-based
                    }
                }
            }
        }
        catch { }

        return -1;
    }

    private void StartSpawning()
    {
        spawningActive = true;
        if (debug) Debug.Log("[DroneSpawnManager] Starting drone spawning.");
        if (spawnLoopCoroutine == null)
            spawnLoopCoroutine = StartCoroutine(SpawnLoop());
    }

    private void StopSpawning()
    {
        spawningActive = false;
        if (debug) Debug.Log("[DroneSpawnManager] Stopping drone spawning.");
        if (spawnLoopCoroutine != null)
        {
            StopCoroutine(spawnLoopCoroutine);
            spawnLoopCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (spawningActive)
        {
            // prune list
            spawnedList.RemoveAll(e => e == null || !e.activeInHierarchy);

            if (spawnedList.Count < maxConcurrent)
            {
                SpawnOneDrone();
            }

            yield return new WaitForSeconds(spawnRate);
        }
    }

    private void SpawnOneDrone()
    {
        if (dronePrefabs == null || dronePrefabs.Count == 0)
        {
            if (debug) Debug.LogWarning("[DroneSpawnManager] No dronePrefabs assigned.");
            return;
        }

        if (spawnPoints == null || spawnPoints.Count == 0)
        {
            if (debug) Debug.LogWarning("[DroneSpawnManager] No spawnPoints available.");
            return;
        }

        int prefabIndex = UnityEngine.Random.Range(0, dronePrefabs.Count);
        GameObject chosenPrefab = dronePrefabs[prefabIndex];
        if (chosenPrefab == null) return;

        int spawnIndex = UnityEngine.Random.Range(0, spawnPoints.Count);
        Transform spawnPoint = spawnPoints[spawnIndex];

        GameObject enemy = null;

        if (usePooling && PoolManager.current != null)
        {
            string sanitized = chosenPrefab.name.Replace("(Clone)", "").Trim();
            enemy = PoolManager.current.GetPooledObject(sanitized);
            if (enemy == null)
                enemy = PoolManager.current.GetPooledObject(chosenPrefab.name);
        }

        if (enemy == null)
        {
            enemy = Instantiate(chosenPrefab, spawnPoint.position, spawnPoint.rotation);
        }
        else
        {
            enemy.transform.position = spawnPoint.position;
            enemy.transform.rotation = spawnPoint.rotation;
            enemy.SetActive(true);
        }

        // try to set movement waypoints if EnemyMovement exists
        var movement = enemy.GetComponent<EnemyMovement>();
        if (movement != null)
        {
            movement.isDead = false;
            if (droneWaypoints != null && droneWaypoints.Count > 0)
            {
                movement.waypoints = droneWaypoints;
            }
            // don't override if droneWaypoints empty – assume prefab provides waypoints or default
        }

        // enable shooting/movement/nav where appropriate
        var shoot = enemy.GetComponent<EnemyShoot>(); if (shoot != null) shoot.enabled = true;
        var nav = enemy.GetComponent<UnityEngine.AI.NavMeshAgent>(); if (nav != null) nav.enabled = true;

        spawnedList.Add(enemy);

        if (debug) Debug.LogFormat("[DroneSpawnManager] Spawned drone '{0}' at spawnIndex {1}", chosenPrefab.name, spawnIndex);
    }
}
