using System.Collections;
using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class EnemyBullets : MonoBehaviour
{
    public float despawnTime = 20f;
    float time = 0f;

    // Update is called once per frame
    void Update()
    {
        // If it's time to go, then go we shall.
        time += Time.deltaTime;
        if (time > despawnTime)
        {
            Destroy(gameObject);
        }
    }
}
