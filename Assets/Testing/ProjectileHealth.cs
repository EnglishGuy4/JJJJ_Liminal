using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProjectileHealth : MonoBehaviour
{
    public int health = 1;

    [Header("Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifetime = 3f;
    public float explosionScale = 1f; // Add this at the top with your other fields

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField, Range(0f, 1f)] private float explosionVolume = 1f;
    [SerializeField] private bool explosionSound2D = false;
    [SerializeField] private float explosionMinDistance = 1f;
    [SerializeField] private float explosionMaxDistance = 50f;

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
            Death();
    }

    private void Death()
    {
        PlayExplosionEffect();
        gameObject.SetActive(false);
    }

    private void PlayExplosionEffect()
    {
        if (explosionPrefab != null)
        {
            GameObject explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);

            // Scale the transform
            explosion.transform.localScale *= explosionScale;

            // Scale the particle system's start size
            var ps = explosion.GetComponent<ParticleSystem>();
            if (ps != null)
            {
                var main = ps.main;
                main.startSize = explosionScale; // This OVERRIDES the prefab's start size!
            }

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

