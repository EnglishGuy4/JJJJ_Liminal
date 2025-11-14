using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ProjectileHealth : MonoBehaviour
{
    public int health = 1;

    [Header("Scoring")]
    public int scoreValue = 50; // points if player destroys the projectile
    private bool lastHitByPlayer = false; // NEW

    [Header("Effects")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionLifetime = 3f;
    public float explosionScale = 1f;

    [Header("Audio")]
    [SerializeField] private AudioClip explosionSound;
    [SerializeField, Range(0f, 1f)] private float explosionVolume = 1f;
    [SerializeField] private bool explosionSound2D = false;
    [SerializeField] private float explosionMinDistance = 1f;
    [SerializeField] private float explosionMaxDistance = 50f;

    [Header("Score UI")]
    [SerializeField] private GameObject scorePopupPrefab;
    [SerializeField] private float scorePopupDuration = 1.5f;
    [SerializeField] private Vector3 scorePopupScale = new Vector3(1f, 1f, 1f);
    [SerializeField] private Vector3 scorePopupPopScale = new Vector3(1.5f, 1.5f, 1.5f);

    public void TakeDamage(int damage, bool fromPlayer)
    {
        lastHitByPlayer = fromPlayer;
        health -= damage;
        if (health <= 0)
            Death();
    }

    public void TakeDamage(int damage) => TakeDamage(damage, false);

    private void Death()
    {
        PlayExplosionEffect();

        if (lastHitByPlayer)
        {
            var gm = FindObjectOfType<GameManager>();
            if (gm != null && scoreValue > 0)
                gm.AddScore(scoreValue);

            // spawn score popup
            SpawnScorePopup();
        }

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

    private void SpawnScorePopup()
    {
        if (scorePopupPrefab == null) return;
        GameObject popup = Instantiate(scorePopupPrefab, transform.position + Vector3.up * 1f, Quaternion.identity);

        // set text if TMP present
        TMPro.TextMeshProUGUI tmpText = popup.GetComponentInChildren<TMPro.TextMeshProUGUI>();
        if (tmpText != null)
            tmpText.text = scoreValue.ToString();

        popup.transform.localScale = scorePopupScale;

        // face camera if possible
        if (Camera.main != null)
            popup.transform.LookAt(popup.transform.position + Camera.main.transform.forward);

        StartCoroutine(ScorePopupAnimation(popup));
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
            popup.transform.localScale = Vector3.Lerp(startScale, targetScale, Mathf.Sin(t * Mathf.PI * 0.5f));
            yield return null;
        }

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
}

