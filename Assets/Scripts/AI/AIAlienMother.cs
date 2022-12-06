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

    public float timeBetweenMovement = 1.0f;
    public float distanceToSeePlayer = 20.0f;
    private float currentTime = 0.0f;
    public bool hasReachedTarget = false;

    private Animator animator;
    private GameObject player;
    private AIManager manager;

    public GameObject mainLight;
    private Quaternion mainLightDefaultRot;

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
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);

        float angle = Vector2.SignedAngle(vec2Us, vec2Us - vec2Player);

        transform.rotation = Quaternion.Euler(new Vector3(
            0,
            -angle - 90,
            0
        ));
    }

    private void Update()
    {
        currentTime += Time.deltaTime;

        // If player is too close, spot them.
        if (Vector3.Distance(transform.position, player.transform.position) <= distanceToSeePlayer)
        {
            if (RaycastHitPlayer())
            {
                manager.SendChasers(player.transform.position);
                mainLight.transform.LookAt(player.transform);
                mainLight.GetComponent<Light>().color = Color.red;
                animator.SetBool("IsMoving", false);

                TurnTowardsPlayer();

                return;
            } else
            {
                ResetLighting();
            }
        } else
        {
            ResetLighting();
        }

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

            transform.LookAt(targetWaypoint.transform);
            animator.SetBool("IsMoving", true);
        }
    }

    private void MoveToNewWaypoint()
    {
        hasReachedTarget = false;
        targetWaypoint = currentWaypoint.connections[Random.Range(0, currentWaypoint.connections.Length)];
    }

    protected bool RaycastHitPlayer()
    {
        LayerMask layers = ~(1 << gameObject.layer);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, distanceToSeePlayer + 1.0f, layers))
        {
            if (hit.collider.gameObject.tag == "Player")
            {
                return true;
            }
        }

        return false;
    }
}
