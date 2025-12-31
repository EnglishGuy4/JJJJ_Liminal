using System.Collections;
using UnityEngine;

public class UFOShield : MonoBehaviour
{
    [Header("Shield Visuals")]
    public Material shieldMaterial;
    public string shieldColorProperty = "_Color";
    public Color hitColor = Color.red;
    public Color normalColor = Color.cyan;
    public float colorFlashDuration = 0.5f;
    public float pulseSpeed = 2f;
    public float pulseIntensity = 0.2f;

    [Header("Shield Audio")]
    public AudioClip shieldHitClip;
    public float shieldHitVolume = 1f;
    private AudioSource audioSource;

    private bool isFlashing = false;
    private Coroutine flashRoutine;

    void Start()
    {
        // Setup audio source
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();
        audioSource.playOnAwake = false;

        // Initialize shield color
        if (shieldMaterial != null && shieldMaterial.HasProperty(shieldColorProperty))
            shieldMaterial.SetColor(shieldColorProperty, normalColor);
    }

    void Update()
    {
        // Pulse the shield color for a subtle glow when not flashing
        if (!isFlashing && shieldMaterial != null && shieldMaterial.HasProperty(shieldColorProperty))
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * pulseIntensity;
            Color pulsedColor = Color.Lerp(normalColor * (1f - pulseIntensity), normalColor, 1f - pulse);
            shieldMaterial.SetColor(shieldColorProperty, pulsedColor);
        }
    }

    public void OnHit()
    {
        // Called by CannonBall when collision occurs
        if (shieldMaterial == null) return;

        if (flashRoutine != null)
            StopCoroutine(flashRoutine);

        flashRoutine = StartCoroutine(FlashShieldColor());

        // Play sound
        if (shieldHitClip != null && audioSource != null)
            audioSource.PlayOneShot(shieldHitClip, shieldHitVolume);
    }

    private IEnumerator FlashShieldColor()
    {
        if (!shieldMaterial.HasProperty(shieldColorProperty))
            yield break;

        isFlashing = true;
        float halfDuration = colorFlashDuration / 2f;
        float elapsed = 0f;

        // Flash to hit color
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            shieldMaterial.SetColor(shieldColorProperty, Color.Lerp(normalColor, hitColor, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        shieldMaterial.SetColor(shieldColorProperty, hitColor);
        elapsed = 0f;

        // Return to normal color
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            shieldMaterial.SetColor(shieldColorProperty, Color.Lerp(hitColor, normalColor, t));
            elapsed += Time.deltaTime;
            yield return null;
        }

        shieldMaterial.SetColor(shieldColorProperty, normalColor);
        isFlashing = false;
    }

    private void OnCollisionEnter(Collision collision)
    {
        // Detect cannonball hits automatically
        if (collision.gameObject.CompareTag("CannonBall"))
        {
            OnHit();
        }
    }
}
