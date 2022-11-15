using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class Utils : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }
}
