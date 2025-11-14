using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneShotMovement : MonoBehaviour
{
    public Vector3 startPos;
    public Vector3 targetPos;
    public float speed = 10f;               // Adjustable speed in Inspector

    [Header("Shield / Fallback")]
    public float outOfBoundsY = -12f;       // fallback Y to apply shield damage if shot falls past world
    public float shieldDamage = 10f;        // amount to damage the shield

    [Header("Optional Hooks")]
    public SpawnerManager enemySpawnerScript; // optional, set by the shooter if you want bookkeeping

    private AudioSource audioSource;
    private GameManager gameManager;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        // set start position and play audio if available
        transform.position = startPos;
        if (audioSource != null) audioSource.Play();

        // cache GameManager reference
        gameManager = FindObjectOfType<GameManager>();
    }

    private void Update()
    {
        // Move straight toward the target
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Arrive if close enough
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
        {
            Despawn();
            return;
        }

        // Fallback: if projectile falls below world Y, treat as shield hit and despawn
        if (transform.position.y <= outOfBoundsY)
        {
            ApplyShieldDamageAndDespawn();
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        // Recommended: tag your shield GameObject with "Shield"
        if (other.CompareTag("Shield"))
        {
            ApplyShieldDamageAndDespawn();
            return;
        }

        // Handle other collisions here (walls, enemies, etc.) if needed
    }

    private void ApplyShieldDamageAndDespawn()
    {
        // Notify spawner bookkeeping (optional)
        if (enemySpawnerScript != null)
            enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);

        // Reduce player shield via GameManager (guarded)
        if (gameManager != null)
            gameManager.ModifyShield(-shieldDamage); // ensure ModifyShield exists on your GameManager

        // play any impact VFX/SFX here if desired

        Despawn();
    }

    private void Despawn()
    {
        if (audioSource != null) audioSource.Stop();
        gameObject.SetActive(false); // return to pool
    }

    private void OnDisable()
    {
        if (audioSource != null) audioSource.Stop();
    }
}
