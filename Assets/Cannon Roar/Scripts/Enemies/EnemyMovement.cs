using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]

public class EnemyMovement : MonoBehaviour
{
    [HideInInspector]
    public Transform enemyTarget; //Holds the target that the enemies will move towards

    //General Components
    private NavMeshAgent agent;
    private Collider enemyCollider;
    private EnemyHealth enemyHealth;
    private SpawnerManager spawnerManager;
    private int lastNumber;
    private int currentNumber;
    [HideInInspector]
    public bool isDead = false;

    //Waypoints
    public float enemyWaypointSpeed = 3f;
    public float enemyWaypointStoppingDistance = 2f;
    private int wayPointIndex = 0; //Stores the waypoint key that the enemy will move towards.
    [HideInInspector]
    public List<Transform> waypoints = new List<Transform>();

    private void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        enemyCollider = GetComponentInChildren<Collider>();
        enemyHealth = GetComponent<EnemyHealth>();
        spawnerManager = GameObject.Find("SpawnerManager").GetComponent<SpawnerManager>();
    }

    private void OnEnable()
    {
        agent.updatePosition = true; //Stops the enemy from moving
        agent.updateRotation = true; //Stops the enemy from rotating
        enemyCollider.enabled = true;
        agent.enabled = true;
    }

    public void FixedUpdate()
    {
        EnemyMovementAI();
    }

    public void OnTriggerEnter(Collider other)
    {
        if (agent.updateRotation == false)
        {
            if (enemyHealth.health > 0)
            {
                agent.updateRotation = true;
                Invoke("UpdatePosition", 0.8f);
            }
        }
    }

    public void UpdatePosition()
    {
        agent.updatePosition = true;
        NavMeshHit closestHit;

        if (NavMesh.SamplePosition(transform.position, out closestHit, 500, 1))
        {
            agent.Warp(closestHit.position);
        }
        else
        {
            enemyHealth.Death();
        }
    }

    void EnemyMovementAI()
    {
        Waypoints();

        if (agent.updatePosition == true && !agent.isOnNavMesh)
        {
            enemyHealth.Death();
        }
    }

    public void NavMeshAgentSettings(Vector3 enemyTarget, float enemySpeed, float enemyStoppingDistance)
    {
        if (agent.isOnNavMesh)
        {
            agent.SetDestination(enemyTarget); //Moves enemy to the Target
            agent.speed = enemySpeed; //Sets the chase speed
            agent.stoppingDistance = enemyStoppingDistance;
        }
    }

    public void Waypoints()
    {
        //Debug.Log(waypoints.Count);
        if (!isDead && agent.remainingDistance <= agent.stoppingDistance)
        {
                wayPointIndex = GetRandom(0, spawnerManager.waypoints.Count);
        }

        if (waypoints == null)
        {
            enemyHealth.Death();
        }
        else
        {
            if (currentNumber != wayPointIndex)
            {
                currentNumber = lastNumber;
                NavMeshAgentSettings(waypoints[wayPointIndex].position, enemyWaypointSpeed, enemyWaypointStoppingDistance);
            }
        }
    }

    int GetRandom(int min, int max)
    {
        int rand = Random.Range(min, max);
        while (rand == lastNumber)
            rand = Random.Range(min, max);
        lastNumber = rand;
        return rand;
    }
}
