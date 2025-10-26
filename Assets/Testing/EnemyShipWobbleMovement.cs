using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyShipWobbleMovement : MonoBehaviour
{
    [Header("Wobble Motion")]
    public float horizontalRange = 2f;
    public float verticalRange = 1f;
    public float wobbleSpeed = 2f;

    private float phase;
    private Vector3 startPosition;
    private Transform cachedTransform;

    void Start()
    {
        cachedTransform = transform;
        startPosition = cachedTransform.position;
        phase = Random.Range(0f, Mathf.PI * 2f);
    }

    void Update()
    {
        float t = Time.time * wobbleSpeed + phase;
        Vector3 wobble = cachedTransform.right * Mathf.Sin(t) * horizontalRange +
                         cachedTransform.up * Mathf.Cos(t) * verticalRange;

        // Set absolute position relative to start to avoid drift
        cachedTransform.position = startPosition + wobble;
    }
}
