using System.Collections;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class CustomCharacterController : MonoBehaviour
{

    [Header("Functional Options")]
    [SerializeField] bool isOnTheGround;
    [SerializeField] bool canWalk = true;
    [SerializeField] bool useHeadbob = true;
    [SerializeField] bool WillSlideOnSlopes = true;
    [SerializeField] bool canUseFootsteps = true;
    public bool CanSprint = true;
    public bool CanMove = true;
    public bool canJump = true;
    public bool canCrouch = true;

    // Lambda fields
    bool IsWalking => canWalk && Input.GetKey(walkKey);
    bool ShouldJump => Input.GetKeyDown(jumpKey) && isOnTheGround && canJump;
    float MoveSpeed => IsCrouching ? crouchSpeed : IsWalking ? walkSpeed : IsSprinting ? currentSprintSpeed : runSpeed * movementSpeedMultiplier;
    bool ShouldCrouch => (Input.GetKeyDown(crouchKey) || Input.GetKeyUp(crouchKey)) && !duringCrouchAnimation && isOnTheGround && canCrouch;
    float BobSpeed => IsCrouching ? crouchBobSpeed : IsWalking ? walkBobSpeed : IsSprinting ? currentSprintBobSpeed : runBobSpeed;
    float BobAmount => IsCrouching ? crouchBobAmount : IsWalking ? walkBobAmount : IsSprinting ? sprintBobAmount : runBobAmount;
    public bool IsSprinting => CanSprint && Input.GetKey(sprintKey);

    [Header("Instances")]
    [SerializeField] LayerMask groundMask; // Control what objects the Physics.CheckSphere should check for
    Transform groundCheck;
    CharacterController controller;
    Camera cameraComponent;

    [Header("Movement Parameters")]
    [SerializeField] float crouchSpeed = 4f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;
    [SerializeField] float currentSprintSpeed = 20f;
    [SerializeField] float slopeSpeed = 8f;
    [SerializeField] Vector3 movementDirection;
    public Vector2 movementInput;
    public float movementSpeedMultiplier = 1;

    [Header("Jump Parameters")]
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] private AudioSource jumpSound;

    [Header("Crouch Parameters")]
    [SerializeField] float crouchHeight = 0.5f;
    [SerializeField] float standingHeight = 2f;
    [SerializeField] float timeToCrouch = 0.25f;
    [SerializeField] Vector3 crouchingCenter = new(0, 0.5f, 0);
    [SerializeField] Vector3 standingCenter = new(0, 0, 0);
    bool IsCrouching;
    bool duringCrouchAnimation;

    [Header("Headbob Parameters")]
    [SerializeField] float crouchBobSpeed = 5f;
    [SerializeField] float crouchBobAmount = 0.025f;
    [SerializeField] float walkBobSpeed = 10f;
    [SerializeField] float walkBobAmount = 0.05f;
    [SerializeField] float runBobSpeed = 15f;
    [SerializeField] float runBobAmount = 0.05f;
    [SerializeField] float sprintBobSpeed = 30f;
    [SerializeField] float currentSprintBobSpeed = 30f;
    [SerializeField] float sprintBobAmount = 0.1f;
    [SerializeField] float timeToReturnCamera = 0.1f;
    float defaultYPos = 0; // camera position
    float timer; // used to determine where the camera should be at given moment
    bool duringHeightLevelAnimation;

    [Header("Footsteps SFX Parameters")]
    [SerializeField] private float baseStepSpeed = 0.5f;
    [SerializeField] private float crouchStepMultiplier = 1.5f;
    [SerializeField] private float sprintStepMultiplier = 0.6f;
    [SerializeField] private float walkStepMultiplier = 1.2f;
    [SerializeField] private AudioSource footstepAudioSource = default;
    [SerializeField] private AudioClip[] groundClips = default;
    [SerializeField] private AudioClip[] grassClips = default;
    private float footstepSoundTimer = 0f;
    private float GetCurrentOffset => IsCrouching ? baseStepSpeed * crouchStepMultiplier : IsWalking ? baseStepSpeed * walkStepMultiplier : IsSprinting ? baseStepSpeed * sprintStepMultiplier : baseStepSpeed;

    [Header("Physics")]
    [SerializeField] Vector3 velocity;
    [SerializeField] float fallWillHurtVelocity = 5f;
    [SerializeField] float fallTime;
    float zeroVelocity = -2f;
    bool falling;
    float acceleration;
    const float GRAVITY = 9.81f;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.C;
    [SerializeField] KeyCode walkKey = KeyCode.LeftControl;

    // SLIDING
    Vector3 hitPointNormal; // Normal position of the surface the character is currently walking on (i.e. angle of the floor)

    private bool IsSlopeSliding
    {
        get
        {
            if (isOnTheGround && Physics.Raycast(transform.position, Vector3.down, out RaycastHit slopeHit, 2f))
            {
                hitPointNormal = slopeHit.normal;
                return Vector3.Angle(hitPointNormal, Vector3.up) > controller.slopeLimit;
            }
            else
            {
                return false;
            }
        }
    }

    private void OnEnable()
    {
        StaminaSystem.OnStaminaChange += GetTired;
    }

    private void OnDisable()
    {
        StaminaSystem.OnStaminaChange -= GetTired;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        acceleration = -GRAVITY;
        groundCheck = transform.Find("GroundCheck");
        cameraComponent = GetComponentInChildren<Camera>();
        defaultYPos = cameraComponent.transform.localPosition.y;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }

    // Update is called once per frame
    void Update()
    {
        if (CanMove)
        {
            CheckIsOnTheGround();
            MoveOnInput();
            HandleJump();
            HandleCrouch();
            if (useHeadbob) CreateHeadbobEffect();
            if (canUseFootsteps) HandleFootsteps();
            ApplyGravity();
        }
    }

    private void MoveOnInput()
    {
        // Get vertical/horizontal keyboard input (can be 1- or 1)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");
        movementInput = new(x, z);
        // Move the character based on the keyboard input
        Vector3 movementVector = transform.right * x + transform.forward * z;
        // Slope sliding
        if (WillSlideOnSlopes && IsSlopeSliding) movementVector += new Vector3(hitPointNormal.x, -hitPointNormal.y, hitPointNormal.z) * slopeSpeed;
        controller.Move(MoveSpeed * Time.deltaTime * movementVector.normalized);
        movementDirection = movementVector;
    }

    // Character is getting tired depending on the stamina level.
    // The lower the stamina the slower the sprint is.
    private void GetTired(float currentStamina)
    {
        float normalizedValue = Mathf.InverseLerp(0, StaminaSystem.maxStamina, currentStamina);
        currentSprintSpeed = Mathf.Lerp(runSpeed, sprintSpeed, normalizedValue);
        currentSprintBobSpeed = Mathf.Lerp(runBobSpeed, sprintBobSpeed, normalizedValue);
    }

    private void HandleJump()
    {
        // Jumping
        if (ShouldJump)
        {
            // Calculate velocity needed to jump set height
            float v = Mathf.Sqrt(jumpHeight * GRAVITY * 2);
            velocity.y = v;
            jumpSound.Play();
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
        HandleFallDamage();
    }


    private void HandleFallDamage()
    {
        // If player falling exceeds 10, consider they are falling
        if (velocity.y < -fallWillHurtVelocity && !isOnTheGround)
        {
            // Calculate how much time the character is free falling
            fallTime += Time.deltaTime;
            falling = true;
        }
        else if (falling)
        {
            // Hurt the player based on how long they fell
            float fallDamage = Mathf.Round(fallTime * 10);
            Debug.Log("Damage from fall: " + fallDamage);
            HealthSystem.OnDamageTaken(fallDamage);

            // Reset fall measurements
            falling = false;
            fallTime = 0;
        }
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
            velocity.y = zeroVelocity;
        }
    }
    private void CreateHeadbobEffect()
    {
        // Check if the character is moving (abs because the movement direction can be negative)
        if (Mathf.Abs(movementDirection.x) > 0.1f || Mathf.Abs(movementDirection.z) > 0.1f)
        {
            // Headbob disabled when the character is not standing on the ground
            if (!isOnTheGround || duringHeightLevelAnimation) return;
            timer += Time.deltaTime * BobSpeed;
            float cameraTransitionY = defaultYPos + Mathf.Sin(timer) * BobAmount;
            cameraComponent.transform.localPosition = new Vector3(cameraComponent.transform.localPosition.x, cameraTransitionY, cameraComponent.transform.localPosition.z);
        }
        // Reset the camera y position to default when the character stops moving
        else if (defaultYPos != cameraComponent.transform.localPosition.y)
        {
            StartCoroutine(HeightLevel());
        }
    }

    private void HandleFootsteps()
    {
        if (movementInput == Vector2.zero || velocity.y != zeroVelocity) return;

        footstepSoundTimer -= Time.deltaTime;

        if (footstepSoundTimer <= 0)
        {
            if (Physics.Raycast(cameraComponent.transform.position, Vector3.down, out RaycastHit hit, 4f))
            {
                switch (hit.collider.tag)
                {
                    case "Footsteps/Grass":
                        footstepAudioSource.PlayOneShot(grassClips[Random.Range(0, grassClips.Length - 1)]);
                        break;
                    default:
                        footstepAudioSource.PlayOneShot(groundClips[Random.Range(0, groundClips.Length - 1)]);
                        break;
                }
            }

            footstepSoundTimer = GetCurrentOffset;
        }
    }

    private IEnumerator CrouchStand()
    {
        // Check for collison above the character while crouching
        // Break the Coroutine if the object is detected (character cannot stand up)
        if (IsCrouching && Physics.Raycast(cameraComponent.transform.position, Vector3.up, 1f))
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

    // Default camera height smoother
    private IEnumerator HeightLevel()
    {

        if (duringCrouchAnimation)
        {
            yield break;
        }

        duringHeightLevelAnimation = true;

        float timeElapsed = 0;
        Vector3 currentHeight = cameraComponent.transform.localPosition;
        Vector3 targetHeight;

        targetHeight = new Vector3(cameraComponent.transform.localPosition.x, defaultYPos, cameraComponent.transform.localPosition.z);

        while (timeElapsed < timeToReturnCamera)
        {
            cameraComponent.transform.localPosition = Vector3.Lerp(currentHeight, targetHeight, timeElapsed / timeToReturnCamera);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        duringHeightLevelAnimation = false;

    }
}