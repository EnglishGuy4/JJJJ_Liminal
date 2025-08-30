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

    [Header("Scoring")]
    public int scoreValue = 100; // How many points this enemy gives when destroyed

    [Header("Shield Damage")]
    public float shieldDamage = 10f; // How much this enemy reduces the shield when it "gets through"

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
        // Enemy fell out of world (missed by player)
        if (transform.position.y <= -12f)
        {
            if (enemyShip && enemySpawnerScript != null)
                enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

            // Reduce shield since enemy got through
            if (gameManager != null)
            {
                Debug.Log("[EnemyHealth] Enemy fell out of world, reducing shield by " + shieldDamage);
                gameManager.ModifyShield(-shieldDamage);
            }

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
        // ✅ Add score to GameManager
        if (gameManager != null)
        {
            gameManager.AddScore(scoreValue);
        }

        gameManager.enemies.Remove(gameObject);

        if (enemyShip)
        {
            enemyMovement.isDead = true;
            agent.enabled = false;
        }

        if (enemyShoot != null)
            enemyShoot.enabled = false;

        // Remove from spawner list if needed
        if (enemyShip && enemySpawnerScript != null)
            enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

        // Shield penalty on deactivation (optional — if you want killed enemies to also reduce shield, uncomment this)
        /*
        if (gameManager != null)
        {
            Debug.Log("[EnemyHealth] Enemy died, reducing shield by " + shieldDamage);
            gameManager.ModifyShield(-shieldDamage);
        }
        */

        // Deactivate instead of destroying
        gameObject.SetActive(false);
    }
}
