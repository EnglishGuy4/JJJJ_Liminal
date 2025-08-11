using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class EnemyHealth : MonoBehaviour
{
    [HideInInspector]
    public NavMeshAgent agent;
    public int health = 1;
    private float time;
    private EnemyShoot enemyShoot;
    [HideInInspector]
    public SpawnerManager enemySpawnerScript;
    [HideInInspector] 
    public BoxCollider boxCollider;
    private BoxCollider waterCollider;
    private MeshCollider environment;
    private GameManager gameManager;
    private EnemyMovement enemyMovement;
    public bool bossShip;
    public bool enemyShip;
    [HideInInspector]
    public GameObject cannonBall;


    // Start is called before the first frame update
    void Awake()
    {
        waterCollider = GameObject.Find("water").GetComponent<BoxCollider>();
        environment = GameObject.Find("Environment").GetComponent<MeshCollider>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
        if (enemyShip)
        {
            boxCollider = GetComponentInChildren<BoxCollider>();
            agent = GetComponentInChildren<NavMeshAgent>();
            enemyShoot = GetComponentInChildren<EnemyShoot>();
            
        }

        if(bossShip)
        {
            boxCollider = GetComponent<BoxCollider>();
            enemyShoot = GetComponent<EnemyShoot>();
        }
    }

    private void Start()
    {
        enemyMovement = GetComponent<EnemyMovement>();
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.position.y <= -12f)
        {
            if(enemyShip)
                enemySpawnerScript.enemiesFromThisSpawnerList.Remove(gameObject);
            gameObject.SetActive(false);
        }
    }

    public void TakeDamage(int damage)
    {
        health -= damage;
        if (health <= 0)
        {
            Death();
        }
    }

    public void Death()
    {
        //if(bossShip)
        //{
            //gameManager.Invoke("Fader", 3f);
            //Invoke("Quit", 6f);
        //}
        time = 0;
        gameManager.enemies.Remove(gameObject);
        if (enemyShip)
        {
            enemyMovement.isDead = true;
            agent.enabled = false;
        }
        enemyShoot.enabled = false;
        StartCoroutine(SinkShip());
    }

    IEnumerator SinkShip()
    {
        Physics.IgnoreCollision(boxCollider, waterCollider);
        Physics.IgnoreCollision(boxCollider, environment);
        Vector3 position = transform.position;
        Vector3 endPosition = new Vector3(position.x, -12.5f, position.z);
        Quaternion startRotation = transform.rotation;
        Quaternion rotation = new Quaternion(0, 0, -45, transform.rotation.w);
        while (position != endPosition)
        {
            transform.position = Vector3.Lerp(position, endPosition, time / 5f);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotation, 0.0005f * Time.deltaTime);
            time += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        yield return null;
    }
}
