using System.Collections;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class KeyboardMovement : MonoBehaviour
{

    [Header("Functinal Options")]
    [SerializeField]
    bool isOnTheGround;
    [SerializeField]
    bool canSprint = true;
    [SerializeField]
    bool canJump = true;
    [SerializeField]
    bool canMove = true;
    [SerializeField] 
    bool canCrouch = true;

    bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    bool ShouldJump => Input.GetKeyDown(jumpKey) && isOnTheGround && canJump;
    bool ShouldCrouch => (Input.GetKeyDown(crouchKey) || Input.GetKeyUp(crouchKey)) && !duringCrouchAnimation && isOnTheGround && canCrouch;

    [Header("Instances")]
    [SerializeField]
    LayerMask groundMask; // Control what objects the Physics.CheckSphere should check for
    Transform groundCheck;
    CharacterController controller;
    Camera camera;

    [Header("Crouch Parameters")]
    [SerializeField] float crouchHeight = 0.5f;
    [SerializeField] float standingHeight = 2f;
    [SerializeField] float timeToCrouch = 0.25f;
    [SerializeField] Vector3 crouchingCenter = new (0, 0.5f, 0);
    [SerializeField] Vector3 standingCenter = new (0, 0, 0);
    bool IsCrouching;
    bool duringCrouchAnimation;

    [Header("Movement Parameters")]
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float sprintSpeed = 20f;
    [SerializeField]
    float crouchSpeed = 5f;

    [Header("Jump Parameters")]
    [SerializeField]
    float jumpHeight = 10f;
    [SerializeField]
    float groundDistance = 0.4f;

    [Header("Physics")]
    [SerializeField]
    Vector3 velocity;
    float acceleration;
    const float GRAVITY = 9.81f;

    [Header("Controls")]
    [SerializeField]
    KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField]
    KeyCode jumpKey = KeyCode.Space;
    [SerializeField]
    KeyCode crouchKey = KeyCode.LeftControl;


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        acceleration = -GRAVITY;
        groundCheck = transform.Find("GroundCheck");
        camera = GetComponentInChildren<Camera>();
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            CheckIsOnTheGround();
            MoveOnInput();
            HandleJump();
            HandleCrouch();
            ApplyGravity();
        }
    }

    private void MoveOnInput()
    {
        // Get vertical/horizontal keyboard input (can be 1- or 1)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Move the character based on the keyboard input
        Vector3 movementVector = transform.right * x + transform.forward * z;
        controller.Move((IsCrouching ? crouchSpeed : IsSprinting ? sprintSpeed : speed) * Time.deltaTime * movementVector.normalized);
    }

    private void HandleJump()
    {
        // Jumping
        if (ShouldJump)
        {
            // Calculate velocity needed to jump set height
            float v = Mathf.Sqrt(jumpHeight * GRAVITY * 2);
            velocity.y = v;
        }
    }

    private void HandleCrouch()
    {
        // Crouching
        if (ShouldCrouch)
        {
            StartCoroutine(CrouchStand());
        }
    }

    private void ApplyGravity()
    {
            // Applying gravity physics to the movement
            velocity.y += acceleration * Time.deltaTime; // v = a * t
            controller.Move(velocity * Time.deltaTime);
    }

    private void CheckIsOnTheGround()
    {
        // Check wether the character is touching the ground
        isOnTheGround = controller.isGrounded;
        // Custom ground checking
        //isOnTheGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset the y velocity (falling) when the playing is touching the ground
        if (isOnTheGround && velocity.y < 0)
        {
            velocity.y = -2f;
        }
    }


    private IEnumerator CrouchStand()
    {
        // Check for collison above the character while crouching
        // Break the Coroutine if the object is detected (character cannot stand up)
        if (IsCrouching && Physics.Raycast(camera.transform.position, Vector3.up, 1f))
            yield break;

        duringCrouchAnimation = true;

        // Set target values
        float timeElapsed = 0f;
        float targetHeight = IsCrouching ? standingHeight : crouchHeight;
        float currentHeight = controller.height;
        Vector3 targetCenter = IsCrouching ? standingCenter : crouchingCenter;
        Vector3 currentCenter = controller.center;

        // Crouching loop
        while (timeElapsed < timeToCrouch)
        {
            controller.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToCrouch);
            // By lerping the center of the controller we enable smooth camera movement when crouching
            controller.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToCrouch);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Sanity check
        controller.height = targetHeight;
        controller.center = targetCenter;

        IsCrouching = !IsCrouching;

        duringCrouchAnimation = false;
    }
}