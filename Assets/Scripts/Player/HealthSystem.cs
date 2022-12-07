using System;
using System.Collections;
using UnityEngine;
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
    [Header("Visuals")]
    [SerializeField] RawImage bloodBorderImage;
    [SerializeField] Image bloodSplashImage;
    [Header("SFX")]
    [SerializeField] AudioSource heartBeat;
    [SerializeField] AudioSource pain;
    [SerializeField] float painSoundTime; 
    [SerializeField] float maxPainVolume;
    bool painCoroutineStarted;

    float currentHealth;
    Coroutine regenHealth;
    public static Action<float> OnDamageTaken;
    public static Action<float> OnDamage;
    public static Action<float> OnHeal;
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
        currentHealth = maxHealth;
        heartBeat.Play();
        pain.Play();
    }

    private void ApplyDamage(float dmgAmount)
    {
        currentHealth -= dmgAmount;
        OnDamage?.Invoke(currentHealth); // Invoke only if anything is listening to the acton event
        ApplyBloodEffect();
        HandleHeartBeat();

        if (currentHealth <= 0) KillPlayer();
        else if (regenHealth != null) StopCoroutine(regenHealth); // Reset the hp regen timer if the character receives damage

        regenHealth = StartCoroutine(RegenHealth()); //Start new hp regen timer
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
    private void KillPlayer()
    {
        // Set health to 0 if overkill occured (negative hp)
        currentHealth = 0;

        if (regenHealth != null) StopCoroutine(regenHealth);

        print("DEAD! YOU HAVE BEEN KILLED");
    }

    private void HandleHeartBeat()
    {
        heartBeat.volume = Mathf.InverseLerp(maxHealth, 0, currentHealth);
    }

    private IEnumerator RegenHealth()
    {
        yield return new WaitForSeconds(timeBeforeRegenStarts);
        WaitForSeconds regenTickTime = new(healthRegenInterval);

        while (currentHealth < maxHealth)
        {
            currentHealth += healthRegenAmount;

            ApplyBloodEffect();
            HandleHeartBeat();

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
