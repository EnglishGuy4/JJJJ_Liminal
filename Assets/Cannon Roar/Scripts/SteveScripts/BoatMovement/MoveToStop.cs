using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class MoveToStop : MonoBehaviour
{
    //Stores a reference to the waypoint system that the boat will use
    [SerializeField] private WaypointStop waypointScript;

    [SerializeField] private float disThreshold = 1f ;

    private Transform currentWaypoint;

    [HideInInspector]
    public NavMeshAgent agent;

    EnemyHealth enemyHealth;

    // Start is called before the first frame update
    void Awake()
    {
        agent = GetComponent<NavMeshAgent>();
        waypointScript = GetComponentInChildren<WaypointStop>();
        enemyHealth = GetComponentInParent<EnemyHealth>();
    }

    void Start()
    {
        //Set Initial position to first waypoint
        currentWaypoint = waypointScript.transform.GetChild(0);
    }

    private void FixedUpdate()
    {

        if(enemyHealth.health == 0)
        {
            return;
        }
        else
        {
            agent.SetDestination(currentWaypoint.position);

            if (Vector3.Distance(transform.position, currentWaypoint.position) < disThreshold)
            {
                if(waypointScript.lastWaypoint == true)
                {
                    agent.stoppingDistance = 2f;
                    return;
                }
                else
                {
                    currentWaypoint = waypointScript.NextWaypoint(currentWaypoint);
                }
                
            }
        }
       
    }
   
}
