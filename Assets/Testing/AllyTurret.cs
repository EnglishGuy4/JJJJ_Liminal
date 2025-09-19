using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AllyTurret : MonoBehaviour
{
    [Header("Shooting Settings")]
    public GameObject cannonBall;        
    public Transform[] firePoints;       // Only need firepoints now
    public float fireRate = 2f;
    public float targetingRange = 100f;

    [Header("Targeting")]
    private Transform currentTarget;
    public Transform turretHead; // Assign this in the Inspector
    public Transform turretCannon; // Assign this in the Inspector

    [Header("Recoil Settings")]
    public float recoilDistance = 0.5f;
    public float recoilReturnSpeed = 8f;
    private Vector3 cannonOriginalLocalPos;
    private bool isRecoiling = false;
    private float fireTimer;
    private ParticleSystem[] muzzleFlashes;

    void Start()
    {
        // Auto-grab muzzle flashes from firepoints
        muzzleFlashes = new ParticleSystem[firePoints.Length];
        for (int i = 0; i < firePoints.Length; i++)
        {
            if (firePoints[i] != null)
                muzzleFlashes[i] = firePoints[i].GetComponentInChildren<ParticleSystem>();
        }

        // Store original local position for recoil
        if (turretCannon != null)
            cannonOriginalLocalPos = turretCannon.localPosition;
    }

    void Update()
    {
        if (currentTarget == null || !currentTarget.gameObject.activeInHierarchy)
            currentTarget = FindRandomEnemy();

        if (currentTarget == null) return;

        Vector3 dir = (currentTarget.position - transform.position).normalized;
        Quaternion lookRot = Quaternion.LookRotation(dir);

        if (turretHead != null)
            turretHead.rotation = Quaternion.Slerp(turretHead.rotation, lookRot, Time.deltaTime * 2f);

        // Recoil return logic
        if (turretCannon != null)
        {
            if (isRecoiling)
            {
                // Lerp back to original position
                turretCannon.localPosition = Vector3.Lerp(turretCannon.localPosition, cannonOriginalLocalPos, Time.deltaTime * recoilReturnSpeed);
                // Stop lerping if close enough
                if (Vector3.Distance(turretCannon.localPosition, cannonOriginalLocalPos) < 0.01f)
                {
                    turretCannon.localPosition = cannonOriginalLocalPos;
                    isRecoiling = false;
                }
            }
        }

        fireTimer += Time.deltaTime;
        if (fireTimer >= fireRate)
        {
            FireCannonballs();
            fireTimer = 0f;
        }
    }

    private void FireCannonballs()
    {
        for (int i = 0; i < firePoints.Length; i++)
        {
            Transform firePoint = firePoints[i];
            if (firePoint == null) continue;

            GameObject pooledBall = PoolManager.current.GetPooledObject(cannonBall.name);
            if (pooledBall == null) return;

            CannonBall cb = pooledBall.GetComponent<CannonBall>();
            cb.firedFrom = null;
            cb.rb.transform.position = firePoint.position;
            cb.rb.transform.rotation = firePoint.rotation;
            pooledBall.SetActive(true);
            cb.rb.isKinematic = false;
            cb.trailRenderer.Clear();
            cb.trailRenderer.enabled = true;
            cb.rb.AddForce(cb.rb.transform.forward * cb.force, ForceMode.Impulse);
            cb.smokeEffect.Play();

            // 🎇 Play muzzle flash if it exists
            if (muzzleFlashes[i] != null)
            {
                var main = muzzleFlashes[i].main;
                main.startRotation = Random.Range(0f, Mathf.PI * 2f);
                muzzleFlashes[i].Play();
            }
        }

        // Trigger recoil
        if (turretCannon != null)
        {
            turretCannon.localPosition = cannonOriginalLocalPos - Vector3.forward * recoilDistance;
            isRecoiling = true;
        }
    }

    private Transform FindRandomEnemy()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        if (enemies.Length == 0) return null;
        return enemies[Random.Range(0, enemies.Length)].transform;
    }

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
    }
        */
    
}
