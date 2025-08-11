using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using System.Collections.Generic;
using UnityEngine.SceneManagement;
using Liminal.SDK.Core;
using Liminal.Core.Fader;


public class GameManager : MonoBehaviour
{
    //Make this class accessible from any class - GameController.gameController.Variable or method name
    //public static GameManager gameManager;

    //Settings
    public List<GameObject> enemies = new List<GameObject>();
    public List<GameObject> allies = new List<GameObject>();

    public bool gameMusic = true;

    public AudioSource gameMusicAudioSource;

    public GameObject bossShip;

    private void Awake()
    {
        Invoke("Quit", 150f);
        Invoke("Fader", 147f);
        Invoke("BossShip", 60f);
        enemies.Add(bossShip);
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
        //Application.Quit();
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
}
