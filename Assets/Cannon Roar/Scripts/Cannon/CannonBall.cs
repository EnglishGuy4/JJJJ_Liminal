using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour
{
    [HideInInspector] public Rigidbody rb;
    public float force = 1;
    public int damage = 1;
    //[HideInInspector] public TrailRenderer trailRenderer;
    private SphereCollider sphereCollider;
    public GameObject standerdHitPrefab;
    public GameObject powerupHitPrefab;
    public GameObject droneHitPrefab;
    

    [HideInInspector] public Cannon firedFrom; // <-- reference back to cannon that fired it

    private bool hasHit = false;

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
        //trailRenderer = GetComponent<TrailRenderer>();
        sphereCollider = GetComponent<SphereCollider>();
        /*shipHit = transform.GetChild(0).GetComponent<ParticleSystem>();
        waterHit = transform.GetChild(1).GetComponent<ParticleSystem>();
        rockHit = transform.GetChild(2).GetComponent<ParticleSystem>();*/
        
    }

    void OnEnable()
    {
        hasHit = false;
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
        //Debug.Log("CannonBall hit: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

        if (hasHit) return; // Prevent multiple hits per activation

        if (collision.gameObject.CompareTag("Ally"))
        {
            Physics.IgnoreCollision(collision.collider, sphereCollider);
        }

        if (collision.gameObject.CompareTag("Enemy"))
        {
            hasHit = true;
            //Debug.Log("Ship Collided");

            var enemyHealth = collision.gameObject.GetComponentInParent<EnemyHealth>();
            if (enemyHealth != null)
            {
                //Debug.Log("Found EnemyHealth!");
                Instantiate(standerdHitPrefab, transform.position, Quaternion.identity);
                enemyHealth.cannonBall = gameObject;
                enemyHealth.TakeDamage(damage);
                ResetBall();
            }
            else
            {
                var droneHealth = collision.gameObject.GetComponentInParent<DroneHealth>();
                if (droneHealth != null)
                {
                    //Debug.Log("Found DroneHealth!");
                    Instantiate(droneHitPrefab, transform.position, Quaternion.identity);
                    droneHealth.cannonBall = gameObject;
                    droneHealth.TakeDamage(damage);
                    ResetBall();
                }
                //else
                {
                    Debug.Log("NO EnemyHealth or DroneHealth found on: " + collision.gameObject.name);
                }
            }
            Instantiate(standerdHitPrefab, transform.position, Quaternion.identity);
            rb.velocity = rb.velocity / 2;
        }

        /*if (collision.gameObject.CompareTag("Ground") && transform.position.y >= 10)
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

            Instantiate(powerupHitPrefab, transform.position, Quaternion.identity);
            collision.gameObject.SetActive(false); // deactivate powerup
            ResetBall(); // also reset this cannonball
        }*/

        
        if (collision.gameObject.CompareTag("DroneShot"))
        {
            var projHealth = collision.gameObject.GetComponentInParent<ProjectileHealth>();
            if (projHealth != null)
            {
                projHealth.TakeDamage(damage); // or just projHealth.Death() if you want instant destruction
            }
            // Optionally, play a hit effect here
            ResetBall(); 
            return;
        }

        if (collision.gameObject.CompareTag("UFO"))
        {
            var ufoShield = collision.gameObject.GetComponentInParent<UFOShield>();
            if (ufoShield != null)
            {
                ufoShield.OnHit();
            }
        }

    }

    private void OnTriggerEnter(Collider trigger)
    {
        /*if (trigger.CompareTag("Water"))
        {
            waterHit.Play();
        }*/
    }

    void ResetBall()
    {
        //Debug.Log("ResetBall");
        //waterHit.Stop();
       // rockHit.Stop();
        //shipHit.Stop();
        rb.isKinematic = true;
        //trailRenderer.enabled = false;
        gameObject.SetActive(false);
    }
}
