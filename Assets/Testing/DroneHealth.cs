using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using TMPro;

public class DroneHealth : MonoBehaviour
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
    public bool isDrone;
    [HideInInspector] public GameObject cannonBall;

    [Header("Scoring")]
    public int scoreValue = 100;
    private bool lastHitByPlayer = false; // NEW

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

    [Header("Score UI")]
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private float scorePopupDuration = 1.5f; // How long the UI stays
    [SerializeField] private Vector3 scorePopupScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 scorePopupPopScale = new Vector3(1.5f, 1.5f, 1.5f);

    private bool hasSpawnedOnce = false; // ✅ prevents portal at pool init

    void Awake()
    {
        //Debug.Log("DroneHealth Awake on: " + gameObject.name);

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
        //Debug.Log("Current health: " + health);
    }

    private void OnEnable()
    {
        // Skip very first activation (pool setup)
        if (!hasSpawnedOnce)
        {
            hasSpawnedOnce = true;
            return;
        }

        // reset hit source for each spawn
        lastHitByPlayer = false;

        // ✅ Only runs when spawned into play
        SpawnPortalEffect();
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
                //Debug.Log("[EnemyHealth] Enemy fell out of world, reducing shield by " + shieldDamage);
                gameManager.ModifyShield(-shieldDamage);

                // ✅ Play explosion when shield is damaged
                PlayExplosionEffect();
            }

            gameObject.SetActive(false);
        }
    }

    // Replace original TakeDamage with overload that accepts who caused it
    public void TakeDamage(int damage, bool fromPlayer)
    {
        lastHitByPlayer = fromPlayer;
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    // keep compatibility
    public void TakeDamage(int damage) => TakeDamage(damage, false);

    public void Death()
    {
        // Award score only if player caused the kill
        if (lastHitByPlayer && scoreValue > 0 && gameManager != null)
        {
            gameManager.AddScore(scoreValue);
            // show popup, etc. (use existing popup code if you have it)
            SpawnScorePopup();
        }

        if (gameManager != null)
        {
            // remove from enemy list (do not add score again)
            gameManager.enemies.Remove(gameObject);
        }

        if (enemyShip)
        {
            if (enemyMovement != null) enemyMovement.isDead = true;
            if (agent != null) agent.enabled = false;
        }

        if (enemyShoot != null)
            enemyShoot.enabled = false;

        if (enemyShip && enemySpawnerScript != null)
            enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

        // Drone-specific logic
        if (isDrone)
        {
            // Add drone death effects here if needed
        }

        // ✅ Explosion on death
        PlayExplosionEffect();

        gameObject.SetActive(false);
    }

    private void SpawnScorePopup()
    {
        if (scorePopupPrefab == null) return;

        GameObject popup = Instantiate(scorePopupPrefab, transform.position, Quaternion.identity);

        // Set the text
        TMPro.TextMeshProUGUI tmpText = popup.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
        {
            tmpText.text = scoreValue.ToString();
        }

        // Optional: Make it front-facing if not using FaceCamera
        if (Camera.main != null)
            popup.transform.LookAt(popup.transform.position + Camera.main.transform.forward);

        popup.transform.localScale = scorePopupScale;

        // Animate scale
        StartCoroutine(ScorePopupAnimation(popup));

        // Destroy after duration
        Destroy(popup, scorePopupDuration);
    }

    private IEnumerator ScorePopupAnimation(GameObject popup)
    {
        float timer = 0f;
        float animDuration = 0.3f; // pop animation time

        Vector3 startScale = scorePopupScale;
        Vector3 targetScale = scorePopupPopScale;

        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;
            popup.transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Sin(t * Mathf.PI * 0.5f)); // smooth pop
            yield return null;
        }

        // Lerp back to normal scale
        timer = 0f;
        while (timer < animDuration)
        {
            timer += Time.deltaTime;
            float t = timer / animDuration;
            popup.transform.localScale = Vector3.Lerp(targetScale, startScale, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }

        popup.transform.localScale = startScale;
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
