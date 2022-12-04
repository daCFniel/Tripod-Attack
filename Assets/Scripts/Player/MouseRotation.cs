using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class MouseRotation : MonoBehaviour
{
    [SerializeField]
    float mouseSensivity = 100f;
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
        // Get mouse x and y position
        float mouseX = Input.GetAxis("Mouse X") * mouseSensivity * Time.deltaTime;
        float mouseY = Input.GetAxis("Mouse Y") * mouseSensivity * Time.deltaTime;

        xRotation -= mouseY;
        // Limit the camera's x axis rotation to 180° agle
        xRotation = Mathf.Clamp(xRotation, -90f, 90f);

        // Rotate around camera's x axis (up and down)
        transform.localRotation = Quaternion.Euler(xRotation, 0f, 0f);
        // Rotate around character's y axis (left and right)
        characterTransform.Rotate(Vector3.up * mouseX);
    }
}
