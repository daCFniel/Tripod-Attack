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
        KeyboardMovement.OnDamage += UpdateHealth;
        KeyboardMovement.OnHeal += UpdateHealth;
    }

    private void OnDisable()
    {
        KeyboardMovement.OnDamage -= UpdateHealth;
        KeyboardMovement.OnHeal -= UpdateHealth;
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
