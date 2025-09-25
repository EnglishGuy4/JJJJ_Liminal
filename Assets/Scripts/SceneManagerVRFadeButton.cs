using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerVRFadeButton : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Default scene to load if no override is passed in")]
    public string defaultSceneToLoad;

    [Header("Fade Settings")]
    [Tooltip("Fade object with a Renderer (plane, quad, etc.)")]
    public GameObject fadeObject;
    [Tooltip("Optional override for fade material")]
    public Material fadeMaterial;
    [Tooltip("Seconds it takes to fade in/out")]
    public float fadeDuration = 1f;

    [Header("Audio")]
    public AudioSource clickSound;

    private bool isFading = false;

    private void Start()
    {
        // If no material assigned manually, get it from fadeObject
        if (fadeMaterial == null && fadeObject != null)
        {
            Renderer r = fadeObject.GetComponent<Renderer>();
            if (r != null)
                fadeMaterial = r.material;  // runtime instance
        }

        if (fadeMaterial != null)
        {
            // Start fully black
            Color color = fadeMaterial.color;
            color.a = 1f;
            fadeMaterial.color = color;

            // Fade into scene
            StartCoroutine(FadeIn());
        }
    }


    /// <summary>
    /// Call this from a UI Button or script to fade out and load the default scene.
    /// </summary>
    public void LoadSceneWithFade()
    {
        LoadSceneWithFade(defaultSceneToLoad);
    }

    /// <summary>
    /// Call this with a scene name to fade out and load that scene.
    /// </summary>
    public void LoadSceneWithFade(string sceneName)
    {
        if (!isFading && !string.IsNullOrEmpty(sceneName))
            StartCoroutine(FadeOutAndLoad(sceneName));
    }

    private IEnumerator FadeOutAndLoad(string sceneName)
    {
        isFading = true;

        if (clickSound != null && !clickSound.isPlaying)
            clickSound.Play();

        // Fade to black
        yield return StartCoroutine(FadeOut());

        // Load next scene after fade
        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator FadeOut()
    {
        if (fadeMaterial != null)
        {
            float timer = 0f;
            Color color = fadeMaterial.color;
            color.a = 0f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(0f, 1f, timer / fadeDuration);
                fadeMaterial.color = color;
                yield return null;
            }

            color.a = 1f;
            fadeMaterial.color = color;
        }
        isFading = false;
    }

    private IEnumerator FadeIn()
    {
        if (fadeMaterial != null)
        {
            float timer = 0f;
            Color color = fadeMaterial.color;
            color.a = 1f;

            while (timer < fadeDuration)
            {
                timer += Time.deltaTime;
                color.a = Mathf.Lerp(1f, 0f, timer / fadeDuration);
                fadeMaterial.color = color;
                yield return null;
            }

            color.a = 0f;
            fadeMaterial.color = color;
        }
        isFading = false;
    }

    // 🔹 NEW FUNCTION FOR UI BUTTON
    public void LoadSceneFromUIButton(string sceneName)
    {
        LoadSceneWithFade(sceneName);
    }

    private void OnDisable()
    {
        ResetFadeMaterial();
    }

    private void OnApplicationQuit()
    {
        ResetFadeMaterial();
    }

    private void ResetFadeMaterial()
    {
        if (fadeMaterial != null && fadeMaterial.HasProperty("_Color"))
        {
            Color c = fadeMaterial.color;
            c.a = 0f; // fully transparent
            fadeMaterial.color = c;
            Debug.Log("[SceneManagerVRFadeButton] Reset fade material to alpha=0");
        }
    }


}
