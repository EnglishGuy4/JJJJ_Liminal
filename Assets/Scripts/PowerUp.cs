using System.Collections;
using UnityEngine;

public class PowerUp : MonoBehaviour
{
    [Header("Respawn Settings")]
    public float respawnTime = 10f; // time before it reappears

    [Header("Child Object to Toggle (optional)")]
    public GameObject childObject; // optional, will auto-assign first child if null

    private Collider col;

    void Awake()
    {
        col = GetComponent<Collider>();

        // Auto-assign first child if not set
        if (childObject == null && transform.childCount > 0)
        {
            childObject = transform.GetChild(0).gameObject;
        }

        if (childObject == null)
        {
            Debug.LogWarning("PowerUp has no child object to toggle!");
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        
        if (collision.gameObject.CompareTag("PlayerShot"))
        {
            // Activate cannon power up
            collision.gameObject.GetComponent<CannonBall>().firedFrom.ActivatePowerUp();

            // Start respawn coroutine to hide & re-enable later
            StartCoroutine(RespawnRoutine());
        }
    }

    private IEnumerator RespawnRoutine()
    {
        // Hide child object & disable collider immediately
        if (childObject != null) childObject.SetActive(false);
        if (col != null) col.enabled = false;

        // Wait for respawn time
        yield return new WaitForSeconds(respawnTime);

        // Reactivate child object & collider
        if (childObject != null) childObject.SetActive(true);
        if (col != null) col.enabled = true;
    }
}
