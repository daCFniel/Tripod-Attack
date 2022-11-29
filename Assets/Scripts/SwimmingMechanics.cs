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

    FirstPersonAIO player;
    Rigidbody playerRB;
    PostProcessProfile defaultProfile;
    GameObject airBubbles;

    // Start is called before the first frame update
    void Start()
    {
        isUnderWater = false;
        wasUnderWater = false;
        player = transform.root.GetComponent<FirstPersonAIO>();
        playerRB = transform.root.GetComponent<Rigidbody>();
        RenderSettings.fogColor = new Color(0.2f, 0.4f, 0.8f, 0.5f);
        RenderSettings.fogDensity = 0.04f;
        defaultProfile = cameraObject.GetComponent<PostProcessVolume>().profile;
        airBubbles = GameObject.Find("Air Bubbles");
        airBubbles.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        RenderSettings.fog = isUnderWater;
        if (isUnderWater)
        {
            if (Input.GetKey(KeyCode.LeftShift))
            {
                playerRB.velocity = new Vector3(0, swimmingUpSpeed, 0);
            }
            if (Input.GetKey(KeyCode.LeftControl))
            {
                playerRB.velocity = new Vector3(0, -swimmingUpSpeed, 0);
            }
            if (Input.GetKeyUp(KeyCode.LeftShift))
            {
                playerRB.velocity = Vector3.zero;
            }

        }
        if (isUnderWater && !wasUnderWater)
        {
            player.walkSpeed = 2;
            player.sprintSpeed = 0;
            player.mouseSensitivity = 5;
            player.advanced.gravityMultiplier = 0.7f;
            player.jumpPower = 1;
            player.canJump = false;
            player._crouchModifiers.useCrouch = false;
            cameraObject.GetComponent<PostProcessVolume>().profile = underwaterEffect;
            airBubbles.SetActive(true);
            AudioSource.PlayClipAtPoint(submergingSound, transform.position);
            wasUnderWater = true;
        }
        if (!isUnderWater && wasUnderWater)
        {
            player.walkSpeed = 4;
            player.sprintSpeed = 8;
            player.mouseSensitivity = 10;
            player.advanced.gravityMultiplier = 1.0f;
            player.jumpPower = 5;
            player.canJump = true;
            player._crouchModifiers.useCrouch = true;
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
