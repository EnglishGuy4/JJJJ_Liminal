using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KillVolume : MonoBehaviour
{
    [SerializeField] private LayerMask projectileLayer;
    private void OnTriggerEnter(Collider other)
    {
        
        //Debug.Log("KillVolume triggered by: " + other.name + " layer: " + LayerMask.LayerToName(other.gameObject.layer));
        if ((projectileLayer.value & (1 << other.gameObject.layer)) == 0)
            return;

        
        var resettable = other.GetComponentInParent<IResettableShot>();
        if (resettable != null)
        {
            resettable.ResetShot();
            //Debug.Log("KillVolume reset a shot: " + other.gameObject.name);
        }
    }
}
