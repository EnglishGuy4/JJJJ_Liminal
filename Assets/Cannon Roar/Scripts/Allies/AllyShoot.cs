using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class AllyShoot : MonoBehaviour
{
    [SerializeField] private GameObject cannonBall;
    [SerializeField] private GameObject[] barrellEnd;
    private float timer;
    private float timeBetweenShots = 0.5f;
    private CannonBall cb;
    private int barrelPicker;
    private int lastNumber;
    private int currentNumber;

    // Update is called once per frame
    void Update()
    {
        RaycastHit hit;
        timer += Time.deltaTime;

        if (timer > timeBetweenShots)
        {
            barrelPicker = GetRandom(0, barrellEnd.Length);

            if(currentNumber != barrelPicker)
            {
                currentNumber = barrelPicker;

                if (Physics.Raycast(barrellEnd[barrelPicker].transform.position, barrellEnd[barrelPicker].transform.forward, out hit, 500f))
                {
                    if (hit.collider.tag != "Enemy")
                    {
                        return;
                    }
                    else
                    {
                        GameObject returnedGameObject = PoolManager.current.GetPooledObject(cannonBall.name);
                        if (returnedGameObject == null) return;
                        cb = returnedGameObject.GetComponent<CannonBall>();
                        cb.rb.transform.position = barrellEnd[barrelPicker].transform.position;
                        cb.rb.transform.rotation = barrellEnd[barrelPicker].transform.rotation;
                        returnedGameObject.SetActive(true);
                        cb.rb.isKinematic = false;
                        cb.trailRenderer.Clear();
                        cb.trailRenderer.enabled = true;
                        cb.rb.AddForce(cb.rb.transform.up * 300, ForceMode.Impulse);
                        cb.rb.AddForce(cb.rb.transform.forward * cb.force, ForceMode.Impulse);
                        timer = 0;
                    }
                }
            } 
        }
    }

    private int GetRandom(int min, int max)
    {
        int rand = Random.Range(min, max);
        while (rand == lastNumber)
            rand = Random.Range(min, max);
        lastNumber = rand;
        return rand;
    }
}
