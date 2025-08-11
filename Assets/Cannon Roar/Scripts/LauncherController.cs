using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LauncherController : MonoBehaviour
{
    [SerializeField] float speed = 100;
    [SerializeField] GameObject bomb, block;
    [SerializeField] Transform spawnPoint;

    // Update is called once per frame
    void Update()
    {
        float xmove = Input.GetAxis("Mouse X");
        float zmove = Input.GetAxis("Mouse Y");
        Vector3 _movement = new Vector3(xmove, 0, zmove);

        transform.Translate(_movement * Time.deltaTime * speed);

        if (Input.GetMouseButtonDown(0))
        {
            Instantiate(bomb, spawnPoint.position, spawnPoint.rotation);
        }
        if (Input.GetMouseButtonDown(1))
        {
            Instantiate(block, spawnPoint.position, spawnPoint.rotation);
        }
    }


}
