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

    [Header("Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifetime = 3f; // Auto-destroy explosion VFX

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField, Range(0f, 1f)] private float explosionVolume = 1f;
    [SerializeField] private bool explosionSound2D = false; // true = 2D (always audible), false = 3D spatial
    [SerializeField] private float explosionMinDistance = 1f;   // 3D only
    [SerializeField] private float explosionMaxDistance = 50f;  // 3D only

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

        // 🔥 Instantiate explosion effect (no scaling)
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (explosionLifetime > 0f)
            {
                Destroy(explosion, explosionLifetime);
            }
        }

        // 💥 Robust explosion audio (independent object, survives deactivation)
        if (explosionSound != null)
        {
            GameObject audioGO = new GameObject("ExplosionAudio");
            audioGO.transform.position = transform.position;
            var src = audioGO.AddComponent<AudioSource>();

            // Configure source
            src.spatialBlend = explosionSound2D ? 0f : 1f; // 0 = 2D, 1 = 3D
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = explosionMinDistance;
            src.maxDistance = Mathf.Max(explosionMaxDistance, explosionMinDistance + 0.01f);
            src.playOnAwake = false;

            // If you're using a spatializer plugin (e.g., Oculus), uncomment:
            // src.spatialize = !explosionSound2D;

            // Play and clean up
            src.PlayOneShot(explosionSound, explosionVolume);
            Destroy(audioGO, explosionSound.length + 0.1f);
        }

        // Deactivate instead of destroying
        gameObject.SetActive(false);
    }
}
