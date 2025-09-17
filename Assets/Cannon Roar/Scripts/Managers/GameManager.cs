using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Fade Settings")]
    public Renderer fadePlaneRenderer; // Assign the plane in front of the VR camera
    public float fadeDuration = 1f;    // How long the fade takes
    private Material fadeMaterial;
    private string fadeColorProperty;


    [Header("Shield Settings")]
    public float currentShield = 100f;
    public float maxShield = 100f;
    public float minShield = 0f;
    public float shieldRegenRate = 2f;

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

    [Header("UI")]
    public Slider shieldSlider;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI waveText;
    public TextMeshProUGUI waveTimerText;
    public GameObject gameOverPanel;
    public TextMeshProUGUI gameOverTimerText; // 🔹 optional countdown display

    [Header("Score")]
    public int score = 0;

    [Header("Game Over Settings")]
    public bool gameOverOnShieldBreak = true;
    public string menuSceneName = "MainMenu"; // 🔹 set via inspector
    public float returnToMenuDelay = 5f;      // 🔹 countdown in seconds

    [HideInInspector]
    public System.Collections.Generic.List<GameObject> enemies = new System.Collections.Generic.List<GameObject>();

    [Header("Score Animation Settings")]
    public float popScale = 1.5f;
    public float popDuration = 0.2f;

    private Vector3 originalScale;
    private Coroutine flashRoutine;
    private bool isFlashing = false;
    private bool isGameOver = false;

    private SpawnerManager spawnerManager;

    private void Start()
    {
        // Prepare fade material
        if (fadePlaneRenderer != null)
        {
            fadeMaterial = fadePlaneRenderer.material;
            DetectFadeColorProperty();
            ForceMaterialTransparent(fadeMaterial);
            SetFadeAlpha(0f); // Start fully transparent
        }

        spawnerManager = FindObjectOfType<SpawnerManager>();

        if (shieldSlider != null)
        {
            shieldSlider.minValue = minShield;
            shieldSlider.maxValue = maxShield;
            shieldSlider.value = currentShield;
        }

        if (scoreText != null)
            originalScale = scoreText.transform.localScale;

        UpdateScoreUI();

        if (shieldMaterial != null && shieldMaterial.HasProperty(shieldColorProperty))
            shieldMaterial.SetColor(shieldColorProperty, normalColor);

        audioSource = GetComponent<AudioSource>();
        if (audioSource == null)
            audioSource = gameObject.AddComponent<AudioSource>();

        audioSource.playOnAwake = false;

        if (gameOverPanel != null)
            gameOverPanel.SetActive(false);

        if (gameOverTimerText != null)
            gameOverTimerText.text = "";
    }

    private void Update()
    {
        if (isGameOver) return;

        if (currentShield < maxShield)
        {
            currentShield += shieldRegenRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, minShield, maxShield);
            UpdateShieldUI();
        }

        if (!isFlashing && shieldMaterial != null && shieldMaterial.HasProperty(shieldColorProperty))
        {
            float pulse = (Mathf.Sin(Time.time * pulseSpeed) * 0.5f + 0.5f) * pulseIntensity;
            Color pulsedColor = Color.Lerp(normalColor * (1f - pulseIntensity), normalColor, 1f - pulse);
            shieldMaterial.SetColor(shieldColorProperty, pulsedColor);
        }

        if (gameOverOnShieldBreak && currentShield <= minShield)
        {
            TriggerGameOver();
        }
    }

    public void ModifyShield(float amount)
    {
        if (isGameOver) return;

        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, minShield, maxShield);
        Debug.Log("[GameManager] Shield modified: " + amount + " | Current: " + currentShield);
        UpdateShieldUI();

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

        if (gameOverOnShieldBreak && currentShield <= minShield)
        {
            TriggerGameOver();
        }
    }

    private void UpdateShieldUI()
    {
        if (shieldSlider != null)
            shieldSlider.value = currentShield;
    }

    private void TriggerGameOver()
    {
        if (isGameOver) return;
        isGameOver = true;

        Debug.Log("[GameManager] Game Over Triggered!");

        // Stop waves
        if (spawnerManager != null)
        {
            spawnerManager.StopAllCoroutines();
            spawnerManager.enabled = false;
        }

        // Disable UI
        if (waveText != null) waveText.gameObject.SetActive(false);
        if (waveTimerText != null) waveTimerText.gameObject.SetActive(false);
        if (shieldSlider != null) shieldSlider.gameObject.SetActive(false);

        // Show Game Over UI
        if (gameOverPanel != null)
            gameOverPanel.SetActive(true);

        // Start return-to-menu countdown
        StartCoroutine(ReturnToMenuCountdown());
    }

    private IEnumerator ReturnToMenuCountdown()
    {
        float timer = returnToMenuDelay;

        // Countdown loop — no fade yet
        while (timer > 0)
        {
            if (gameOverTimerText != null)
                gameOverTimerText.text = "Returning to Menu in " + Mathf.Ceil(timer).ToString() + "...";

            yield return new WaitForSeconds(1f);
            timer -= 1f;
        }

        // Countdown finished — now fade to black
        if (fadeMaterial != null)
            yield return StartCoroutine(Fade(0f, 1f));

        // Finally load the menu scene
        if (!string.IsNullOrEmpty(menuSceneName))
            UnityEngine.SceneManagement.SceneManager.LoadScene(menuSceneName);
        else
            Debug.LogError("[GameManager] Menu scene name is not set!");
    }



    public void AddScore(int amount)
    {
        if (isGameOver) return;

        score += amount;
        UpdateScoreUI();
        StartCoroutine(AnimateScoreText());
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
            scoreText.text = score.ToString();
    }

    private IEnumerator AnimateScoreText()
    {
        if (scoreText == null) yield break;

        float elapsed = 0f;
        while (elapsed < popDuration)
        {
            float t = elapsed / popDuration;
            scoreText.transform.localScale = Vector3.Lerp(originalScale, originalScale * popScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.transform.localScale = originalScale * popScale;

        elapsed = 0f;
        while (elapsed < popDuration)
        {
            float t = elapsed / popDuration;
            scoreText.transform.localScale = Vector3.Lerp(originalScale * popScale, originalScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        scoreText.transform.localScale = originalScale;
    }

    private IEnumerator FlashShieldColor()
    {
        if (!shieldMaterial.HasProperty(shieldColorProperty)) yield break;

        isFlashing = true;

        float halfDuration = colorFlashDuration / 2f;
        float elapsed = 0f;

        while (elapsed < halfDuration)
        {
            float t = elapsed / halfDuration;
            shieldMaterial.SetColor(shieldColorProperty, Color.Lerp(normalColor, hitColor, t));
            elapsed += Time.deltaTime;
            yield return null;
        }
        shieldMaterial.SetColor(shieldColorProperty, hitColor);

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

    private void DetectFadeColorProperty()
    {
        if (fadeMaterial == null) return;

        if (fadeMaterial.HasProperty("_Color")) fadeColorProperty = "_Color";
        else if (fadeMaterial.HasProperty("_BaseColor")) fadeColorProperty = "_BaseColor";
        else fadeColorProperty = null;
    }

    private void ForceMaterialTransparent(Material m)
    {
        if (m == null) return;

        m.SetInt("_SrcBlend", (int)UnityEngine.Rendering.BlendMode.SrcAlpha);
        m.SetInt("_DstBlend", (int)UnityEngine.Rendering.BlendMode.OneMinusSrcAlpha);
        m.SetInt("_ZWrite", 0);
        m.DisableKeyword("_ALPHATEST_ON");
        m.EnableKeyword("_ALPHABLEND_ON");
        m.DisableKeyword("_ALPHAPREMULTIPLY_ON");
        m.renderQueue = 3000;
    }

    private void SetFadeAlpha(float alpha)
    {
        if (fadeMaterial == null) return;

        Color c = Color.black;
        c.a = Mathf.Clamp01(alpha);

        if (!string.IsNullOrEmpty(fadeColorProperty) && fadeMaterial.HasProperty(fadeColorProperty))
            fadeMaterial.SetColor(fadeColorProperty, c);
        else
        {
            Color cur = fadeMaterial.color;
            cur.r = 0f; cur.g = 0f; cur.b = 0f; cur.a = c.a;
            fadeMaterial.color = cur;
        }
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            SetFadeAlpha(Mathf.Lerp(startAlpha, endAlpha, t));
            yield return null;
        }
        SetFadeAlpha(endAlpha);
    }

}
