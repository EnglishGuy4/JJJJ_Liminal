using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSpawnManager : MonoBehaviour
{
    [Header("Drone Spawning")]
    public GameObject dronePrefab;
    public List<Transform> leftSpawnPoints;
    public List<Transform> rightSpawnPoints;

    [Header("Approach slots (place several in front of player)")]
    public List<Transform> approachLeftSlots;    public List<Transform> approachRightSlots;  // slots for drones coming from right
    public Transform fallbackApproachPoint;
    public float minSpawnInterval = 5f;
    public float maxSpawnInterval = 10f;
    [Tooltip("Wave number (1-based) when drones start spawning")]
    public int startSpawningWave = 3;

    // NEW: assign the Transform drones should shoot at (player shield, player, etc.)
    [Header("Shooting")]
    public Transform shootTarget;

    private Coroutine spawnRoutine;
    private bool running = false;
    public static DroneSpawnManager Instance { get; private set; }
    private HashSet<Transform> occupiedSlots = new HashSet<Transform>();

    void Awake()
    {
        Instance = this;
    }

    void OnDestroy()
    {
        if (Instance == this) Instance = null;
    }

    void Start()
    {
        if (dronePrefab == null) Debug.LogWarning("[DroneSpawnManager] dronePrefab not assigned.");
        if ((leftSpawnPoints == null || leftSpawnPoints.Count == 0) &&
            (rightSpawnPoints == null || rightSpawnPoints.Count == 0))
            Debug.LogWarning("[DroneSpawnManager] No side spawn points assigned.");
        if (minSpawnInterval <= 0f) minSpawnInterval = 1f;
        if (maxSpawnInterval < minSpawnInterval) maxSpawnInterval = minSpawnInterval + 1f;
    }

    // call from SpawnerManager
    public void OnWaveStarted(int waveNumber)
    {
        if (waveNumber >= startSpawningWave) StartSpawning();
        else StopSpawning();
    }
    public void OnWaveEnded() => StopSpawning();

    public void StartSpawning()
    {
        if (running) return;
        running = true;
        spawnRoutine = StartCoroutine(SpawnLoop());
    }
    public void StopSpawning()
    {
        if (!running) return;
        running = false;
        if (spawnRoutine != null) { StopCoroutine(spawnRoutine); spawnRoutine = null; }
    }

    private IEnumerator SpawnLoop()
    {
        while (running)
        {
            try { SpawnDrone(); }
            catch (System.Exception ex)
            {
                Debug.LogError("[DroneSpawnManager] Exception in SpawnDrone: " + ex);
                StopSpawning();
                yield break;
            }
            float wait = Random.Range(minSpawnInterval, maxSpawnInterval);
            yield return new WaitForSeconds(wait);
        }
    }

    private void SpawnDrone()
    {
        if (dronePrefab == null) { Debug.LogWarning("[DroneSpawnManager] dronePrefab not assigned."); return; }

        bool spawnLeft = (Random.value < 0.5f);
        List<Transform> spawnList = spawnLeft ? leftSpawnPoints : rightSpawnPoints;
        if (spawnList == null || spawnList.Count == 0)
        {
            spawnList = (leftSpawnPoints != null && leftSpawnPoints.Count > 0) ? leftSpawnPoints : rightSpawnPoints;
            if (spawnList == null || spawnList.Count == 0) { Debug.LogWarning("[DroneSpawnManager] No side spawn points assigned."); return; }
        }
        Transform spawnTransform = spawnList[Random.Range(0, spawnList.Count)];

        // obtain instance (pooled preferred)
        GameObject drone = null;
        if (PoolManager.current != null)
        {
            string tryName = dronePrefab.name.Replace("(Clone)", "").Trim();
            drone = PoolManager.current.GetPooledObject(tryName) ?? PoolManager.current.GetPooledObject(dronePrefab.name);
        }
        if (drone == null) drone = Instantiate(dronePrefab);

        Debug.Log($"[DroneSpawnManager] Got drone instance '{drone.name}' (active={drone.activeSelf})");

        // detach from pool parent so world position assignments are reliable
        if (drone.transform.parent != null)
            drone.transform.SetParent(null, true);

        // ensure inactive so OnEnable won't run/complete before configuration
        if (drone.activeSelf)
        {
            drone.SetActive(false);
            Debug.Log("[DroneSpawnManager] Deactivated pooled instance before configuration.");
        }

        // reserve slot and configure movement BEFORE final activation
        Transform reserved = ReserveApproachSlot(spawnLeft);

        // position + rotation BEFORE activation and BEFORE configuring movement so OnEnable sees correct state
        drone.transform.position = spawnTransform.position;
        drone.transform.rotation = spawnTransform.rotation;

        // configure movement components BEFORE activation:
        var enemyMovement = drone.GetComponent<EnemyMovement>();
        if (enemyMovement != null)
        {
            if (reserved != null) enemyMovement.waypoints = new List<Transform> { reserved };
            else if (fallbackApproachPoint != null) enemyMovement.waypoints = new List<Transform> { fallbackApproachPoint };
            enemyMovement.isDead = false;
            Debug.Log($"[DroneSpawnManager] Configured EnemyMovement for '{drone.name}' -> waypoint={(reserved!=null?reserved.name:"fallback")}");
        }
        else
        {
            var shipMove = drone.GetComponent<EnemyShipMovement>();
            if (shipMove != null)
            {
                if (reserved != null)
                {
                    shipMove.useSpawnBasedHold = false;
                    shipMove.SetHoldPoint(reserved);
                    Debug.Log($"[DroneSpawnManager] Assigned slot {reserved.name} to {drone.name}");
                }
                else if (fallbackApproachPoint != null)
                {
                    shipMove.useSpawnBasedHold = false;
                    shipMove.SetHoldPoint(fallbackApproachPoint);
                    Debug.Log($"[DroneSpawnManager] Assigned fallback point to {drone.name}");
                }
                else
                {
                    shipMove.ChooseClosestHoldPoint();
                }
            }
        }

        // NEW: assign shooting target to drone's DroneShot (if present)
        var droneShot = drone.GetComponentInChildren<DroneShot>();
        if (droneShot != null)
        {
            // prefer the reserved approach slot as the shooting target (allows drones to shoot at the waypoint)
            if (reserved != null)
            {
                droneShot.target = reserved;
                Debug.Log($"[DroneSpawnManager] Assigned reserved slot '{reserved.name}' as shoot target for {drone.name}");
            }
            else if (shootTarget != null)
            {
                droneShot.target = shootTarget;
                Debug.Log($"[DroneSpawnManager] Assigned global shoot target '{shootTarget.name}' to {drone.name}");
            }
            else
            {
                // no explicit target provided — leave null and log so it won't silently fail later
                Debug.LogWarning($"[DroneSpawnManager] No shoot target available for {drone.name}; it will not fire until a target is assigned.");
            }
        }

        // Finally activate
        drone.SetActive(true);
        Debug.Log($"[DroneSpawnManager] Activated drone '{drone.name}'. reservedSlot={(reserved!=null?reserved.name:"none")}");
    }

    // Reserve a free slot; prefer left or right slots according to spawn side
    private Transform ReserveApproachSlot(bool preferLeft)
    {
        List<Transform> preferred = preferLeft ? approachLeftSlots : approachRightSlots;
        List<Transform> other = preferLeft ? approachRightSlots : approachLeftSlots;

        // try preferred list first
        if (preferred != null)
        {
            foreach (var t in preferred)
            {
                if (t != null && !occupiedSlots.Contains(t))
                {
                    occupiedSlots.Add(t);
                    return t;
                }
            }
        }

        // try other list
        if (other != null)
        {
            foreach (var t in other)
            {
                if (t != null && !occupiedSlots.Contains(t))
                {
                    occupiedSlots.Add(t);
                    return t;
                }
            }
        }

        // no slot free
        return null;
    }

    // called by DroneSlotTracker when drone is done / disabled
    public void ReleaseApproachSlot(Transform slot)
    {
        if (slot == null) return;
        if (occupiedSlots.Contains(slot)) occupiedSlots.Remove(slot);
    }
}
