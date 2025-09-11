using System.Collections;
using UnityEngine;

public class UFOController : MonoBehaviour
{
    [Header("Movement Settings")]
    public Transform startPosition;    // Where the UFO begins (off-screen, etc.)
    public float moveDuration = 3f;    // How long it takes to reach the final spot

    [Header("References")]
    public SpawnerManager spawnerManager; // Assign in Inspector or auto-find

    private Vector3 finalPosition;     // Where it was placed in the editor
    private bool isMoving = false;
    private Transform[] childObjects;

    private void Start()
    {
        // Save editor-placed position as the "final" destination
        finalPosition = transform.position;

        // Cache children
        childObjects = GetComponentsInChildren<Transform>(includeInactive: true);

        // Hide children at start
        SetChildrenActive(false);

        // Auto-find SpawnerManager if not set
        if (spawnerManager == null)
        {
            spawnerManager = FindObjectOfType<SpawnerManager>();
        }

        // Subscribe to waves beginning
        if (spawnerManager != null)
        {
            spawnerManager.BeginSpawningEvent += OnWavesBegin;
        }
    }

    private void OnDestroy()
    {
        if (spawnerManager != null)
        {
            spawnerManager.BeginSpawningEvent -= OnWavesBegin;
        }
    }

    private void OnWavesBegin()
    {
        if (!isMoving && startPosition != null)
        {
            // Place UFO at start position before moving
            transform.position = startPosition.position;

            // Turn children back on
            SetChildrenActive(true);

            StartCoroutine(MoveToFinalPosition());
        }
    }

    private IEnumerator MoveToFinalPosition()
    {
        isMoving = true;

        float elapsed = 0f;
        while (elapsed < moveDuration)
        {
            float t = elapsed / moveDuration;

            // 🔹 Smooth easing in/out
            t = Mathf.SmoothStep(0f, 1f, t);

            transform.position = Vector3.Lerp(startPosition.position, finalPosition, t);
            elapsed += Time.deltaTime;
            yield return null;
        }

        // Snap exactly to final editor-set position
        transform.position = finalPosition;
        isMoving = false;
    }

    private void SetChildrenActive(bool state)
    {
        foreach (Transform child in childObjects)
        {
            if (child != this.transform) // don’t disable the root
                child.gameObject.SetActive(state);
        }
    }
}
