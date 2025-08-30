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
    }

    private void Update()
    {
        if (currentShield < maxShield)
        {
            currentShield += shieldRegenRate * Time.deltaTime;
            currentShield = Mathf.Clamp(currentShield, minShield, maxShield);
            UpdateShieldUI();
        }
    }

    public void ModifyShield(float amount)
    {
        currentShield += amount;
        currentShield = Mathf.Clamp(currentShield, minShield, maxShield);
        Debug.Log("[GameManager] Shield modified: " + amount + " | Current: " + currentShield);
        UpdateShieldUI();
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
}
