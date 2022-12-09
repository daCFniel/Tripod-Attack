using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class Flashlight : MonoBehaviour
{
    Vector3 offset;
    GameObject cameraObject;
    [SerializeField] float speed;
    [SerializeField] AudioSource sound;
    [SerializeField] GameObject parent;
    bool IsOn;
    // Start is called before the first frame update
    void Start()
    {
        cameraObject = Camera.main.gameObject;
        offset = transform.position - cameraObject.transform.position;
    }

    // Update is called once per frame
    void Update()
    {
        HandleFlashlightOffset();
    }

    private void HandleFlashlightOffset()
    {
        // Change the position and rotation of the flashling based on camera transform
        transform.SetPositionAndRotation(cameraObject.transform.position + offset, Quaternion.Slerp(transform.rotation, cameraObject.transform.rotation, speed * Time.deltaTime));
    }

    public void ToggleFlashlight()
    {
        sound.Play();
        parent.SetActive(!IsOn);
        IsOn = !IsOn;
    }
}
