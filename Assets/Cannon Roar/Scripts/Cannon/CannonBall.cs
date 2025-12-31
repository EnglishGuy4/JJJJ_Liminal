using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CannonBall : MonoBehaviour, IResettableShot
{
    [HideInInspector] public Rigidbody rb;
    public float force = 1;
    public int damage = 1;
   
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
        
        sphereCollider = GetComponent<SphereCollider>();
        
        
    }

    void OnEnable()
    {
        hasHit = false;
        rb.isKinematic = false;
        rb.collisionDetectionMode = CollisionDetectionMode.Continuous;
    }

    void Update()
    {
        if (transform.position.y <= -10)
        {
            ResetShot();
        }
    }

    void OnCollisionEnter(Collision collision)
    {
        //Debug.Log("CannonBall hit: " + collision.gameObject.name + " (Tag: " + collision.gameObject.tag + ")");

        if (hasHit) return; 

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
                ResetShot();
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
                    ResetShot();
                }
                /*else
                {
                    Debug.Log("NO EnemyHealth or DroneHealth found on: " + collision.gameObject.name);
                }*/
            }
            Instantiate(standerdHitPrefab, transform.position, Quaternion.identity);
            rb.velocity = rb.velocity / 2;
        }

        

        
        if (collision.gameObject.CompareTag("DroneShot"))
        {
            var projHealth = collision.gameObject.GetComponentInParent<ProjectileHealth>();
            if (projHealth != null)
            {
                projHealth.TakeDamage(damage); 
            }
            
            ResetShot(); 
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



    public void ResetShot()
    {
        rb.velocity = Vector3.zero;
        rb.angularVelocity = Vector3.zero;

        rb.collisionDetectionMode = CollisionDetectionMode.ContinuousSpeculative; // or Discrete
        rb.isKinematic = true;

        gameObject.SetActive(false);
    }
}
