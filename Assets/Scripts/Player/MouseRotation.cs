using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class MouseRotation : MonoBehaviour
{

    [Header("Look Parameters")]
    [SerializeField, Range(1, 200)] public float mouseSensivityX = 100.0f;
    [SerializeField, Range(1, 200)] public float mouseSensivityY = 100.0f;
    [SerializeField] private float upperLookLimit = 85.0f;
    [SerializeField] private float lowerLookLimit = -85.0f;
    public bool canRotate = true;


    [SerializeField]
    Transform characterTransform;
    // Rotation around X-axis
    float xRotation = 0f;

    // Start is called before the first frame update
    void Start()
    {
        // Hide and lock the mouse cursor
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (canRotate)
        {
            // Get mouse x and y position
            float mouseX = Input.GetAxis("Mouse X") * mouseSensivityX * Time.deltaTime;
            float mouseY = Input.GetAxis("Mouse Y") * mouseSensivityY * Time.deltaTime;

            xRotation -= mouseY;
            // Limit the camera's x axis rotation to 180° agle
            xRotation = Mathf.Clamp(xRotation, lowerLookLimit, upperLookLimit);

            // Rotate around camera's x axis (up and down)
            transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
            // Rotate around character's y axis (left and right)
            characterTransform.Rotate(Vector3.up * mouseX);
        }
    }
}
