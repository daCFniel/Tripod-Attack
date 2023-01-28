using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GasProximityHurt : MonoBehaviour
{
    public bool isPlayerInside = false;
    public float timeBetweenDamage = 0.5f;
    private float timeElapsed = 0.0f;

    public float damageToInflict = 10.0f;

    private Inventory inventory;
    private GameObject gasMaskOverlay;
    public AudioSource maskAudio;

    private void Start()
    {
        inventory = GameObject.FindGameObjectWithTag("Player").transform.Find("Inventory").GetComponent<Inventory>();
        gasMaskOverlay = GameObject.FindGameObjectWithTag("Player").transform.Find("PlayerCanvas").Find("Gas Mask Overlay").gameObject;
    }

    void Update() {
        if (inventory.hasGasMask) return;

        if (isPlayerInside) {
            timeElapsed += Time.deltaTime;

            if (timeElapsed >= timeBetweenDamage) {
                HealthSystem.OnDamageTaken(damageToInflict);
                timeElapsed = 0.0f;
            }
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInside = true;
            timeElapsed = 0.0f;

            if (inventory.hasGasMask)
            {
                maskAudio.Play();
                gasMaskOverlay.SetActive(true);
            }
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInside = false;

            if (inventory.hasGasMask)
            {
                maskAudio.Play();
                gasMaskOverlay.SetActive(false);
            }
        }
    }
}
