using System;
using System.Collections;
using TMPro;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class KeyboardMovement : MonoBehaviour
{

    [Header("Functinal Options")]
    [SerializeField] bool isOnTheGround;
    [SerializeField] bool canSprint = true;
    [SerializeField] bool canJump = true;
    [SerializeField] bool canMove = true;
    [SerializeField] bool canCrouch = true;
    [SerializeField] bool canWalk = true;
    [SerializeField] bool useHeadbob = true;
    [SerializeField] bool canZoom = true;
    [SerializeField] bool canInteract = true;

    // Lambda fields
    bool IsSprinting => canSprint && Input.GetKey(sprintKey);
    bool IsWalking => canWalk && Input.GetKey(walkKey);
    bool ShouldJump => Input.GetKeyDown(jumpKey) && isOnTheGround && canJump;
    float MoveSpeed => IsCrouching ? crouchSpeed : IsWalking ? walkSpeed : IsSprinting ? sprintSpeed : runSpeed;
    bool ShouldCrouch => (Input.GetKeyDown(crouchKey) || Input.GetKeyUp(crouchKey)) && !duringCrouchAnimation && isOnTheGround && canCrouch;
    float BobSpeed => IsCrouching ? crouchBobSpeed : IsWalking ? walkBobSpeed : IsSprinting ? sprintBobSpeed : runBobSpeed;
    float BobAmount => IsCrouching ? crouchBobAmount : IsWalking ? walkBobAmount : IsSprinting ? sprintBobAmount : runBobAmount;
    bool ShouldInteract => Input.GetKeyDown(interactKey) && currentInteractable != null;

    [Header("Instances")]
    [SerializeField] LayerMask groundMask; // Control what objects the Physics.CheckSphere should check for
    Transform groundCheck;
    CharacterController controller;
    Camera camera;

    [Header("Movement Parameters")]
    [SerializeField] float crouchSpeed = 4f;
    [SerializeField] float walkSpeed = 6f;
    [SerializeField] float runSpeed = 10f;
    [SerializeField] float sprintSpeed = 20f;

    [Header("Health Parameters")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float timeBeforeRegenStarts = 3f;
    [SerializeField] float healthRegenAmount = 1f;
    [SerializeField] float healthRegenInterval = 0.1f;
    float currentHealth;
    Coroutine regenHealth;
    public static Action<float> OnDamageTaken;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;

    [SerializeField]
    Vector3 movementDirection;

    [Header("Jump Parameters")]
    [SerializeField] float jumpHeight = 10f;
    [SerializeField] float groundDistance = 0.4f;
    [SerializeField] float sloapLimit = 100f;
    float defaultSloap;

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
    [SerializeField] float runBobSpeed = 10f;
    [SerializeField] float runBobAmount = 0.05f;
    [SerializeField] float sprintBobSpeed = 15f;
    [SerializeField] float sprintBobAmount = 0.1f;
    [SerializeField] float timeToReturnCamera = 0.1f;
    float defaultYPos = 0; // camera position
    float timer; // used to determine where the camera should be at given moment
    bool duringHeightLevelAnimation;

    [Header("Interactions")]
    [SerializeField] Vector3 interactionRayPoint = default;
    [SerializeField] float interactionDistance = default;
    [SerializeField] LayerMask interactionLayer = default;
    Interactable currentInteractable;

    [Header("Zoom Feature")]
    [SerializeField] float timeToZoom = 0.4f;
    [SerializeField] float zoomFOV = 30f; // Target camera Field of View when zooming in
    float defaultFOV;
    Coroutine zoomRoutine;

    [Header("Physics")]
    [SerializeField] Vector3 velocity;
    [SerializeField] float fallWillHurtVelocity = 5f;
    [SerializeField] float fallTime;
    bool falling;
    float acceleration;
    const float GRAVITY = 9.81f;

    [Header("Controls")]
    [SerializeField] KeyCode sprintKey = KeyCode.LeftShift;
    [SerializeField] KeyCode jumpKey = KeyCode.Space;
    [SerializeField] KeyCode crouchKey = KeyCode.C;
    [SerializeField] KeyCode walkKey = KeyCode.LeftControl;
    [SerializeField] KeyCode zoomKey = KeyCode.Mouse1;
    [SerializeField] KeyCode interactKey = KeyCode.E;

    private void OnEnable()
    {
        OnDamageTaken += ApplyDamage;
    }

    private void OnDisable()
    {
        OnDamageTaken -= ApplyDamage;
    }

    void Awake()
    {
        controller = GetComponent<CharacterController>();
        acceleration = -GRAVITY;
        groundCheck = transform.Find("GroundCheck");
        camera = GetComponentInChildren<Camera>();
        defaultYPos = camera.transform.localPosition.y;
        defaultFOV = camera.fieldOfView;
        defaultSloap = controller.slopeLimit;
        currentHealth = maxHealth;
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
            if (useHeadbob) CreateHeadbobEffect();
            if (canZoom) ZoomCamera();
            if (canInteract)
            {
                ProcessInteractions();
                HandleInteractionsInput();
            }
            ApplyGravity();
        }
    }

    private void ApplyDamage(float dmgAmount)
    {
        currentHealth -= dmgAmount;
        OnDamage?.Invoke(currentHealth); // Invoke only if anything is listening to the acton event

        if (currentHealth <= 0) KillPlayer();
        else if (regenHealth != null) StopCoroutine(regenHealth); // Reset the hp regen timer if the character receives damage

        regenHealth = StartCoroutine(RegenHealth()); //Start new hp regen timer
    }

    private void KillPlayer()
    {
        // Set health to 0 if overkill occured (negative hp)
        currentHealth = 0;

        if (regenHealth != null) StopCoroutine(regenHealth);

        print("DEAD! YOU HAVE BEEN KILLED");
    }

    // Perform an action after pressing an interaction key
    private void HandleInteractionsInput()
    {
        if (ShouldInteract && Physics.Raycast(camera.ViewportPointToRay(interactionRayPoint), out _, interactionDistance, interactionLayer))
        {
            currentInteractable.OnInteract();
        }
    }

    // Look for interactable objects
    private void ProcessInteractions()
    {
        // Take into consideration all colliders when checking for interactable objects (To prevent interacting through objects)
        if (Physics.Raycast(camera.ViewportPointToRay(interactionRayPoint), out RaycastHit hit, interactionDistance))
        {
            bool collidedWithInteractableLayer = hit.collider.gameObject.layer == Interactable.interactableLayerId;
            // If its on Interactable layer and the character is not currently interacting (or is changing focus to other object next by)
            if (collidedWithInteractableLayer && (currentInteractable == null || hit.collider.gameObject.GetInstanceID() != currentInteractable.GetInstanceID()))
            {
                // Try to get the object the ray collided with
                hit.collider.TryGetComponent(out currentInteractable);

                if (currentInteractable)
                {
                    currentInteractable.OnFocus();
                }
            }
        } // Stop interacting
        else if (currentInteractable)
        {
            currentInteractable.OnLoseFocus();
            currentInteractable = null;
        }
    }

    private void ZoomCamera()
    {
        if (Input.GetKeyDown(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            // Start zooming in
            zoomRoutine = StartCoroutine(ToggleCameraZoom(true));
        }

        if (Input.GetKeyUp(zoomKey))
        {
            if (zoomRoutine != null)
            {
                StopCoroutine(zoomRoutine);
                zoomRoutine = null;
            }
            // Start zooming out
            zoomRoutine = StartCoroutine(ToggleCameraZoom(false));
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
            camera.transform.localPosition = new Vector3(camera.transform.localPosition.x, cameraTransitionY, camera.transform.localPosition.z);
        }
        // Reset the camera y position to default when the character stops moving
        else if (defaultYPos != camera.transform.localPosition.y)
        {
            StartCoroutine(HeightLevel());
        }
    }

    private void MoveOnInput()
    {
        // Get vertical/horizontal keyboard input (can be 1- or 1)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Move the character based on the keyboard input
        Vector3 movementVector = transform.right * x + transform.forward * z;
        controller.Move(MoveSpeed * Time.deltaTime * movementVector.normalized);
        movementDirection = movementVector;
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
            Debug.Log("Damage from fall: " + Mathf.Round(fallTime * 10));

            // Reset fall measurements
            falling = false;
            fallTime = 0;
        }
    }

    private void CheckIsOnTheGround()
    {
        // Check wether the character is touching the ground
        //isOnTheGround = controller.isGrounded;
        // Custom ground checking
        isOnTheGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset the y velocity (falling) when the playing is touching the ground
        if (isOnTheGround && velocity.y < 0)
        {
            controller.slopeLimit = defaultSloap;
            velocity.y = -2f;
        }
        else // Change the slope when jumping
        {
            controller.slopeLimit = sloapLimit;
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

    // Default camera height smoother
    private IEnumerator HeightLevel()
    {

        if (duringCrouchAnimation)
        {
            yield break;
        }

        duringHeightLevelAnimation = true;

        float timeElapsed = 0;
        Vector3 currentHeight = camera.transform.localPosition;
        Vector3 targetHeight;

        targetHeight = new Vector3(camera.transform.localPosition.x, defaultYPos, camera.transform.localPosition.z);

        while (timeElapsed < timeToReturnCamera)
        {
            camera.transform.localPosition = Vector3.Lerp(currentHeight, targetHeight, timeElapsed / timeToReturnCamera);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        duringHeightLevelAnimation = false;

    }

    private IEnumerator ToggleCameraZoom(bool isZoomed)
    {
        float targetFOV = isZoomed ? zoomFOV : defaultFOV;
        float startingFOV = camera.fieldOfView;
        float timeElapsed = 0f;

        while (timeElapsed < timeToZoom)
        {
            camera.fieldOfView = Mathf.Lerp(startingFOV, targetFOV, timeElapsed / timeToZoom);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        camera.fieldOfView = targetFOV;
        zoomRoutine = null;
    }

    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds regenTickTime = new(healthRegenInterval);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthRegenAmount;

            // Prevent overhealing
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            OnHeal?.Invoke(currentHealth);
            yield return regenTickTime;
        }

        regenHealth = null;
    }
}