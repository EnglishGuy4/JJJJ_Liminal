using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DroneSlotTracker : MonoBehaviour
{
    public Transform assignedSlot;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AssignSlot(Transform slot)
    {
        assignedSlot = slot;
    }

    void OnDisable()
    {
        if (assignedSlot != null && DroneSpawnManager.Instance != null)
        {
            DroneSpawnManager.Instance.ReleaseApproachSlot(assignedSlot);
            assignedSlot = null;
        }
    }
}
