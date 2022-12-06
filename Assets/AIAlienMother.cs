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
    private float currentTime = 0.0f;
    public bool hasReachedTarget = false;

    private void Start()
    {
        MoveToNewWaypoint();
    }

    private void Update()
    {
        Animator animator = GetComponent<Animator>();
        currentTime += Time.deltaTime;

        if (Vector3.Distance(transform.position, targetWaypoint.transform.position) <= distanceToReach)
        {
            if (!hasReachedTarget)
            {
                currentTime = 0.0f;
                hasReachedTarget = true;
            }

            if (currentTime >= timeBetweenMovement)
            {
                MoveToNewWaypoint();
            }

            animator.SetFloat("speed", 0);
        }
        else
        {
            Vector3 movement = (targetWaypoint.transform.position - targetWaypoint.transform.position).normalized * movementSpeed;
            transform.position += movement;
            animator.SetFloat("speed", 1);
        }
    }

    private void MoveToNewWaypoint()
    {
        hasReachedTarget = true;
        targetWaypoint = currentWaypoint.connections[Random.Range(0, currentWaypoint.connections.Length)];
    }
}
