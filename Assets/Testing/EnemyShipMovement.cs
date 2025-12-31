using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipMovement : MonoBehaviour
{
    [Header("Arrival / Hold")]
    public Transform holdPoint;         // optional: set by spawner or inspector
    public Vector3 holdPosition;        // optional: set directly if not using a Transform
    public bool useHoldPointTransform = true;
    public float moveSpeed = 8f;
    public float stopDistance = 0.5f;
    [SerializeField] private bool arrived = false; // visible in Inspector
    public bool Arrived => arrived; // read-only accessor for code

    [Header("Optional hooks")]
    public bool enableDroneShootingOnArrive = true;

    [Header("Bobbing Motion")]
    public float moveWobbleAmplitude = 0.6f;    // wobble while moving
    public float moveWobbleFrequency = 2.0f;
    public float idleWobbleAmplitude = 0.15f;   // smaller wobble when arrived
    public float idleWobbleFrequency = 1.2f;

    [Header("Spawn-based hold (fallback)")]
    public bool useSpawnBasedHold = true;
    public float holdDistanceFromSpawn = 90f;
    private Vector3 spawnPosition;

    // cached components
    private DroneShot droneShot;

    // wobble internal state
    private float wobblePhase;
    private float baseY;

    // if true, movement will wait until a holdPoint/holdPosition is explicitly assigned (used by spawners)
    private bool waitingForHoldAssignment = false;

    // optional array for spawner usage
    public Transform[] possibleHoldPoints;

    void Start()
    {
        arrived = false;
        droneShot = GetComponent<DroneShot>();
        if (droneShot != null)
            droneShot.canShoot = false;

        wobblePhase = Random.Range(0f, Mathf.PI * 2f);
    }

    void OnEnable()
    {
        spawnPosition = transform.position;
        baseY = transform.position.y;
        Debug.Log($"[EnemyShipMovement] OnEnable for {gameObject.name}: holdPoint={(holdPoint!=null?holdPoint.name:"null")}, useSpawnBasedHold={useSpawnBasedHold}");

        // Only apply spawn-based hold if no holdPoint is already assigned AND useSpawnBasedHold is true
        if (holdPoint == null && useSpawnBasedHold)
        {
            float dir = -Mathf.Sign(spawnPosition.x);
            if (Mathf.Approximately(dir, 0f)) dir = -1f;
            Vector3 offset = Vector3.right * dir * holdDistanceFromSpawn;
            SetHoldPosition(spawnPosition + offset);
            Debug.Log($"[EnemyShipMovement] Applying spawn-based hold at {holdPosition} for {gameObject.name}");
            waitingForHoldAssignment = false;
        }
        else
        {
            if (holdPoint != null)
            {
                Debug.Log($"[EnemyShipMovement] Existing holdPoint preserved for {gameObject.name}: {holdPoint.name}");
                waitingForHoldAssignment = false;
            }
            else
            {
                // No holdPoint and spawn-based fallback disabled -> wait for spawner to assign
                if (!useSpawnBasedHold)
                {
                    Debug.Log($"[EnemyShipMovement] Waiting for external hold assignment for {gameObject.name}");
                    waitingForHoldAssignment = true;
                }
                else
                {
                    waitingForHoldAssignment = false;
                }
            }
        }

        if (droneShot != null) droneShot.canShoot = false;
        arrived = false;
    }

    void Update()
    {
        // If we are waiting for a hold assignment, do nothing until SetHoldPoint/SetHoldPosition is called
        if (waitingForHoldAssignment)
            return;

        if (!arrived)
        {
            Vector3 targetPos = useHoldPointTransform && holdPoint != null ? holdPoint.position : holdPosition;
            // guard: if still no valid target, skip this frame
            if (useHoldPointTransform && holdPoint == null)
                return;
            if (!useHoldPointTransform && targetPos == Vector3.zero)
                return;

            // Move on X/Z toward target while keeping a baseY anchor for vertical wobble
            Vector3 flatCurrent = new Vector3(transform.position.x, 0f, transform.position.z);
            Vector3 flatTarget = new Vector3(targetPos.x, 0f, targetPos.z);
            Vector3 flatNext = Vector3.MoveTowards(flatCurrent, flatTarget, moveSpeed * Time.deltaTime);

            // Smoothly follow target Y for baseline
            baseY = Mathf.Lerp(baseY, targetPos.y, Time.deltaTime * 2f);

            // vertical wobble while moving
            float wobble = Mathf.Sin(Time.time * moveWobbleFrequency + wobblePhase) * moveWobbleAmplitude;
            transform.position = new Vector3(flatNext.x, baseY + wobble, flatNext.z);

            // face movement direction
            Vector3 lookDir = (targetPos - transform.position);
            if (lookDir.sqrMagnitude > 0.001f)
                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(lookDir), Time.deltaTime * 2f);

            // arrival check using horizontal distance
            if (Vector3.Distance(flatNext, flatTarget) <= stopDistance)
            {
                Arrive();
            }

            return;
        }

        // Arrived: small idle wobble
        float idleWob = Mathf.Sin(Time.time * idleWobbleFrequency + wobblePhase) * idleWobbleAmplitude;
        Vector3 p = transform.position;
        p.y = baseY + idleWob;
        transform.position = p;
    }

    private void Arrive()
    {
        arrived = true;
        if (enableDroneShootingOnArrive && droneShot != null)
            droneShot.canShoot = true;

        baseY = transform.position.y;

        // Release reserved approach slot so other drones can use it (optional behavior)
        var tracker = GetComponent<DroneSlotTracker>();
        if (tracker != null && tracker.assignedSlot != null && DroneSpawnManager.Instance != null)
        {
            DroneSpawnManager.Instance.ReleaseApproachSlot(tracker.assignedSlot);
            tracker.assignedSlot = null;
        }
    }

    // Spawner helpers
    public void SetHoldPoint(Transform t)
    {
        holdPoint = t;
        useHoldPointTransform = true;
        arrived = false;
        waitingForHoldAssignment = false;
        if (droneShot != null) droneShot.canShoot = false;
        Debug.Log($"[EnemyShipMovement] SetHoldPoint called on {gameObject.name} -> {(t!=null?t.name:"null")}");
    }

    public void SetHoldPosition(Vector3 pos)
    {
        holdPosition = pos;
        useHoldPointTransform = false;
        arrived = false;
        waitingForHoldAssignment = false;
        if (droneShot != null) droneShot.canShoot = false;
    }

    public void ChooseClosestHoldPoint()
    {
        if (possibleHoldPoints == null || possibleHoldPoints.Length == 0) return;

        float bestDist = float.MaxValue;
        Transform best = null;
        for (int i = 0; i < possibleHoldPoints.Length; i++)
        {
            if (possibleHoldPoints[i] == null) continue;
            float d = Vector3.SqrMagnitude(possibleHoldPoints[i].position - transform.position);
            if (d < bestDist)
            {
                bestDist = d;
                best = possibleHoldPoints[i];
            }
        }
        if (best != null) SetHoldPoint(best);
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Vector3 t = useHoldPointTransform && holdPoint != null ? holdPoint.position : holdPosition;
        Gizmos.DrawWireSphere(t, stopDistance);
        Gizmos.DrawLine(transform.position, t);
    }
}
