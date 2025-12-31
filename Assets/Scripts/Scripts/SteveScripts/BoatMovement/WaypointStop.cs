using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WaypointStop : MonoBehaviour
{
  
    [Range(0f, 2f)]
    [SerializeField]
    private float waypointSize = 2f;
    public bool lastWaypoint;

    private void OnDrawGizmos()
    {
        foreach (Transform t in transform)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(t.position, waypointSize);
        }

        Gizmos.color = Color.red;
        for (int i = 0; i < transform.childCount - 1; i++)
        {
            Gizmos.DrawLine(transform.GetChild(i).position, transform.GetChild(i + 1).position);
        }

    }

    public Transform NextWaypoint(Transform currentWaypoint)
    {
        if (currentWaypoint == null)
        {
            return transform.GetChild(0);
        }

        if (currentWaypoint.GetSiblingIndex() < transform.childCount - 1)
        {
            return transform.GetChild(currentWaypoint.GetSiblingIndex() + 1);
        }

        if (currentWaypoint.GetSiblingIndex() >= transform.childCount - 1)
        {
            lastWaypoint = true;
            return transform.GetChild(transform.childCount - 1);
        }
        else
        {
            return null;
        }
        
    }

    //Sets currentWaypoint in BoatMovement script to the first waypoint
}


