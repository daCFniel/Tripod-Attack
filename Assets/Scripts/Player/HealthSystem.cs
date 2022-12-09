using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.PostProcessing;
using UnityEngine.UI;
/// <summary>
/// Daniel Bielech db662 - COMP6100
/// </summary>
public class HealthSystem : MonoBehaviour
{
    [Header("Health Parameters")]
    [SerializeField] float maxHealth = 100f;
    [SerializeField] float timeBeforeRegenStarts = 3f;
    [SerializeField] float healthRegenAmount = 1f;
    [SerializeField] float healthRegenInterval = 0.1f;
    [SerializeField] float maxMovementPenalty = 0.5f;
    [Header("Visuals")]
    [SerializeField] RawImage bloodBorderImage;
    [SerializeField] RawImage bloodSplashImage;
    [SerializeField] Camera cameraComponent;
    [SerializeField] float showDeathScreenAfter;
    [SerializeField] GameObject deathScreen;
    [SerializeField] float timeToFallOnDeath;
    PostProcessVolume playerPPV;
    float defaultSaturation;
    ColorGrading colorGrading;
    [Header("SFX")]
    [SerializeField] AudioSource heartBeat;
    [SerializeField] AudioSource pain;
    [SerializeField] List<AudioClip> onDamageSounds;
    [SerializeField] List<AudioClip> deathPianoSounds;
    [SerializeField] AudioClip deathPlayerSound;
    [SerializeField] float painSoundTime;
    [SerializeField] float maxPainVolume;
    [SerializeField] AudioSource currentSound;
    bool painCoroutineStarted;

    float currentHealth;
    Coroutine regenHealth;
    public static Action<float> OnDamageTaken;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;

    CustomCharacterController controller;
    CharacterController charController;
    private void OnEnable()
    {
        OnDamageTaken += ApplyDamage;
    }

    private void OnDisable()
    {
        OnDamageTaken -= ApplyDamage;
    }


    // Start is called before the first frame update
    void Start()
    {
        controller = GetComponent<CustomCharacterController>();
        charController = GetComponent<CharacterController>();
        playerPPV = cameraComponent.GetComponent<PostProcessVolume>();
        playerPPV.profile.TryGetSettings(out colorGrading);
        defaultSaturation = colorGrading.saturation.value;
        currentHealth = maxHealth;
        heartBeat.Play();
        pain.Play();
    }

    private void ApplyDamage(float dmgAmount)
    {
        RotateBloodSplash();
        currentHealth -= dmgAmount;
        OnDamage?.Invoke(currentHealth); // Invoke only if anything is listening to the acton event
        PlayOnDamageSound();
        HandleHpLossEffects();

        if (currentHealth <= 0) KillPlayer();
        else if (regenHealth != null) StopCoroutine(regenHealth); // Reset the hp regen timer if the character receives damage

        regenHealth = StartCoroutine(RegenHealth()); //Start new hp regen timer
    }

    private void HandleHpLossEffects()
    {
        ApplyBloodEffect();
        ApplyGreyScaleEffect();
        HandleHeartBeat();
        HandleMovementPenalty();
    }

    private void HandleMovementPenalty()
    {
        float normalized = Mathf.InverseLerp(maxHealth, 0, currentHealth);
        controller.movementSpeedMultiplier = Mathf.Lerp(1, maxMovementPenalty, normalized);
    }

    private void ApplyGreyScaleEffect()
    {
        playerPPV.profile.TryGetSettings(out colorGrading);
        float normalized = Mathf.InverseLerp(maxHealth, 0, currentHealth);
        colorGrading.saturation.value = Mathf.Lerp(defaultSaturation, -100, normalized);
    }

    private void PlayOnDamageSound()
    {
        AudioClip clip = onDamageSounds[UnityEngine.Random.Range(0, onDamageSounds.Count)];
        currentSound.clip = clip;
        currentSound.Play();
    }

    private void RotateBloodSplash()
    {
        if (currentHealth == maxHealth)
        {
            float randomRot = UnityEngine.Random.Range(0, 360);
            bloodSplashImage.transform.rotation = Quaternion.Euler(0, 0, randomRot);
        }
    }

    private void ApplyBloodEffect()
    {
        Color borderColor = bloodBorderImage.color;
        Color splashColor = bloodSplashImage.color;
        float newAlpha = 1 - (currentHealth / maxHealth);

        if (currentHealth < maxHealth / 2)
        {
            float normalizedValue = Mathf.InverseLerp(maxHealth / 2, 0, currentHealth);
            float splashAlpha = Mathf.Lerp(0, 1, normalizedValue);
            splashColor.a = splashAlpha;
            bloodSplashImage.color = splashColor;
            if (!painCoroutineStarted) StartCoroutine(PlayPainSound(painSoundTime, maxPainVolume, painSoundTime));
        }
        borderColor.a = newAlpha;
        bloodBorderImage.color = borderColor;
    }
    private void HandleHeartBeat()
    {
        heartBeat.volume = Mathf.InverseLerp(maxHealth, 0, currentHealth);
    }
    private void KillPlayer()
    {
        // Set health to 0 if overkill occured (negative hp)
        currentHealth = 0;

        if (regenHealth != null) StopCoroutine(regenHealth);

        print("DEAD! YOU HAVE BEEN KILLED");

        AudioClip clip = deathPianoSounds[UnityEngine.Random.Range(0, deathPianoSounds.Count)];
        currentSound.clip = clip;
        currentSound.Play();
        AudioSource.PlayClipAtPoint(deathPlayerSound, controller.transform.position);

        controller.CanSprint = false;
        cameraComponent.GetComponent<MouseRotation>().mouseSensivityX /= 4;
        cameraComponent.GetComponent<MouseRotation>().mouseSensivityY /= 4;
        controller.movementSpeedMultiplier /= 5;
        StartCoroutine(FallOnTheGround());
        StartCoroutine(ShowDeathScreen());


    }

    private IEnumerator FallOnTheGround()
    {
        // Set target values
        float timeElapsed = 0f;
        float targetHeight = 0f;
        float currentHeight = charController.height;
        Vector3 targetCenter = new(0, 1.5f, 0);
        Vector3 currentCenter = charController.center;

        // Crouching loop
        while (timeElapsed < timeToFallOnDeath)
        {
            charController.height = Mathf.Lerp(currentHeight, targetHeight, timeElapsed / timeToFallOnDeath);
            // By lerping the center of the controller we enable smooth camera movement when crouching
            charController.center = Vector3.Lerp(currentCenter, targetCenter, timeElapsed / timeToFallOnDeath);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
    }

    private IEnumerator ShowDeathScreen()
    {
        yield return new WaitForSeconds(showDeathScreenAfter);
        deathScreen.SetActive(true);
    }


    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds regenTickTime = new(healthRegenInterval);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthRegenAmount;

            HandleHpLossEffects();

            // Prevent overhealing
            if (currentHealth > maxHealth) currentHealth = maxHealth;

            OnHeal?.Invoke(currentHealth);
            yield return regenTickTime;
        }

        regenHealth = null;
    }

    IEnumerator PlayPainSound(float changeTime, float maxVolume, float backTime)
    {
        painCoroutineStarted = true;
        float timeElapsed = 0f;
        float startingVolume = 0f;

        while (timeElapsed < changeTime)
        {
            pain.volume = Mathf.Lerp(startingVolume, maxVolume, timeElapsed / changeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        pain.volume = maxVolume;

        yield return new WaitForSeconds(backTime);

        timeElapsed = 0f;
        startingVolume = pain.volume;

        while (timeElapsed < changeTime)
        {
            pain.volume = Mathf.Lerp(startingVolume, 0f, timeElapsed / changeTime);
            timeElapsed += Time.deltaTime;
            yield return null;
        }
        pain.volume = 0f;
        painCoroutineStarted = false;
    }
}
