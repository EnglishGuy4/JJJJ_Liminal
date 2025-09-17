using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneManagerVRFadeButton : MonoBehaviour
{
    [Header("Scene Settings")]
    [Tooltip("Default scene to load if no override is passed in")]
    public string defaultSceneToLoad;

    [Header("Fade Settings")]
    [Tooltip("Renderer of the fade plane (with a transparent-capable material)")]
    public Renderer fadePlaneRenderer;
    [Tooltip("Seconds it takes to fade in/out")]
    public float fadeDuration = 1f;

    [Header("Audio")]
    public AudioSource clickSound;

    private Material fadeMaterial;
    private string colorPropertyName;
    private bool isFading = false;

    private void Start()
    {
        if (fadePlaneRenderer != null)
        {
            fadeMaterial = fadePlaneRenderer.material;
            DetectColorProperty();
            ForceMaterialTransparent(fadeMaterial);

            // Start black and fade in
            SetAlpha(1f);
            StartCoroutine(Fade(1f, 0f));
        }
    }

    /// <summary>
    /// Call this from a UI Button or script to fade out and load a scene.
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

        yield return StartCoroutine(Fade(0f, 1f));

        SceneManager.LoadScene(sceneName);
    }

    private IEnumerator Fade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.Clamp01(elapsed / fadeDuration);
            float a = Mathf.Lerp(startAlpha, endAlpha, t);
            SetAlpha(a);
            yield return null;
        }
        SetAlpha(endAlpha);
    }

    private void SetAlpha(float alpha)
    {
        if (fadeMaterial == null) return;

        Color c = Color.black;
        c.a = Mathf.Clamp01(alpha);

        if (!string.IsNullOrEmpty(colorPropertyName) && fadeMaterial.HasProperty(colorPropertyName))
            fadeMaterial.SetColor(colorPropertyName, c);
        else
        {
            Color cur = fadeMaterial.color;
            cur.r = 0f; cur.g = 0f; cur.b = 0f; cur.a = c.a;
            fadeMaterial.color = cur;
        }
    }

    private void DetectColorProperty()
    {
        if (fadeMaterial == null) return;

        if (fadeMaterial.HasProperty("_Color")) colorPropertyName = "_Color";
        else if (fadeMaterial.HasProperty("_BaseColor")) colorPropertyName = "_BaseColor";
        else colorPropertyName = null;
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
}
