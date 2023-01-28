using System;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class SwimmingMechanics : MonoBehaviour
{
    bool isUnderWater;
    bool wasUnderWater;

    [Header("Oxygen Parameters")]
    [SerializeField] float oxygenUseMultiplier = 5f;
    [SerializeField] float timeBetweenDamage = 1f;
    [SerializeField] int damageToInflict = 10;
    public float maxOxygen = 100f;
    public float currentOxygen;
    float timeElapsed = 0.0f;

    [Header("SFX")]
    [SerializeField] AudioSource drowningSound;
    [SerializeField] AudioClip submergingSound;
    [SerializeField] AudioSource defaultBreath;
    [SerializeField] AudioSource inhaleSound;
    [SerializeField] AudioSource splashSound;

    [Header("VFX")]
    [SerializeField]
    PostProcessProfile underwaterEffect;

    [Header("Swimming Parameters")]
    [SerializeField] float swimmingUpSpeed;
    [SerializeField] float sinkingSpeed;

    [Header("Other")]
    [SerializeField] Camera cameraObject;
    [SerializeField] GameObject surface;
  

    CustomCharacterController player;
    PostProcessProfile defaultProfile;
    GameObject airBubbles;
    Inventory inventory;
    GameObject divingHelmetOverlay;

    // Defaults
    float defaultMouseSensX;
    float defaultMouseSensY;

    // Start is called before the first frame update
    void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").transform.Find("Inventory").GetComponent<Inventory>();
        divingHelmetOverlay = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerCanvas").Find("Diving Helmet Overlay").gameObject;
        isUnderWater = false;
        wasUnderWater = false;
        player = GetComponent<CustomCharacterController>();
        RenderSettings.fogColor = new Color(0.2f, 0.4f, 0.8f, 0.5f);
        RenderSettings.fogDensity = 0.04f;
        defaultProfile = cameraObject.GetComponent<PostProcessVolume>().profile;
        airBubbles = GameObject.Find("Air Bubbles");
        airBubbles.SetActive(false);
        defaultMouseSensX = cameraObject.GetComponent<MouseRotation>().mouseSensivityX;
        defaultMouseSensY = cameraObject.GetComponent<MouseRotation>().mouseSensivityY;
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.fog = isUnderWater;
        if (isUnderWater)
        {
            player.velocity = new Vector3(0, -sinkingSpeed, 0);
            if (Input.GetKey(KeyCode.LeftShift))
            {
                player.velocity = new Vector3(0, swimmingUpSpeed, 0);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                player.velocity = new Vector3(0, -swimmingUpSpeed, 0);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                player.velocity = Vector3.zero;
            }
            if (Input.GetKeyUp(KeyCode.LeftControl))
            {
                player.velocity = Vector3.zero;
            }
        }
        if (isUnderWater && !wasUnderWater)
        {
            HandleUnderwaterEffects();
            splashSound.Play();
            if (inventory.hasScubaMask) divingHelmetOverlay.SetActive(true);
            wasUnderWater = true;
        }
        if (!isUnderWater && wasUnderWater)
        {
            DisableUnderwaterEffects();
            currentOxygen = maxOxygen;
            inhaleSound.Play();
            drowningSound.Stop();
            if (inventory.hasScubaMask) divingHelmetOverlay.SetActive(false);
            wasUnderWater = false;
        }
        if (isUnderWater && !inventory.hasScubaMask)
        {
            HandleOxygenLevel();
            if (currentOxygen <= 0)
            {
                timeElapsed += Time.deltaTime;
                if (timeElapsed >= timeBetweenDamage)
                {
                    HealthSystem.OnDrowning(damageToInflict);
                    timeElapsed = 0.0f;
                }
            }
        }
    }


    private void HandleOxygenLevel()
    {
        if (isUnderWater)
        {
            currentOxygen -= oxygenUseMultiplier * Time.deltaTime;

            if (currentOxygen < 0f)
            {
                currentOxygen = 0f;
            }

            if (currentOxygen <= 0f)
            {
                if (!drowningSound.isPlaying) drowningSound.Play();
            }
        }
    }
    private void HandleUnderwaterEffects()
    {
        player.useHeadbob = false;
        player.WillSlideOnSlopes = false;
        surface.SetActive(true);
        defaultBreath.Pause();
        player.currentGravity = 0;
        player.velocity = Vector3.zero;
        player.movementSpeedMultiplier /= 4;
        cameraObject.GetComponent<MouseRotation>().mouseSensivityX /= 6;
        cameraObject.GetComponent<MouseRotation>().mouseSensivityY /= 6;
        player.canJump = false;
        player.canCrouch = false;
        player.CanSprint = false;
        player.implyFallDamage = false;
        player.canUseFootsteps = false;
        cameraObject.GetComponent<PostProcessVolume>().profile = underwaterEffect;
        airBubbles.SetActive(true);
        AudioSource.PlayClipAtPoint(submergingSound, transform.position);
    }

    private void DisableUnderwaterEffects()
    {
        player.useHeadbob = true;
        player.WillSlideOnSlopes = true;
        surface.SetActive(false);
        defaultBreath.Play();
        player.currentGravity = CustomCharacterController.GRAVITY;
        player.velocity = Vector3.zero;
        player.movementSpeedMultiplier = 1;
        cameraObject.GetComponent<MouseRotation>().mouseSensivityX = defaultMouseSensX;
        cameraObject.GetComponent<MouseRotation>().mouseSensivityY = defaultMouseSensY;
        player.canJump = true;
        player.canCrouch = true;
        player.CanSprint = true;
        player.implyFallDamage = true;
        player.canUseFootsteps = true;
        cameraObject.GetComponent<PostProcessVolume>().profile = defaultProfile;
        airBubbles.SetActive(false);
        Destroy(GameObject.Find("One shot audio"));
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isUnderWater = true;
        }
        if (other.CompareTag("Smoother"))
        {
            player.implyFallDamage = false;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isUnderWater = false;
        }
        if (other.CompareTag("Smoother"))
        {
            player.implyFallDamage = true;
        }
    }
}
