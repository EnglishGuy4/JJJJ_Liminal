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

    [Header("Enemy Settings")]
    public bool bossShip;
    public bool enemyShip;
    public bool explodeIntoPieces;

    [Header("Death Effects")]
    public GameObject explodesInto;
    public GameObject spherePart;
    public Material hitMaterial;

    [HideInInspector]
    public GameObject cannonBall;

    [Header("Scoring")]
    public int scoreValue = 100; // ✅ How many points this enemy gives when it dies

    void Awake()
    {
        // Find GameManager in scene
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

        // Debug feedback
        if (health == 1)
        {
            Debug.Log("Wow! You got the hit!");
        }
    }

    public void Death()
    {
        // ✅ Award score before deactivating
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

        enemyShoot.enabled = false;

        // Remove from spawner list if needed
        if (enemyShip && enemySpawnerScript != null)
            enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

        // Should we explode and fly out into a million (or two) pieces?
        if (explodeIntoPieces)
        {
            for (int i = 0; i < 2; i++)
            {
                GameObject shrapnel = Instantiate(
                    explodesInto,
                    transform.position,
                    Quaternion.identity
                );

                Rigidbody rb = shrapnel.GetComponent<Rigidbody>();
                if (rb != null)
                {
                    Vector3 dir = (i == 0 ? Vector3.left : Vector3.right) + Vector3.up;
                    rb.AddForce(dir.normalized * 5f, ForceMode.Impulse);
                }
            }
        }

        // Deactivate instead of destroying
        gameObject.SetActive(false);
    }
}
