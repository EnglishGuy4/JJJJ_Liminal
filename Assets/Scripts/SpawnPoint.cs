using UnityEngine;

public class SpawnPoint : MonoBehaviour
{
    [Header("Gizmo Settings")]
    public Color gizmoColor = Color.green;
    public float gizmoSize = 1f;

    // Draw gizmos only when not in play mode
    private void OnDrawGizmos()
    {
        if (!Application.isPlaying)
        {
            Gizmos.color = gizmoColor;
            Gizmos.DrawSphere(transform.position, gizmoSize);
        }
    }
}
