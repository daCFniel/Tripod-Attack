using TMPro;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>

public class UI : MonoBehaviour
{
    [SerializeField] TextMeshProUGUI healthText = default;
    [SerializeField] TextMeshProUGUI staminaText = default;

    private void OnEnable()
    {
        HealthSystem.OnDamage += UpdateHealth;
        HealthSystem.OnHeal += UpdateHealth;
        StaminaSystem.OnStaminaChange += UpdateStamina;
    }

    private void OnDisable()
    {
        HealthSystem.OnDamage -= UpdateHealth;
        HealthSystem.OnHeal -= UpdateHealth;
        StaminaSystem.OnStaminaChange -= UpdateStamina;
    }

    private void Start()
    {
        UpdateHealth(100);
        UpdateStamina(100);
    }

    private void UpdateHealth(float currentHealth)
    {
        healthText.text = "Health: " + currentHealth.ToString("00");
    }

    private void UpdateStamina(float currentStamina)
    {
        staminaText.text = "Stamina: " + currentStamina.ToString("00");
    }
}
