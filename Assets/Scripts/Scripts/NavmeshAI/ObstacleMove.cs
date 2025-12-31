using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleMove : MonoBehaviour
{
    [SerializeField]
    public float speed,distance;

    private Vector3 startPos;
    // Start is called before the first frame update
    void Start()
    {
        startPos = transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        var z = distance * Mathf.Sin(Time.time * speed) * Time.deltaTime;

        transform.Translate(new Vector3(0, 0, z));
    }
}
