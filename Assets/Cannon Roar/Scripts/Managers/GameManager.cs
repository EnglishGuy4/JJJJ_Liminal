using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class GameManager : MonoBehaviour
{
    [Header("Shield Settings")]
    public float currentShield = 100f;
    public float maxShield = 100f;
    public float minShield = 0f;
    public float shieldRegenRate = 2f;

    [Header("Shield Visuals")]
    public Material shieldMaterial;          // Plug shield material here
    public string shieldColorProperty = "_Color"; // Name of the color property (default "_Color")
    public Color hitColor = Color.red;       // Color when hit
    public Color normalColor = Color.cyan;   // Default/idle shield color
    public float colorFlashDuration = 0.5f;  // Total time for hit flash
    public float pulseSpeed = 2f;            // Speed of pulsing
    public float pulseIntensity = 0.2f;      // How much it shifts from normalColor

    [Header("Shield Audio")]
    public AudioClip shieldHitClip;          // Sound when shield is hit
    public float shieldHitVolume = 1f;       // Volume of hit sound
    private AudioSource audioSource;

    [Header("UI")]
    public Slider shieldSlider;
    public TextMeshProUGUI scoreText; // TMP score display

    [Header("Score")]
    public int score = 0;

    [HideInInspector]
    public System.Collections.Generic.List<GameObject> enemies = new System.Collections.Generic.List<GameObject>();

    [Header("Score Animation Settings")]
    public float popScale = 1.5f;   // how big it scales up
    public float popDuration = 0.2f; // time to scale up/down

    private Vector3 originalScale;
    private Coroutine flashRoutine;
    private bool isFlashing = false;

    private void Start()
    {
        if (shieldSlider != null)
        {
            shieldSlider.minValue = minShield;
            shieldSlider.maxValue = maxShield;
            shieldSlider.value = currentShield;
        }

        if (scoreText != null)
            originalScale = scoreText.transform.localScale;

        UpdateScoreUI();

        // Ensure shield starts at normal color
        if (shieldMaterial != null && shieldMaterial.HasProperty(shieldColorProperty))
        {
            shieldMaterial.SetColor(shieldColorProperty, normalColor);
        }

        // Setup AudioSource if not already attached
        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;
    }

    private void Update()
    {
        if (currentShield < maxShield)
        {
            currentShield += shieldRegenRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, minShield, maxShield);
            UpdateShieldUI();
        }

        // Idle pulsing (only if not in flash effect)
        if (!isFlashing && shieldMaterial != null && shieldMaterial.HasProperty(shieldColorProperty))
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * pulseIntensity;
            Color pulsedColor = Color.Lerp(normalColor * (1f - pulseIntensity), normalColor, 1f - pulse);
            shieldMaterial.SetColor(shieldColorProperty, pulsedColor);
        }
    }

    public void ModifyShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, minShield, maxShield);
        Debug.Log("[GameManager] Shield modified: " + amount + " | Current: " + currentShield);
        UpdateShieldUI();

        // Trigger shield flash & sound if it took damage
        if (amount < 0)
        {
            if (shieldMaterial != null)
            {
                if (flashRoutine != null)
                    StopCoroutine(flashRoutine);

                flashRoutine = StartCoroutine(FlashShieldColor());
            }

            if (shieldHitClip != null && audioSource != null)
            {
                audioSource.PlayOneShot(shieldHitClip, shieldHitVolume);
            }
        }
    }

    private void UpdateShieldUI()
    {
        if (shieldSlider != null)
            shieldSlider.value = currentShield;
    }

    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();
        StartCoroutine(AnimateScoreText()); // 🔥 trigger animation
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private IEnumerator AnimateScoreText()
    {
        if (scoreText == null) yield break;

        // scale up
        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            float t = elapsed / popDuration;
            scoreText.transform.localScale = Vector3.Lerp(originalScale, originalScale * popScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // hold peak
        scoreText.transform.localScale = originalScale * popScale;

        // scale back down
        elapsed = 0f;
        while (elapsed < popDuration)
        {
            float t = elapsed / popDuration;
            scoreText.transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.transform.localScale = originalScale; // reset
    }

    private IEnumerator FlashShieldColor()
    {
        if (!shieldMaterial.HasProperty(shieldColorProperty)) yield break;

        isFlashing = true;

        float halfDuration = colorFlashDuration / 2f;
        float elapsed = 0f;

        // Transition to hitColor
        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            shieldMaterial.SetColor(shieldColorProperty, Color.Lerp(normalColor, hitColor, t));
            elapsed += Time.deltaTime;
            yield return null;
        }
        shieldMaterial.SetColor(shieldColorProperty, hitColor);

        // Transition back to normalColor
        elapsed = 0f;
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
}
