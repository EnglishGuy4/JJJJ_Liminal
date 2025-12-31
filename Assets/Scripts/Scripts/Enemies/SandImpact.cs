using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandImpact : MonoBehaviour
{
    ParticleSystem sandHit;
    
    void Start()
    {
        sandHit = transform.GetChild(0).GetComponent<ParticleSystem>();
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.CompareTag("EnemyCannonBall"))
        {
            sandHit.Play();
        }
    }
}
