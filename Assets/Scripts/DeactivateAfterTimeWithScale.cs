using UnityEngine;
using System.Collections;

public class DeactivateAfterTimeWithScale : MonoBehaviour
{
    [Header("Timing Settings")]
    [Tooltip("Time in seconds before the scale animation begins")]
    [SerializeField] private float deactivateDelay = 5f;

    [Tooltip("Duration of the scale animation before deactivation")]
    [SerializeField] private float scaleDuration = 1f;

    [Header("Scale Animation Settings")]
    [Tooltip("Starting scale before the animation")]
    [SerializeField] private Vector3 startScale = Vector3.one;

    [Tooltip("Ending scale before deactivation")]
    [SerializeField] private Vector3 endScale = Vector3.zero;

    private Coroutine deactivateRoutine;

    private void OnEnable()
    {
        // Reset scale to starting value and start coroutine
        transform.localScale = startScale;
        deactivateRoutine = StartCoroutine(DeactivateAfterDelay());
    }

    private void OnDisable()
    {
        // Stop the coroutine if object is deactivated early
        if (deactivateRoutine != null)
            StopCoroutine(deactivateRoutine);
    }

    private IEnumerator DeactivateAfterDelay()
    {
        // Wait until it's time to start the scale animation
        yield return new WaitForSeconds(deactivateDelay);

        // Animate scale from startScale to endScale over scaleDuration
        float elapsed = 0f;
        while (elapsed < scaleDuration)
        {
            float t = elapsed / scaleDuration;
            transform.localScale = Vector3.Lerp(startScale, endScale, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure final scale is exact
        transform.localScale = endScale;

        // Deactivate object after animation
        gameObject.SetActive(false);
    }
}
