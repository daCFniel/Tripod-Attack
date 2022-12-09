using UnityEngine;
using UnityEngine.Rendering.PostProcessing;

/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class SwimmingMechanics : MonoBehaviour
{
    bool isUnderWater;
    bool wasUnderWater;

    [SerializeField]
    float swimmingUpSpeed;
    [SerializeField]
    PostProcessProfile underwaterEffect;
    [SerializeField]
    Camera cameraObject;
    [SerializeField]
    private AudioClip submergingSound;

    CustomCharacterController player;
    StaminaSystem stamina;
    PostProcessProfile defaultProfile;
    GameObject airBubbles;

    // Defaults
    float defaultMouseSensX;
    float defaultMouseSensY;

    // Start is called before the first frame update
    void Start()
    {
        isUnderWater = false;
        wasUnderWater = false;
        player = GetComponent<CustomCharacterController>();
        stamina = GetComponent<StaminaSystem>();
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

        }
        if (isUnderWater && !wasUnderWater)
        {
            player.velocity = Vector3.zero;
            player.movementSpeedMultiplier /= 8;
            cameraObject.GetComponent<MouseRotation>().mouseSensivityX /= 6;
            cameraObject.GetComponent<MouseRotation>().mouseSensivityY /= 6;
            player.canJump = false;
            player.canCrouch = false;
            player.CanSprint = false;
            player.implyFallDamage = false;
            player.canUseFootsteps = false;
            stamina.currentStamina = StaminaSystem.maxStamina;
            cameraObject.GetComponent<PostProcessVolume>().profile = underwaterEffect;
            airBubbles.SetActive(true);
            AudioSource.PlayClipAtPoint(submergingSound, transform.position);
            wasUnderWater = true;
        }
        if (!isUnderWater && wasUnderWater)
        {
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
            wasUnderWater = false;
        }
    }

    private void OnTriggerStay(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isUnderWater = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Water"))
        {
            isUnderWater = false;
        }
    }
}
