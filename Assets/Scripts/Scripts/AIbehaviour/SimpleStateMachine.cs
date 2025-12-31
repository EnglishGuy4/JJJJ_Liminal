using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class SimpleStateMachine : MonoBehaviour
{
    private NavMeshAgent agent;

    public Transform target;

    public float sightRadius;

    public enum STATES {IDLE, ROAMING, CHASING, ATTACKING}

    public STATES currentState;

    [SerializeField] Transform[] nodes;
    int currentNode;


    private void Awake()
    {
        currentState = STATES.IDLE;
    }

    // Start is called before the first frame update
    void Start()
    {
        agent = GetComponent<NavMeshAgent>();
        StartCoroutine(EnemyFSM());
        
    }



    #region State machine Coroutines
    IEnumerator EnemyFSM()
    {
        while (true)
        {
            yield return StartCoroutine(currentState.ToString());
        }
        
    }

    IEnumerator IDLE()
    {
        //ENTER THE STATE run behaviours or state start here
        Debug.Log("I AM CUBE");
        int idleTime = 0;
        //EXECUTE STATE run the main behaviour of the state here

        while (currentState == STATES.IDLE)
        {
            yield return new WaitForSeconds(1);
            idleTime += 1;
            Debug.Log("I STAY HERE");

            yield return new WaitForSeconds(1);
            idleTime += 1;
            Debug.Log("I GUARD THIS AREA");

            
            
             
            yield return new WaitForSeconds(2);
            currentState = STATES.ROAMING;
            

        }

        //EXIT THE STATE run anything when the state is finished
        Debug.Log("SUSPICIOUS PERSONS SPOTTED");
    }

    IEnumerator CHASING()
    {
        //ENTER THE STATE run behaviours or state start here
        Debug.Log("MOVING TO CATCH PERSONS");

        //EXECUTE STATE run the main behaviour of the state here

        while (currentState == STATES.CHASING)
        {
            agent.SetDestination(target.position);

            if (agent.remainingDistance <= agent.stoppingDistance)
            {
                currentState = STATES.IDLE;
            }

            yield return new WaitForEndOfFrame();
                

        }

        //EXIT THE STATE run anything when the state is finished
        Debug.Log("PERSONS LOST");
        agent.ResetPath();
    }

    IEnumerator ROAMING()
    {
        //ENTER THE STATE run behaviours or state start here
        Debug.Log("SEARCHING AREA");
        agent.SetDestination(nodes[currentNode].position);

        //EXECUTE STATE run the main behaviour of the state here

        while (currentState == STATES.ROAMING)
        {
            //CheckSight(STATES.CHASING);

           
            if (!agent.pathPending && agent.remainingDistance <= agent.stoppingDistance)
            {
              currentNode = (currentNode +1) % nodes.Length;
              agent.SetDestination(nodes[currentNode].position);
            }

            

            yield return new WaitForEndOfFrame();


        }

        //EXIT THE STATE run anything when the state is finished
        Debug.Log("SEARCH OVER");
        agent.ResetPath();
    }




    #endregion 

    // Update is called once per frame
    void Update()
    {
        
    }

    void CheckSight(STATES stateToEnter)
    {
        if(Vector3.Distance(transform.position, target.position) > sightRadius)
        {
            currentState = stateToEnter;
        }
    }

    void onDrawGizmos()
    {
        Gizmos.DrawSphere(transform.position, sightRadius);
    }

    
}
