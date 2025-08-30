using UnityEngine;

public class ContinuousRotation : MonoBehaviour
{
    [Header("Rotation Settings")]
    public Vector3 rotationSpeed = new Vector3(0f, 100f, 0f); // Degrees per second

    void Update()
    {
        // Rotate the object every frame
        transform.Rotate(rotationSpeed * Time.deltaTime);
    }
}
