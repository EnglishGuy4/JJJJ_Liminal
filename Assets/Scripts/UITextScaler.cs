using UnityEngine;
using TMPro; // Only needed if you're using TextMeshPro

public class UITextScaler : MonoBehaviour
{
    [Header("Scale Settings")]
    [Tooltip("Minimum scale of the text")]
    public float minScale = 0.9f;

    [Tooltip("Maximum scale of the text")]
    public float maxScale = 1.1f;

    [Tooltip("Speed of scaling up and down")]
    public float speed = 2f;

    [Tooltip("Should the animation start immediately?")]
    public bool playOnStart = true;

    private Vector3 originalScale;
    private bool isAnimating = false;

    void Awake()
    {
        originalScale = transform.localScale;
    }

    void Start()
    {
        if (playOnStart)
        {
            StartScaling();
        }
    }

    void Update()
    {
        if (isAnimating)
        {
            float scale = Mathf.Lerp(minScale, maxScale, (Mathf.Sin(Time.time * speed) + 1f) / 2f);
            transform.localScale = originalScale * scale;
        }
    }

    public void StartScaling()
    {
        isAnimating = true;
    }

    public void StopScaling()
    {
        isAnimating = false;
        transform.localScale = originalScale; // Reset
    }
}
