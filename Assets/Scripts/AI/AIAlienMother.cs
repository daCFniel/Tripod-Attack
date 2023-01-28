using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIAlienMother : MonoBehaviour
{

    public GameObject waypoints;

    public AIWaypoint currentWaypoint;
    public AIWaypoint targetWaypoint;

    public float distanceToReach = 2.0f;
    public float movementSpeed = 1.0f;

    public float sightTimeout = 0.5f;
    private float currentSightTimeout = 0.0f;

    public float timeBetweenMovement = 1.0f;
    public float distanceToSeePlayer = 20.0f;
    private float currentTime = 0.0f;
    public bool hasReachedTarget = false;

    private Animator animator;
    private GameObject player;
    private AIManager manager;

    private float targetRotationY = 0.0f;
    public float rotationSpeed = 5.0f;

    public GameObject mainLight;
    private Quaternion mainLightDefaultRot;

    public bool isAlive = true;

    private void Start()
    {
        animator = GetComponent<Animator>();
        player = GameObject.FindGameObjectsWithTag("Player")[0].gameObject;
        manager = GameObject.FindGameObjectsWithTag("AIManager")[0].GetComponent<AIManager>();
        mainLightDefaultRot = mainLight.transform.localRotation;

        transform.position = currentWaypoint.transform.position;

        MoveToNewWaypoint();
    }

    private void ResetLighting()
    {
        mainLight.transform.localRotation = mainLightDefaultRot;
        mainLight.GetComponent<Light>().color = Color.white;
    }

    private void TurnTowardsPlayer()
    {
        float oldY = transform.rotation.eulerAngles.y;

        transform.LookAt(player.transform);

        targetRotationY = transform.rotation.eulerAngles.y;

        transform.rotation = Quaternion.Euler(new Vector3(
            0,
            oldY,
            0
        ));
    }

    private void SmoothRotation() {
        transform.rotation = Quaternion.Euler(new Vector3(
            0,
            Mathf.LerpAngle(transform.rotation.eulerAngles.y, targetRotationY, rotationSpeed * Time.deltaTime),
            0
        ));
    }

    private void LateUpdate()
    {
        if (isAlive)
        {
            currentTime += Time.deltaTime;

            // If player is too close, spot them.
            if (Vector3.Distance(transform.position, player.transform.position) <= distanceToSeePlayer)
            {
                if (RaycastHitPlayer())
                {
                    if (manager) manager.SendChasers(player.transform.position);
                    currentSightTimeout = 0.0f;

                    TurnTowardsPlayer();
                    SmoothRotation();

                    mainLight.transform.LookAt(player.transform);
                    mainLight.GetComponent<Light>().color = Color.red;
                    animator.SetBool("IsMoving", false);

                    GetComponent<AlienHealth>().PlayScreechAudio();

                    return;
                }
            }

            currentSightTimeout += Time.deltaTime;

            if (currentSightTimeout <= sightTimeout)
            {
                return;
            }

            ResetLighting();

            if (Vector3.Distance(transform.position, targetWaypoint.transform.position) <= distanceToReach)
            {
                if (!hasReachedTarget)
                {
                    currentTime = 0.0f;
                    hasReachedTarget = true;
                    currentWaypoint = targetWaypoint;
                }

                if (currentTime >= timeBetweenMovement)
                {
                    MoveToNewWaypoint();
                }

                animator.SetBool("IsMoving", false);
            }
            else
            {
                Vector3 movement = (targetWaypoint.transform.position - transform.position).normalized * movementSpeed * Time.deltaTime;
                transform.position += movement;


                float oldY = transform.rotation.eulerAngles.y;
                transform.LookAt(targetWaypoint.transform);
                targetRotationY = transform.rotation.eulerAngles.y;

                transform.rotation = Quaternion.Euler(new Vector3(
                    0,
                    oldY,
                    0
                ));

                SmoothRotation();

                animator.SetBool("IsMoving", true);
            }
        }
    }

    private void MoveToNewWaypoint()
    {
        hasReachedTarget = false;
        targetWaypoint = currentWaypoint.connections[Random.Range(0, currentWaypoint.connections.Length)];
    }

    protected bool RaycastHitPlayer() {
        LayerMask layers = ~(1 << gameObject.layer);
        RaycastHit hit;

        Vector3 raycastPosition = transform.position + (Vector3.up * 10.0f);
        Vector3 playerPosition = player.transform.position + (Vector3.up * 0.0f);

        Debug.DrawRay(raycastPosition, playerPosition - raycastPosition, Color.red);

        if (Physics.Raycast(raycastPosition, playerPosition - raycastPosition, out hit, distanceToSeePlayer + 1.0f, layers)) {
            if (hit.collider.gameObject.tag == "Player") {
                return true;
            } else {
                // print("raycast hit: " + hit.collider.gameObject.name);
            }
        }

        return false;
    }
}
