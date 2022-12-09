using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class TestDamageFloor : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            HealthSystem.OnDamageTaken(100);
        }
    }
}
