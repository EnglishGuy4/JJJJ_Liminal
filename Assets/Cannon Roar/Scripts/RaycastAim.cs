using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RaycastAim : MonoBehaviour
{
    LineRenderer line;

    [SerializeField] Transform target;
    [SerializeField] LayerMask layerMask;

    void Start()
    {
        line = GetComponent<LineRenderer>();
    }

    void Update()
    {
        RaycastHit hit;

        if (Physics.Raycast(transform.position, transform.TransformDirection(Vector3.down), out hit, Mathf.Infinity, ~layerMask))
        {
            target.position = hit.point;
        }
        line.SetPosition(0, transform.position);
        line.SetPosition(1, target.position);
    }

}
