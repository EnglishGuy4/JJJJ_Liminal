using System.Collections;
using UnityEngine;

public class PowerUpAllies : MonoBehaviour
{
    [Header("Ally Turret Group")]
    // assign the empty with 2 turrets in Inspector
    public GameObject allyTurretGroup; // Assign in Inspector (parent or both turrets)

    void Awake()
    {
        if (allyTurretGroup != null)
            allyTurretGroup.SetActive(false); // Set allyTurretGroup inactive on Awake
    }

    void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
            if (allyTurretGroup != null)
                allyTurretGroup.SetActive(true); // Activate allyTurretGroup on powerup trigger

            gameObject.SetActive(false); // Disable the powerup
    }
}
