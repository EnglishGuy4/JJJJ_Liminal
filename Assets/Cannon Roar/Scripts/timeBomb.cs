using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class timeBomb : MonoBehaviour
{
    [SerializeField] float explosionRange;
    [SerializeField] float timeToBlow;
    [SerializeField] float explosionPower;

    void Start()
    {
        Invoke("Explode", timeToBlow);
    }

    void Explode()
    {
        Collider[] hitObjects = Physics.OverlapSphere(transform.position, explosionRange);

        foreach (Collider hitItem in hitObjects)
        {
           hitItem.attachedRigidbody?.AddExplosionForce(explosionPower, transform.position, explosionRange, 3.0f);
        }
        Destroy(gameObject);
        
    }

}
