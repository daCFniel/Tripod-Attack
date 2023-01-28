using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class Rotation : MonoBehaviour
{
    [SerializeField]
    private float rotationSpeed; // degrees per second
     void Update()
    {
        transform.Rotate(0, rotationSpeed * Time.deltaTime, 0, Space.World);
    }
}
