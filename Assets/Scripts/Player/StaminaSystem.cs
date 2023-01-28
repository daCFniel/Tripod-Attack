using System;
using System.Collections;
using UnityEngine;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class StaminaSystem : MonoBehaviour
{
    [Header("Stamina Parameters")]
    [SerializeField] bool  useStamina = true;
    [SerializeField] float staminaUseMultiplier = 5f;
    [SerializeField] float timeBeforeRegenStarts = 5f;
    [SerializeField] float staminaRegenAmount = 2f;
    [SerializeField] float staminaRegenInterval = 0.1f;
    public static float maxStamina = 100f;
    [Header("SFX")]
    [SerializeField] AudioSource heavyBreathingSound;
    public float currentStamina;
    Coroutine regenStamina;
    public static Action<float> OnStaminaChange;
    CustomCharacterController controller;
    bool ShouldUseStamina => controller.CanMove && controller.CanSprint && useStamina;

    void Start()
    {
        heavyBreathingSound.Play();
        controller = GetComponent<CustomCharacterController>();
        currentStamina = maxStamina;
    }

    private void Update()
    {
        if (ShouldUseStamina) HandleStamina();
    }

    private void HandleStamina()
    {
        // Use the stamina only when the sprint key is pressed and the character is moving
        if (controller.IsSprinting && controller.movementInput != Vector2.zero)
        {
            if (regenStamina != null)
            {
                StopCoroutine(regenStamina);
                regenStamina = null;
            }

            currentStamina -= staminaUseMultiplier * Time.deltaTime;

            if (currentStamina < 0f)
            {
                currentStamina = 0f;
            }

            OnStaminaChange?.Invoke(currentStamina);
            HandleHeavyBreathing();

            // Disable sprinting when the stamina reaches 0
            if (currentStamina <= 0f)
            {
                controller.CanSprint = false;
            }
        }

        // Regen stamina only when the character is not sprinting and stamina is not full
        if (!controller.IsSprinting && currentStamina < maxStamina && regenStamina == null)
        {
            regenStamina = StartCoroutine(RegenStamina());
        }
    }

    private void HandleHeavyBreathing()
    {
        heavyBreathingSound.volume = Mathf.InverseLerp(maxStamina, 0, currentStamina) / 4;
    }

    private IEnumerator RegenStamina()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds timeToWait = new(staminaRegenInterval);

        while (currentStamina < maxStamina)
        {
            if (currentStamina > 0f)
            {
                controller.CanSprint = true;
            }

            currentStamina += staminaRegenAmount;

            if (currentStamina > maxStamina)
            {
                currentStamina = maxStamina;
            }

            OnStaminaChange?.Invoke(currentStamina);
            HandleHeavyBreathing();

            yield return timeToWait;
        }

        regenStamina = null;
    }
}
