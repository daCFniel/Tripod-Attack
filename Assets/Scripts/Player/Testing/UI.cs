using TMPro;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText = default;

    private void OnEnable()
    {
        HealthSystem.OnDamage += UpdateHealth;
        HealthSystem.OnHeal += UpdateHealth;
    }

    private void OnDisable()
    {
        HealthSystem.OnDamage -= UpdateHealth;
        HealthSystem.OnHeal -= UpdateHealth;
    }

    private void Start()
    {
        UpdateHealth(100);
    }

    private void UpdateHealth(float currentHealth)
    {
        healthText.text = "Health: " + currentHealth.ToString("00");
    }
}
