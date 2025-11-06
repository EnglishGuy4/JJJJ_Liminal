using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector] public NavMeshAgent agent;
    public int health = 1;
    private EnemyShoot enemyShoot;
    [HideInInspector] public SpawnerManager enemySpawnerScript;
    [HideInInspector] public BoxCollider boxCollider;
    private GameManager gameManager;
    private EnemyMovement enemyMovement;
    public bool bossShip;
    public bool enemyShip;
    [HideInInspector] public GameObject cannonBall;

    [Header("Scoring")]
    public int scoreValue = 100;

    [Header("Shield Damage")]
    public float shieldDamage = 10f;

    [Header("Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifetime = 3f;

    [Header("Portal Spawn Effects")]
    [SerializeField] private GameObject portalPrefab;
    [SerializeField] private float portalLifetime = 3f;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField, Range(0f, 1f)] private float explosionVolume = 1f;
    [SerializeField] private bool explosionSound2D = false;
    [SerializeField] private float explosionMinDistance = 1f;
    [SerializeField] private float explosionMaxDistance = 50f;

    [Header("Portal Audio")]
    [SerializeField] private AudioClip portalSound;
    [SerializeField, Range(0f, 1f)] private float portalVolume = 1f;
    [SerializeField] private bool portalSound2D = false;
    [SerializeField] private float portalMinDistance = 1f;
    [SerializeField] private float portalMaxDistance = 50f;

    [Header("Hit Materials")]
    [SerializeField] private Renderer enemyRenderer;  // Assign main Renderer
    [SerializeField] private Material normalMaterial;  // Default material
    [SerializeField] private Material hitMaterial;     // Material to show when hit
    [SerializeField] private float hitFlashDuration = 0.1f; // Duration of hit flash


    private Material enemyMaterial;
    private Coroutine hitFlashCoroutine;


    private bool hasSpawnedOnce = false; // ✅ prevents portal at pool init
    private int startHealth;


    void Awake()
    {
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        startHealth = health; // store initial value

        if (enemyRenderer != null && normalMaterial != null)
        {
            enemyRenderer.material = normalMaterial;
        }

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

    private void OnEnable()
    {
        health = startHealth; // Reset health

        if (enemyRenderer != null && normalMaterial != null)
        {
            enemyRenderer.material = normalMaterial;
        }

        if (!hasSpawnedOnce)
        {
            hasSpawnedOnce = true;
            return;
        }

        SpawnPortalEffect();
    }


    private void FlashHit()
    {
        if (enemyRenderer == null || hitMaterial == null || normalMaterial == null) return;

        // Stop previous flash if still running
        if (hitFlashCoroutine != null)
            StopCoroutine(hitFlashCoroutine);

        hitFlashCoroutine = StartCoroutine(HitFlashCoroutine());
    }

    private IEnumerator HitFlashCoroutine()
    {
        // Switch to hit material
        enemyRenderer.material = hitMaterial;

        // Wait for the flash duration
        yield return new WaitForSeconds(hitFlashDuration);

        // Revert to normal material
        enemyRenderer.material = normalMaterial;

        hitFlashCoroutine = null;
    }




    private void SpawnPortalEffect()
    {
        if (portalPrefab != null)
        {
            GameObject portal = Instantiate(portalPrefab, transform.position, Quaternion.identity);
            if (portalLifetime > 0f)
            {
                Destroy(portal, portalLifetime);
            }
        }

        if (portalSound != null)
        {
            GameObject audioGO = new GameObject("PortalAudio");
            audioGO.transform.position = transform.position;
            var src = audioGO.AddComponent<AudioSource>();

            src.spatialBlend = portalSound2D ? 0f : 1f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = portalMinDistance;
            src.maxDistance = Mathf.Max(portalMaxDistance, portalMinDistance + 0.01f);
            src.playOnAwake = false;

            src.PlayOneShot(portalSound, portalVolume);
            Destroy(audioGO, portalSound.length + 0.1f);
        }
    }

    void Update()
    {
        if (transform.position.y <= -12f)
        {
            if (enemyShip && enemySpawnerScript != null)
                enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

            if (gameManager != null)
            {
                Debug.Log("[EnemyHealth] Enemy fell out of world, reducing shield by " + shieldDamage);
                gameManager.ModifyShield(-shieldDamage);

                // ✅ Play explosion when shield is damaged
                PlayExplosionEffect();
            }

            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        Debug.Log($"{name} hit! Health before: {health}, Damage: {damage}");

        // Trigger material flash
        FlashHit();

        health -= damage;
        Debug.Log($"{name} health after: {health}");

        if (health <= 0)
        {
            Debug.Log($"{name} died!");
            Death();
        }
    }





    public void Death()
    {
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

        if (enemyShip && enemySpawnerScript != null)
            enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

        // ✅ Explosion on death
        PlayExplosionEffect();

        gameObject.SetActive(false);
    }

    /// <summary>
    /// Handles instantiating explosion VFX + SFX
    /// </summary>
    private void PlayExplosionEffect()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            if (explosionLifetime > 0f)
            {
                Destroy(explosion, explosionLifetime);
            }
        }

        if (explosionSound != null)
        {
            GameObject audioGO = new GameObject("ExplosionAudio");
            audioGO.transform.position = transform.position;
            var src = audioGO.AddComponent<AudioSource>();

            src.spatialBlend = explosionSound2D ? 0f : 1f;
            src.rolloffMode = AudioRolloffMode.Linear;
            src.minDistance = explosionMinDistance;
            src.maxDistance = Mathf.Max(explosionMaxDistance, explosionMinDistance + 0.01f);
            src.playOnAwake = false;

            src.PlayOneShot(explosionSound, explosionVolume);
            Destroy(audioGO, explosionSound.length + 0.1f);
        }
    }
}
