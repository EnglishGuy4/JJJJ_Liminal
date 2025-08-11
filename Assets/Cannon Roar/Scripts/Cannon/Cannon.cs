using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;
using UnityEngine.Analytics;

public class Cannon : MonoBehaviour
{
    //Prefabs
    [SerializeField]
    private GameObject cannonBall; // prefab of the cannonballs
    [SerializeField]
    private GameObject barrelEnd; // the empty game object placed at the end of the barrel of the cannon that the cannonball prefab will instantiate from
    public GameObject hand; // The hand model
    public GameObject handleHand; // A prefab of the handle with the hand placed in the center, using this to remove the jittering of the hand that would happen when the hand was moved directly to the handle position. by Disabling the mesh renderer of the hand when and enabling this prefab, the hand movement looks much smoother
    public GameObject primaryHand;
    public GameObject cannonPos;
    private CannonBall cb;

    //Transform information for hand movement
    [SerializeField]
    private Transform primaryHandAnchor; // The Transform position of the Primary Hand Anchor on the VR Avatar, used to return the hand to the original position when the player lets go of the cannon
    [SerializeField]
    private Transform cannon; // The Transform position of the empty game object named Pivot in the heirarchy located under the cannon game object, this is for rotating the cannon barrel on the X Axis
    [SerializeField]
    private Transform cBase; // The transform position of the base of the cannon that will rotate on the Y Axis

    //Audio & Particle Effects
    private new ParticleSystem particleSystem; // Fire or smoke, whatever looks cool
    private new AudioSource audio; // bang!

    //Bools for grabbing the handle on the cannon
    [HideInInspector]
    public bool grabHandle; // When this bool is active the cannon will move with the hand
    [HideInInspector]
    public bool grabHandleComplete; // Sets up the hand position when grabbing the handle
    [HideInInspector]
    public bool initialGrab; // This is for the script on the handle not triggering every time the hand moves in and out of the collider
    private bool cannonReload;

    // Start is called before the first frame update
    void Start()
    {
        initialGrab = false;
        handleHand.GetComponent<MeshRenderer>().enabled = false;
        grabHandleComplete = true;
        grabHandle = false;
        particleSystem = GetComponentInChildren<ParticleSystem>();
        audio = GetComponent<AudioSource>();
    }

    // Update is called once per frame
    void Update()
    {
        IVRInputDevice primaryInput = VRDevice.Device.PrimaryInputDevice;
        IVRInputDevice secondaryInput = VRDevice.Device.SecondaryInputDevice;
        
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

            if (primaryInput.GetButton(VRButton.Trigger) || Input.GetKey(KeyCode.Q))
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

            if (cannonPos.transform.localPosition.z >= 0.9314323f)
                cannonReload = false;

            if (cannonPos.transform.localPosition.z <= 0.8814323f && !cannonReload)
            {
                FireCannon();
            }
        }

        if (Input.GetKeyDown(KeyCode.A) || primaryInput.GetButtonDown(VRButton.Three))
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
        cannonReload = true;
    }
}
