using UnityEngine;

public class PingPongMovement : MonoBehaviour
{
    [Header("Movement Settings")]
    public Vector3 moveDirection = Vector3.right; // Direction to move
    public float moveDistance = 5f;               // How far to move
    public float speed = 2f;                       // Movement speed

    private Vector3 startPosition;

    void Start()
    {
        startPosition = transform.position;
    }

    void Update()
    {
        // Calculate movement using PingPong
        float offset = Mathf.PingPong(Time.time * speed, moveDistance);
        transform.position = startPosition + moveDirection.normalized * offset;
    }

}
