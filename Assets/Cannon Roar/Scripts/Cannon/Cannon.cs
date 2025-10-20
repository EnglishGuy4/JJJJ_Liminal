using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Liminal.SDK.VR;
using Liminal.SDK.VR.Input;

public class Cannon : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private GameObject cannonBall;
    [SerializeField] private GameObject barrelEnd;
    public GameObject hand;
    public GameObject handleHand;
    public GameObject primaryHand;
    public GameObject secondaryHand; // 🔹 Added reference for secondary hand
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
    public float autoFireRate = 0.25f;

    [Header("Spawner Manager")]
    public SpawnerManager spawnerManager; // 🔹 Reference to SpawnerManager

    // Handle interaction
    [HideInInspector] public bool grabHandle;
    [HideInInspector] public bool grabHandleComplete;
    [HideInInspector] public bool initialGrab;

    // Mouse control
    public float mouseSensitivity = 50f;
    private float pitch = 0f;
    private float yaw = 0f;

    // Recoil Settings
    [Header("Recoil Settings")]
    public float recoilAngle = 5f;
    public float recoilRecovery = 10f;
    private float currentRecoil = 0f;

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
            if (secondaryHand != null) secondaryHand.SetActive(false);

            // 🔹 Tell SpawnerManager to begin waves
            if (spawnerManager != null)
                spawnerManager.BeginSpawning();
        }

        // ---------- VR Grab Handle ----------
        if (!Application.isEditor && VRDevice.Device != null)
        {
            bool leftGrab = OVRInput.Get(OVRInput.Button.PrimaryHandTrigger);
            bool rightGrab = OVRInput.Get(OVRInput.Button.SecondaryHandTrigger);

            if (leftGrab && rightGrab)
            {
                if (!grabHandle)
                {
                    grabHandle = true;
                    grabHandleComplete = true;
                    initialGrab = true;
                    handleHand.GetComponent<MeshRenderer>().enabled = true;
                    hand.GetComponent<MeshRenderer>().enabled = false;
                    if (secondaryHand != null) secondaryHand.SetActive(false);

                    // 🔹 Trigger waves when cannon is first grabbed
                    if (spawnerManager != null)
                        spawnerManager.BeginSpawning();
                }
            }
            else
            {
                if (grabHandle)
                {
                    grabHandle = false;
                    grabHandleComplete = false;
                    handleHand.GetComponent<MeshRenderer>().enabled = false;
                    hand.GetComponent<MeshRenderer>().enabled = true;
                    hand.transform.position = primaryHandAnchor.position;
                    hand.transform.rotation = primaryHandAnchor.rotation;
                    initialGrab = false;
                    if (secondaryHand != null) secondaryHand.SetActive(true);
                }
            }
        }

        if (grabHandle)
        {
            // VR Controls
            if (!Application.isEditor && VRDevice.Device != null)
            {
                Quaternion rotation = Quaternion.LookRotation(
                    cannonPos.transform.position - (primaryHand.transform.position - cannonPos.transform.position) * 1000
                );

                float handX = Mathf.Clamp(rotation.x, -0.4f, 0.2f);
                float handY = Mathf.Clamp(rotation.y, -0.4f, 0.4f);

                float rotationSpeed = 15f;
                cBase.transform.rotation = Quaternion.Lerp(
                    cBase.transform.rotation,
                    new Quaternion(0, handY, 0, cBase.transform.rotation.w),
                    rotationSpeed * Time.deltaTime
                );

                Quaternion baseCannonRotation = Quaternion.Lerp(
                    cannon.transform.localRotation,
                    new Quaternion(handX, 0, 0, cannon.transform.localRotation.w),
                    rotationSpeed * Time.deltaTime
                );

                Quaternion recoilRotation = Quaternion.Euler(-currentRecoil, 0, 0);
                cannon.transform.localRotation = baseCannonRotation * recoilRotation;

                currentRecoil = Mathf.Lerp(currentRecoil, 0f, recoilRecovery * Time.deltaTime);
            }
            else
            {
                // Mouse Controls
                float mouseX = Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
                float mouseY = Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;

                yaw += mouseX;
                pitch -= mouseY;
                pitch = Mathf.Clamp(pitch, -30f, 30f);

                cBase.localRotation = Quaternion.Euler(0f, yaw, 0f);

                Quaternion baseCannonRotation = Quaternion.Euler(pitch, 0f, 0f);
                Quaternion recoilRotation = Quaternion.Euler(-currentRecoil, 0, 0);
                cannon.localRotation = baseCannonRotation * recoilRotation;

                currentRecoil = Mathf.Lerp(currentRecoil, 0f, recoilRecovery * Time.deltaTime);
            }

            // Fire
            bool firePressed = false;
            if (Application.isEditor)
                firePressed = Input.GetMouseButtonDown(0);
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
    }

    private void FireCannon()
    {
        if (isPoweredUp)
        {
            for (int i = 0; i < 9; i++)
            {
                float spreadX = Random.Range(-5f, 5f);
                float spreadY = Random.Range(-5f, 5f);

                Quaternion spreadRotation = barrelEnd.transform.rotation * Quaternion.Euler(spreadX, spreadY, 0);
                SpawnCannonball(barrelEnd.transform.position, spreadRotation);
            }
        }
        else
        {
            SpawnCannonball(barrelEnd.transform.position, barrelEnd.transform.rotation);
        }


        // 🎇 Randomize muzzle flash Z rotation
        if (particleSystem != null)
        {
            var main = particleSystem.main;
            main.startRotation = Random.Range(0f, Mathf.PI * 2f); // radians
            particleSystem.Play();
        }

        
        //particleSystem.Play();
        audio.Play();

        currentRecoil += recoilAngle;
        currentRecoil = Mathf.Clamp(currentRecoil, 0, recoilAngle * 2f);

        OVRInput.SetControllerVibration(1f, 1f, OVRInput.Controller.RTouch | OVRInput.Controller.LTouch);
        StartCoroutine(StopHaptics(0.2f));
    }

    private IEnumerator StopHaptics(float duration)
    {
        yield return new WaitForSeconds(duration);
        OVRInput.SetControllerVibration(0, 0, OVRInput.Controller.RTouch | OVRInput.Controller.LTouch);
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
        //cb.trailRenderer.Clear();
        //cb.trailRenderer.enabled = true;
        cb.rb.AddForce(cb.rb.transform.forward * cb.force, ForceMode.Impulse);
        
    }

    public void ActivatePowerUp()
    {
        if (isPoweredUp) return;
        isPoweredUp = true;

        if (powerUpRoutine != null)
            StopCoroutine(powerUpRoutine);
        if (autoFireRoutine != null)
            StopCoroutine(autoFireRoutine);

        powerUpRoutine = StartCoroutine(PowerUpTimer());
        autoFireRoutine = StartCoroutine(AutoFireCannon());
    }

    private IEnumerator AutoFireCannon()
    {
        while (isPoweredUp)
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
