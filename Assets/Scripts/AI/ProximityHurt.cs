using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProximityHurt : MonoBehaviour
{
    public bool isPlayerInside = false;
    public float timeBetweenDamage = 0.5f;
    private float timeElapsed = 0.0f;

    public float damageToInflict = 10.0f;

    void Update() {
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
        }
    }

    private void OnTriggerExit(Collider other) {
        if (other.CompareTag("Player")) {
            isPlayerInside = false;
        }
    }
}
