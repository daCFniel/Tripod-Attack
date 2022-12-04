using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class KeyboardMovement : MonoBehaviour
{
    const float GRAVITY = 9.81f;

    [Header("Instances")]
    CharacterController controller;
    [SerializeField]
    LayerMask groundMask; // Control what objects the Physics.CheckSphere should check for
    Transform groundCheck;

    [Header("Movement Parameters")]
    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float sprintSpeed = 20f;
    [SerializeField]
    float jumpHeight = 10f;
    [SerializeField]
    float groundDistance = 0.4f;

    [Header("Functinal Options")]
    bool isOnTheGround;
    bool canMove = true;
    bool canSprint = true;
    bool IsSprinting => canSprint && Input.GetKey(sprintKey);

    [Header("Physics")]
    float acceleration;
    Vector3 velocity;

    [Header("Controls")]
    [SerializeField]
    KeyCode sprintKey = KeyCode.LeftShift;

    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CharacterController>();
        acceleration = -GRAVITY;
        groundCheck = transform.Find("GroundCheck");
    }

    // Update is called once per frame
    void Update()
    {
        if (canMove)
        {
            CheckIsOnTheGround();
            MoveOnInput();
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
        controller.Move((IsSprinting ? sprintSpeed : speed) * Time.deltaTime * movementVector.normalized);

        // Jumping
        if (Input.GetButtonDown("Jump") && isOnTheGround)
        {
            // Calculate velocity needed to jump set height
            float v = Mathf.Sqrt(jumpHeight * GRAVITY * 2);
            velocity.y = v;
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
}