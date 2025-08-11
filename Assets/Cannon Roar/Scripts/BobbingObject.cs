using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BobbingObject : MonoBehaviour
{
    private Vector3 _startPosition;
    public float speed;
    void Start()
    {
        _startPosition = transform.position;
    }

    void Update()
    {
        transform.position = _startPosition + new Vector3(0.0f, Mathf.Sin(Time.time) * speed, 0.0f);
        transform.Rotate(Vector3.left * speed, Space.Self);
    }
}
