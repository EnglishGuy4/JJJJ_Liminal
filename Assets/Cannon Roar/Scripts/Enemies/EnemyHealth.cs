using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    public int health = 1;
    private EnemyShoot enemyShoot;
    [HideInInspector]
    public SpawnerManager enemySpawnerScript;
    [HideInInspector]
    public BoxCollider boxCollider;
    private GameManager gameManager;
    private EnemyMovement enemyMovement;
    public bool bossShip;
    public bool enemyShip;
    [HideInInspector]
    public GameObject cannonBall;

    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (enemyShip)
        {
            boxCollider = GetComponentInChildren<BoxCollider>();
            agent = GetComponentInChildren<NavMeshAgent>();
            enemyShoot = GetComponentInChildren<EnemyShoot>();
        }

        if (bossShip)
        {
            boxCollider = GetComponent<BoxCollider>();
            enemyShoot = GetComponent<EnemyShoot>();
        }
    }

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    void Update()
    {
        if (transform.position.y <= -12f)
        {
            if (enemyShip)
                enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        gameManager.enemies.Remove(gameObject);

        if (enemyShip)
        {
            enemyMovement.isDead = true;
            agent.enabled = false;
        }

        enemyShoot.enabled = false;

        // Remove from spawner list if needed
        if (enemyShip && enemySpawnerScript != null)
            enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

        // Deactivate instead of destroying
        gameObject.SetActive(false);
    }
}
