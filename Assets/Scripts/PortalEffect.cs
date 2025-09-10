using System.Collections;
using UnityEngine;

public class PortalEffect : MonoBehaviour
{
    [SerializeField] private float scaleDuration = 0.5f; // grow/shrink speed
    [SerializeField] private float lifeTime = 2f;        // how long it stays at full size

    private Vector3 targetScale;

    private void Awake()
    {
        targetScale = transform.localScale;   // whatever size you set in prefab
        transform.localScale = Vector3.zero;  // start at 0
    }

    private void OnEnable()
    {
        StartCoroutine(ScaleRoutine());
    }

    private IEnumerator ScaleRoutine()
    {
        // Grow in
        float t = 0f;
        while (t < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(Vector3.zero, targetScale, t / scaleDuration);
            t += Time.deltaTime;
            yield return null;
        }
        transform.localScale = targetScale;

        // Stay at full size
        yield return new WaitForSeconds(lifeTime);

        // Shrink out
        t = 0f;
        while (t < scaleDuration)
        {
            transform.localScale = Vector3.Lerp(targetScale, Vector3.zero, t / scaleDuration);
            t += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;

        Destroy(gameObject); // clean up
    }
}
