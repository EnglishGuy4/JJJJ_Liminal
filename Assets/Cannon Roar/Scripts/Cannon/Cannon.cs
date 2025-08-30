using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;
using UnityEngine.Analytics;

public class Cannon : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject cannonBall;
    [SerializeField] private GameObject barrelEnd;
    public GameObject hand;
    public GameObject handleHand;
    public GameObject primaryHand;
    public GameObject cannonPos;
    private CannonBall cb;

    [Header("Transforms")]
    [SerializeField] private Transform primaryHandAnchor;
    [SerializeField] private Transform cannon;
    [SerializeField] private Transform cBase;

    [Header("Effects")]
    private new ParticleSystem particleSystem;
    private new AudioSource audio;

    [Header("Power Up Settings")]
    public bool isPoweredUp = false;
    public float powerUpDuration = 8f;
    private Coroutine powerUpRoutine;
    private Coroutine autoFireRoutine;
    public float autoFireRate = 0.25f; // Time between automatic shots

    // Handle interaction
    [HideInInspector] public bool grabHandle;
    [HideInInspector] public bool grabHandleComplete;
    [HideInInspector] public bool initialGrab;
    private bool cannonReload;

    // Mouse control
    public float mouseSensitivity = 50f;
    private float pitch = 0f;
    private float yaw = 0f;

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

        // ---------- PC Editor Grab ----------
        if (Application.isEditor && Input.GetKeyDown(KeyCode.E))
        {
            grabHandle = true;
            grabHandleComplete = true;
            initialGrab = true;
            handleHand.GetComponent<MeshRenderer>().enabled = true;
            hand.GetComponent<MeshRenderer>().enabled = false;
        }

        if (!grabHandleComplete && grabHandle)
        {
            handleHand.GetComponent<MeshRenderer>().enabled = true;
            hand.GetComponent<MeshRenderer>().enabled = false;
            cannonReload = false;
            grabHandleComplete = true;
        }

        if (grabHandle)
        {
            // VR Controls
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
                // Mouse Controls (Editor)
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, -30f, 30f);

                cBase.localRotation = Quaternion.Euler(0f, yaw, 0f);
                cannon.localRotation = Quaternion.Euler(pitch, 0f, 0f);
            }

            // Fire
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

            if (firePressed && !isPoweredUp)
                FireCannon();
        }

        // Release handle
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
        if (isPoweredUp)
        {
            // Fire 9 in a spread
            for (int i = 0; i < 9; i++)
            {
                float spreadX = Random.Range(-5f, 5f); // degrees
                float spreadY = Random.Range(-5f, 5f);

                Quaternion spreadRotation = barrelEnd.transform.rotation * Quaternion.Euler(spreadX, spreadY, 0);
                SpawnCannonball(barrelEnd.transform.position, spreadRotation);
            }
        }
        else
        {
            // Normal single shot
            SpawnCannonball(barrelEnd.transform.position, barrelEnd.transform.rotation);
        }

        particleSystem.Play();
        audio.Play();
    }

    private void SpawnCannonball(Vector3 pos, Quaternion rot)
    {
        GameObject returnedGameObject = PoolManager.current.GetPooledObject(cannonBall.name);
        if (returnedGameObject == null) return;

        cb = returnedGameObject.GetComponent<CannonBall>();
        cb.firedFrom = this;
        cb.rb.transform.position = pos;
        cb.rb.transform.rotation = rot;
        returnedGameObject.SetActive(true);
        cb.rb.isKinematic = false;
        cb.trailRenderer.Clear();
        cb.trailRenderer.enabled = true;
        cb.rb.AddForce(cb.rb.transform.forward * cb.force, ForceMode.Impulse);
        cb.smokeEffect.Play();
    }

    // Called by PowerUp hit
    public void ActivatePowerUp()
    {
        if (isPoweredUp) return; // ignore if already active
        isPoweredUp = true;

        // Stop previous routines if any
        if (powerUpRoutine != null)
            StopCoroutine(powerUpRoutine);
        if (autoFireRoutine != null)
            StopCoroutine(autoFireRoutine);

        powerUpRoutine = StartCoroutine(PowerUpTimer());
        autoFireRoutine = StartCoroutine(AutoFireCannon());
    }

    private IEnumerator AutoFireCannon()
    {
        while (isPoweredUp && grabHandle)
        {
            FireCannon();
            yield return new WaitForSeconds(autoFireRate);
        }
    }

    private IEnumerator PowerUpTimer()
    {
        yield return new WaitForSeconds(powerUpDuration);
        isPoweredUp = false;

        if (autoFireRoutine != null)
            StopCoroutine(autoFireRoutine);
    }
}
