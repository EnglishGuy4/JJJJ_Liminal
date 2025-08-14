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

            if (primaryHand.transform.position.z > handleHand.transform.position.z && cannonPos.transform.localPosition.z <= 0.9814323f)
            {
                cannonPos.transform.position += cannonPos.transform.forward * Time.deltaTime;
            }
            else if (primaryHand.transform.position.z < handleHand.transform.position.z - 0.075f && cannonPos.transform.localPosition.z >= 0.8814323f)
            {
                cannonPos.transform.position -= cannonPos.transform.forward * Time.deltaTime;
            }

            // Firing logic - unlimited in both VR & Editor
            bool firePressed = false;

            if (Application.isEditor)
            {
                // PC / Editor Mode
                firePressed = Input.GetMouseButtonDown(0);
            }
            else
            {
                // VR Mode - either controller's trigger
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
        // cannonReload removed from firing restriction
    }
}
