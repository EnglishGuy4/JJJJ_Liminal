using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;
using UnityEngine.Analytics;

public class Cannon : MonoBehaviour
{
    // Prefabs
    [SerializeField]
    private GameObject cannonBall;
    [SerializeField]
    private GameObject barrelEnd;
    public GameObject hand;
    public GameObject handleHand;
    public GameObject primaryHand;
    public GameObject cannonPos;
    private CannonBall cb;

    // Transform information for hand movement
    [SerializeField]
    private Transform primaryHandAnchor;
    [SerializeField]
    private Transform cannon;
    [SerializeField]
    private Transform cBase;

    // Audio & Particle Effects
    private new ParticleSystem particleSystem;
    private new AudioSource audio;

    // Bools for grabbing the handle on the cannon
    [HideInInspector]
    public bool grabHandle;
    [HideInInspector]
    public bool grabHandleComplete;
    [HideInInspector]
    public bool initialGrab;
    private bool cannonReload; // no longer used to block firing

    // Sensitivity for mouse look
    public float mouseSensitivity = 50f;
    private float pitch = 0f; // up and down
    private float yaw = 0f;   // left and right

    void Start()
    {
        initialGrab = false;
        handleHand.GetComponent<MeshRenderer>().enabled = false;
        grabHandleComplete = true;
        grabHandle = false;
        particleSystem = GetComponentInChildren<ParticleSystem>();
        audio = GetComponent<AudioSource>();
    }

    void Update()
    {
        IVRInputDevice primaryInput = VRDevice.Device != null ? VRDevice.Device.PrimaryInputDevice : null;
        IVRInputDevice secondaryInput = VRDevice.Device != null ? VRDevice.Device.SecondaryInputDevice : null;

        if (!grabHandleComplete)
        {
            handleHand.GetComponent<MeshRenderer>().enabled = true;
            hand.GetComponent<MeshRenderer>().enabled = false;
            cannonReload = false;
            grabHandleComplete = true;
            grabHandle = true;
        }

        if (grabHandle)
        {
            // ---------- VR Controls ----------
            if (!Application.isEditor && VRDevice.Device != null)
            {
                Quaternion rotation = Quaternion.LookRotation(cannonPos.transform.position - (primaryHand.transform.position - cannonPos.transform.position) * 1000);
                float handX = Mathf.Clamp(rotation.x, -0.4f, 0.2f);
                float handY = Mathf.Clamp(rotation.y, -0.4f, 0.4f);

                if ((primaryInput != null && primaryInput.GetButton(VRButton.Trigger)) || Input.GetKey(KeyCode.Q))
                {
                    cBase.transform.rotation = Quaternion.Slerp(cBase.transform.rotation, new Quaternion(0, handY, 0, cBase.transform.rotation.w), 0.25f * Time.deltaTime);
                    cannon.transform.localRotation = Quaternion.Slerp(cannon.transform.localRotation, new Quaternion(handX, 0, 0, cannon.transform.localRotation.w), 0.25f * Time.deltaTime);
                }
                else
                {
                    cBase.transform.rotation = Quaternion.Slerp(cBase.transform.rotation, new Quaternion(0, handY, 0, cBase.transform.rotation.w), 4 * Time.deltaTime);
                    cannon.transform.localRotation = Quaternion.Slerp(cannon.transform.localRotation, new Quaternion(handX, 0, 0, cannon.transform.localRotation.w), 4 * Time.deltaTime);
                }
            }
            else
            {
                // ---------- PC / Editor Mouse Controls ----------
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, -30f, 30f); // restrict up/down

                // Apply rotations
                cBase.localRotation = Quaternion.Euler(0f, yaw, 0f);   // left/right on base
                cannon.localRotation = Quaternion.Euler(pitch, 0f, 0f); // up/down on barrel
            }

            // ---------- Fire ----------
            bool firePressed = false;
            if (Application.isEditor)
            {
                firePressed = Input.GetMouseButtonDown(0);
            }
            else
            {
                if (primaryInput != null && primaryInput.GetButtonDown(VRButton.Trigger))
                    firePressed = true;
                if (secondaryInput != null && secondaryInput.GetButtonDown(VRButton.Trigger))
                    firePressed = true;
            }

            if (firePressed)
            {
                FireCannon();
            }
        }

        // ---------- Release Handle ----------
        if (Input.GetKeyDown(KeyCode.A) || (primaryInput != null && primaryInput.GetButtonDown(VRButton.Three)))
        {
            grabHandle = false;
            grabHandleComplete = true;
            handleHand.GetComponent<MeshRenderer>().enabled = false;
            hand.GetComponent<MeshRenderer>().enabled = true;
            hand.transform.position = primaryHandAnchor.position;
            hand.transform.rotation = primaryHandAnchor.rotation;
            initialGrab = false;
        }
    }

    private void FireCannon()
    {
        GameObject returnedGameObject = PoolManager.current.GetPooledObject(cannonBall.name);
        if (returnedGameObject == null) return;
        cb = returnedGameObject.GetComponent<CannonBall>();
        cb.rb.transform.position = barrelEnd.transform.position;
        cb.rb.transform.rotation = barrelEnd.transform.rotation;
        returnedGameObject.SetActive(true);
        cb.rb.isKinematic = false;
        cb.trailRenderer.Clear();
        cb.trailRenderer.enabled = true;
        cb.rb.AddForce(cb.rb.transform.forward * cb.force, ForceMode.Impulse);
        particleSystem.Play();
        cb.smokeEffect.Play();
        audio.Play();
    }
}
