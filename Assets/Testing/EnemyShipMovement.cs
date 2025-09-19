using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipMovement : MonoBehaviour
{
    [Header("Forward Movement")]
    public float forwardSpeed = 10f;

    [Header("Arena Settings")]
    public Vector2 areaSize = new Vector2(200f, 200f); // X/Y size of play area
    public float targetChangeDistance = 50f;           // how far forward before picking a new target

    [Header("Wobble Motion")]
    public float horizontalRange = 2f;
    public float verticalRange = 1f;
    public float wobbleSpeed = 2f;

    private Vector3 targetOffset;  // local XY offset target
    private float phase;

    void Start()
    {
        phase = Random.Range(0f, Mathf.PI * 2f);
        PickNewOffset();
    }

    void Update()
    {
        // Always move forward
        transform.position += transform.forward * forwardSpeed * Time.deltaTime;

        // Wobble (side-to-side + up/down relative to ship’s orientation)
        float t = Time.time * wobbleSpeed + phase;
        Vector3 wobble = transform.right * Mathf.Sin(t) * horizontalRange +
                         transform.up * Mathf.Cos(t) * verticalRange;

        transform.position += wobble * Time.deltaTime;

        // If we’ve traveled far enough forward, pick a new side/up offset
        if (Vector3.Distance(transform.localPosition, targetOffset) < 1f)
        {
            PickNewOffset();
        }
    }

    void PickNewOffset()
    {
        // Pick a new random offset inside arena bounds
        float x = Random.Range(-areaSize.x / 2f, areaSize.x / 2f);
        float y = Random.Range(-areaSize.y / 2f, areaSize.y / 2f);

        // Keep z moving forward
        float z = transform.localPosition.z + targetChangeDistance;

        targetOffset = new Vector3(x, y, z);
    }
}
