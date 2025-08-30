using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Liminal.SDK.Core;
using Liminal.Core.Fader;
using TMPro;

public class GameManager : MonoBehaviour
{
    // Settings
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> allies = new List<GameObject>();

    public bool gameMusic = true;
    public AudioSource gameMusicAudioSource;
    public GameObject bossShip;

    // Score system
    private int score = 0;
    public TextMeshProUGUI scoreText; // Assign in Inspector

    [Header("Score Animation Settings")]
    public float scaleAmount = 1.2f;   // How much bigger the text gets
    public float animationTime = 0.2f; // Time to scale up and back

    private Vector3 originalScale;
    private Coroutine scoreAnimCoroutine;

    private void Awake()
    {
        Invoke("Quit", 150f);
        Invoke("Fader", 147f);
        Invoke("BossShip", 60f);
        enemies.Add(bossShip);

        if (scoreText != null)
            originalScale = scoreText.rectTransform.localScale;

        UpdateScoreUI();
    }

    // Called by enemies when they die
    public void AddScore(int amount)
    {
        score += amount;
        UpdateScoreUI();

        if (scoreAnimCoroutine != null)
            StopCoroutine(scoreAnimCoroutine);

        scoreAnimCoroutine = StartCoroutine(AnimateScorePop());
    }

    private void UpdateScoreUI()
    {
        if (scoreText != null)
        {
            scoreText.text = score.ToString(); // ✅ just the number now
        }
        else
        {
            Debug.LogWarning("No Score Text (TextMeshProUGUI) assigned in GameManager!");
        }
    }

    private IEnumerator AnimateScorePop()
    {
        float halfTime = animationTime / 2f;
        Vector3 targetScale = originalScale * scaleAmount;

        // Scale up
        float t = 0;
        while (t < halfTime)
        {
            t += Time.deltaTime;
            float lerp = t / halfTime;
            scoreText.rectTransform.localScale = Vector3.Lerp(originalScale, targetScale, lerp);
            yield return null;
        }

        // Scale back
        t = 0;
        while (t < halfTime)
        {
            t += Time.deltaTime;
            float lerp = t / halfTime;
            scoreText.rectTransform.localScale = Vector3.Lerp(targetScale, originalScale, lerp);
            yield return null;
        }

        scoreText.rectTransform.localScale = originalScale;
    }

    public bool GameMusicToggle()
    {
        if (!gameMusicAudioSource)
        {
            Debug.LogWarning("No Audio Source for game music found");
            return false;
        }
        if (gameMusic && gameMusicAudioSource.isPlaying)
        {
            gameMusic = false;
            gameMusicAudioSource.Stop();
            return false;
        }
        else
        {
            gameMusic = true;
            gameMusicAudioSource.Play();
            return true;
        }
    }

    public void Quit()
    {
        ExperienceApp.End();
    }

    public void Fader()
    {
        var fader = ScreenFader.Instance;
        fader.FadeToBlack();
    }

    private void BossShip()
    {
        bossShip.SetActive(true);
    }

    public void TriggerVictory()
    {
        Debug.Log("All waves complete! Victory!");

        // Example: just fade out after a short delay
        StartCoroutine(VictorySequence());
    }

    private IEnumerator VictorySequence()
    {
        // Optional: Wait 2 seconds so player sees the "All Waves Complete!" text
        yield return new WaitForSeconds(2f);

        // Fade out
        var fader = ScreenFader.Instance;
        fader.FadeToBlack();

        // Wait for fade to finish
        yield return new WaitForSeconds(2f);

        // End experience (or load victory scene instead)
        ExperienceApp.End();
        // OR: SceneManager.LoadScene("VictoryScene");
    }

}
