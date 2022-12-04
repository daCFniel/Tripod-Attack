using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class KeyboardMovement : MonoBehaviour
{
    const float GRAVITY = 9.81f;

    CharacterController controller;

    Transform groundCheck;
    [SerializeField]
    float groundDistance = 0.4f;
    [SerializeField]
    LayerMask groundMask; // Control what objects the Physics.CheckSphere should check for
    bool isOnTheGround;

    [SerializeField]
    float speed = 10f;
    [SerializeField]
    float jumpHeight = 10f;
    float acceleration;

    Vector3 velocity;

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
        CheckIsOnTheGround();
        MoveOnInput();
        ApplyGravity();
    }

    private void MoveOnInput()
    {
        // Get vertical/horizontal keyboard input (can be 1- or 1)
        float x = Input.GetAxisRaw("Horizontal");
        float z = Input.GetAxisRaw("Vertical");

        // Move the character based on the keyboard input
        Vector3 movementVector = transform.right * x + transform.forward * z;
        controller.Move(speed * Time.deltaTime * movementVector.normalized);

        // Jumping
        if(Input.GetButtonDown("Jump") && isOnTheGround)
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
        isOnTheGround = Physics.CheckSphere(groundCheck.position, groundDistance, groundMask);

        // Reset the y velocity (falling) when the playing is touching the ground
        if (isOnTheGround && velocity.y < 0)
        {
            velocity.y = -1f;
        }
    }
}