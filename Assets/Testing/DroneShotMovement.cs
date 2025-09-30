using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Xsl;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DroneShotMovement : MonoBehaviour
{
    public Vector3 targetPos;
    public Vector3 startPos;
    public float speed = 10f; // Adjustable speed in Inspector

    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnEnable()
    {
        audioSource.Play();
        transform.position = startPos;
    }

    private void Update()
    {
        // Move straight toward the target
        transform.position = Vector3.MoveTowards(transform.position, targetPos, speed * Time.deltaTime);

        // Arrive if close enough
        if (Vector3.Distance(transform.position, targetPos) < 0.1f)
            Arrived();
    }

    private void Arrived()
    {
        gameObject.SetActive(false);
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }
}
