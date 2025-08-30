using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public float force = 1;
    public int damage = 1;
    [HideInInspector] public TrailRenderer trailRenderer;
    private SphereCollider sphereCollider;
    ParticleSystem shipHit;
    ParticleSystem waterHit;
    ParticleSystem rockHit;
    [HideInInspector] public ParticleSystem smokeEffect;

    [HideInInspector] public Cannon firedFrom; // <-- reference back to cannon that fired it

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        trailRenderer = GetComponent<TrailRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        shipHit = transform.GetChild(0).GetComponent<ParticleSystem>();
        waterHit = transform.GetChild(1).GetComponent<ParticleSystem>();
        rockHit = transform.GetChild(2).GetComponent<ParticleSystem>();
        smokeEffect = transform.GetChild(3).GetComponent<ParticleSystem>();
    }

    void Update()
    {
        if (transform.position.y <= -10)
        {
            ResetBall();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Ally"))
        {
            Physics.IgnoreCollision(collision.collider, sphereCollider);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            Debug.Log("Ship Collided");
            collision.gameObject.GetComponentInParent<EnemyHealth>().cannonBall = gameObject;
            collision.gameObject.GetComponentInParent<EnemyHealth>().TakeDamage(damage);
            shipHit.Play();
            rb.velocity = rb.velocity / 2;
        }

        if (collision.gameObject.CompareTag("Ground") && transform.position.y >= 10)
        {
            Debug.Log("Cliff Collided");
            rockHit.Play();
            rb.velocity = rb.velocity / 2;
        }

        if (collision.gameObject.CompareTag("Ground") && transform.position.y < 1)
        {
            Invoke("ResetBall", 2f);
        }

        // ---------- NEW: PowerUp ----------
        if (collision.gameObject.CompareTag("PowerUp"))
        {
            Debug.Log("Hit PowerUp!");
            if (firedFrom != null)
                firedFrom.ActivatePowerUp();

            collision.gameObject.SetActive(false); // deactivate powerup
            ResetBall(); // also reset this cannonball
        }
    }

    private void OnTriggerEnter(Collider trigger)
    {
        if (trigger.CompareTag("Water"))
        {
            waterHit.Play();
        }
    }

    void ResetBall()
    {
        Debug.Log("ResetBall");
        waterHit.Stop();
        rockHit.Stop();
        shipHit.Stop();
        rb.isKinematic = true;
        trailRenderer.enabled = false;
        gameObject.SetActive(false);
    }
}
