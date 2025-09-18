using UnityEngine;
using TMPro;

public class ResultsUI : MonoBehaviour
{
    [Header("UI References")]
    public TextMeshProUGUI scoreText;         // Displays the current score
    public TextMeshProUGUI highScoreText;     // Displays just the number for high score
    public GameObject newHighScoreText;       // "New High Score!" text

    void Start()
    {
        // Show the player's score
        if (scoreText != null)
            scoreText.text = ScoreManager.CurrentScore.ToString();

        // Show the high score number
        if (highScoreText != null)
            highScoreText.text = ScoreManager.HighScore.ToString();

        // Show/hide "New High Score!" message
        if (newHighScoreText != null)
            newHighScoreText.SetActive(ScoreManager.CurrentScore == ScoreManager.HighScore);
    }
}
