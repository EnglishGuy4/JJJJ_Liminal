using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Xml.Xsl;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class EnemyCannonBall : MonoBehaviour
{
    private float count = 0f;

    public Vector3 targetPos;
    public Vector3 startPos;

    private float distance;
    public float maxDistance = 180f;

    public AudioSource audioSource;

    private void Awake()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (count < 1)
        {
            count += 0.5f * Time.deltaTime;
            Vector3 halfway = startPos + (targetPos - startPos) / 2 + Vector3.up * (30 * distance);
            Vector3 m1 = Vector3.Lerp(startPos, halfway, count / distance);
            Vector3 m2 = Vector3.Lerp(halfway, targetPos, count / distance);
            transform.position = Vector3.Lerp(m1, m2, count / distance);
            
        }
        
        if (transform.position == targetPos)
            Arrived();
    }

    private void Arrived()
    {
        count = 0f;
        gameObject.SetActive(false);
    }

    private void OnEnable()
    {
        audioSource.Play();
        distance = Vector3.Distance(startPos, targetPos);
        distance = distance / maxDistance;
        distance = Mathf.Clamp01(distance);
    }

    private void OnDisable()
    {
        audioSource.Stop();
    }
}
