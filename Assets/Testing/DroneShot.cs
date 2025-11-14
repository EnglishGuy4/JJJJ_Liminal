using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class DroneShot : MonoBehaviour
{
    public GameObject droneShotPrefab; // Assign your projectile prefab here
    public Transform barrelEnd;        // Assign your barrel end (empty GameObject) here
    public float timeBetweenShots = 1f;
    public float timer = 4f;
    public Transform target; // Assign your shield here
    [HideInInspector] public bool canShoot = false; // controlled by ship
    [HideInInspector] public SpawnerManager enemySpawnerScript;
    private GameManager gameManager;
    [SerializeField] private GameObject explosionPrefab;
    public float shieldDamage = 20f;

    

    private void Update()
    {
        if (target == null) return;
        if (!canShoot) return; // don't shoot until allowed

        timer += Time.deltaTime;
        if (timer >= timeBetweenShots)
        {
            ShootAtTarget();
            timer = 0f;
        }
    }

    private void ShootAtTarget()
    {
        //Debug.Log("Drone is shooting!");

        GameObject shot = PoolManager.current.GetPooledObject(droneShotPrefab.name);
        if (shot == null) return;

        shot.transform.position = barrelEnd.position;
        shot.transform.rotation = barrelEnd.rotation;

        DroneShotMovement projectile = shot.GetComponent<DroneShotMovement>();
        if (projectile != null)
        {
            projectile.startPos = barrelEnd.position;
            projectile.targetPos = target.position;
        }

        shot.SetActive(true); // <-- Set active AFTER setting positions!
    }

    private void PlayExplosionEffect()
    {
        
    }
}