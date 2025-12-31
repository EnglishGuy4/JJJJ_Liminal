using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class BoatSpawner : MonoBehaviour
{
    public BoatSpawner current;
    [SerializeField]
    private GameObject boatPatrol;
    
    [SerializeField]
    private float spawnTime = 2f;

    public int enemyCount;
    //Holds the max number of enemies alloweed at anyone time
    public int maxNumberOfEnemiesAtOneTime = 1;
    public int maxNumberOfEnemiesOtherSpawnerLifeTime = 10;

    [HideInInspector] //hides variable below
    public List<GameObject> enemiesFromThisSpawnerList = new List<GameObject>();

    // Start is called before the first frame update
    void Start()
    {
        current = this;
        InvokeRepeating("checkIfObjectShouldBeSpawned", spawnTime, spawnTime);
    }

    void checkIfObjectShouldBeSpawned()
    {
        
        
           //if we have not reached the limit of enemies from this spawner
            if (enemiesFromThisSpawnerList.Count < maxNumberOfEnemiesAtOneTime && enemyCount < maxNumberOfEnemiesOtherSpawnerLifeTime)
            {
                EnemySpawn();
            }
            //if we have reacher the max number of enemies and all the enemies from this spawner are dead
            //else if (enemiesFromThisSpawnerList.Count == 0 && enemyCount >= maxNumberOfEnemiesOtherSpawnerLifeTime)
            //{
               // DestroySpawner();
           // }
        
    }

    void EnemySpawn()
    {
        GameObject returnedGameObject = PoolManager.current.GetPooledObject(boatPatrol.name);
        enemyCount++;
        returnedGameObject.SetActive(true);
        if (returnedGameObject.name == "MoveStopright(Clone)")
        {
            returnedGameObject.GetComponentInChildren<WaypointStop>().NextWaypoint(null);
        }
        else
        {
            returnedGameObject.GetComponentInChildren<PatrolWaypoint>().NextWaypoint(null);
        }
        returnedGameObject.transform.position = transform.position;
        returnedGameObject.transform.rotation = transform.rotation;
        //returnedGameObject.GetComponent<EnemyHealth>().enemySpawnerScript = current;
        returnedGameObject.GetComponent<EnemyHealth>().health = 1;
        returnedGameObject.GetComponent<EnemyHealth>().agent.enabled = true;
        returnedGameObject.GetComponent<EnemyHealth>().agent.ResetPath();
        returnedGameObject.GetComponentInChildren<BoxCollider>().enabled = true;
        returnedGameObject.GetComponentInChildren<EnemyShoot>().enabled = true;
        //Allows is to track the number of enemies currently alive
        enemiesFromThisSpawnerList.Add(returnedGameObject);
        
    }
}
