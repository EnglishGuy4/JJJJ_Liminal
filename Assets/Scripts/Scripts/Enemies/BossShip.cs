using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BossShip : MonoBehaviour
{
    public GameObject[] waypoints;
    private NavMeshAgent agent;
    private NavMeshObstacle obstacle;
    private EnemyShoot enemyShoot;
    private bool lastWaypoint = false;

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        obstacle = GetComponent<NavMeshObstacle>();
        agent.SetDestination(waypoints[0].transform.position);
        enemyShoot = GetComponent<EnemyShoot>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position == waypoints[0].transform.position)
        {
            agent.SetDestination(waypoints[1].transform.position);
        }

        if (transform.position == waypoints[1].transform.position)
        {
            agent.SetDestination(waypoints[2].transform.position);
        }

        if(transform.position == waypoints[2].transform.position)
        {
            agent.SetDestination(waypoints[3].transform.position);
        }

        if(transform.position == waypoints[3].transform.position && !lastWaypoint)
        {
            agent.enabled = false;
            obstacle.enabled = true;
            enemyShoot.bossShip = true;
            enemyShoot.reverseShot = false;
            enemyShoot.currentNumber = 0;
            enemyShoot.timeBetweenShots = 0.2f;
            lastWaypoint = true;
        }
    }
}
