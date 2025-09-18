using UnityEngine;

public static class ScoreManager
{
    public static int CurrentScore { get; set; }

    private const string HighScoreKey = "HighScore";

    public static int HighScore
    {
        get => PlayerPrefs.GetInt(HighScoreKey, 0);
        private set
        {
            PlayerPrefs.SetInt(HighScoreKey, value);
            PlayerPrefs.Save();
        }
    }

    public static void SubmitScore(int score)
    {
        CurrentScore = score;

        if (score > HighScore)
            HighScore = score;
    }
}
