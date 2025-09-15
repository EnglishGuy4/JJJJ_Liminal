using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTurret : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject cannonBall;        // Cannonball prefab (pooled)
    public Transform[] firePoints;       // Fire positions
    public float fireRate = 2f;          // Time between shots
    public float targetingRange = 100f;  // Max distance to look for enemies

    [Header("Targeting")]
    private Transform currentTarget;

    private float fireTimer;

    void Update()
    {
        // Refresh target if needed
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
        {
            currentTarget = FindRandomEnemy();
        }

        if (currentTarget == null) return;

        // Rotate towards target
        Vector3 dir = (currentTarget.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);
        transform.rotation = Quaternion.Slerp(transform.rotation, lookRot, Time.deltaTime * 2f);

        // Fire logic
        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            FireCannonballs();
            fireTimer = 0f;
        }
    }

    private void FireCannonballs()
    {
        foreach (Transform firePoint in firePoints)
        {
            if (firePoint == null) continue;

            GameObject pooledBall = PoolManager.current.GetPooledObject(cannonBall.name);
            if (pooledBall == null) return;

            CannonBall cb = pooledBall.GetComponent<CannonBall>();
            cb.firedFrom = null; // allies don’t need special reference
            cb.rb.transform.position = firePoint.position;
            cb.rb.transform.rotation = firePoint.rotation;
            pooledBall.SetActive(true);
            cb.rb.isKinematic = false;
            cb.trailRenderer.Clear();
            cb.trailRenderer.enabled = true;
            cb.rb.AddForce(cb.rb.transform.forward * cb.force, ForceMode.Impulse);
            cb.smokeEffect.Play();
        }
    }

    private Transform FindRandomEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");

        if (enemies.Length == 0)
            return null;

        // Option A: pick random enemy
        int rand = Random.Range(0, enemies.Length);
        return enemies[rand].transform;

        // Option B: pick closest enemy (if you want smarter allies)
        /*
        Transform closest = null;
        float minDist = Mathf.Infinity;
        foreach (var e in enemies)
        {
            float dist = Vector3.Distance(transform.position, e.transform.position);
            if (dist < minDist && dist <= targetingRange)
            {
                minDist = dist;
                closest = e.transform;
            }
        }
        return closest;
        */
    }
}
