using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFind : MonoBehaviour
{
    public Vector3 goal;
    private GameObject player;
    private Light spotlight;

    private Vector3 startPos;
    public float wanderDistMax = 250.0f;
    public float timeRangeMin = 15.0f;
    public float timeRangeMax = 30.0f;
    private float timeCountdown;


    public float angleToSeePlayer = 25.0f;
    public float distanceToSeePlayer = 15.0f;
    private State currentState;

    private enum State
    {
        WANDER,
        CHASE
    }


    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        startPos = transform.position;
        spotlight = this.transform.Find("Light").GetComponent<Light>();
        currentState = State.WANDER;

        newGoal();
    }

    void Update()
    {
        switch (currentState)
        {
            case State.WANDER:
                wander();
                break;
            case State.CHASE:
                chase();
                break;
            default:
                break;
        }
    }

    private void chase() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        timeCountdown = Random.Range(timeRangeMin, timeRangeMax);
        goal = player.transform.position;
        agent.destination = goal;

        if (!raycastHitPlayer()) {
            currentState = State.WANDER;
        }

        spotlight.color = Color.red;
    }

    private void wander() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal;

        Vector2 vec2goal = new Vector2(goal.x, goal.z);
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);
        Vector2 vec2Forward = new Vector2(transform.forward.x, transform.forward.z);

        // print(agent.pathStatus);
        // print(agent.remainingDistance);

        timeCountdown -= Time.deltaTime;
        if (timeCountdown <= 0) {
            newGoal();
        }

        Vector2 playerDirection = vec2Player - vec2Us;
        float angle = Vector2.Angle(playerDirection, vec2Forward);
        float distance = Vector2.Distance(vec2Player, vec2Us);

        if (angle < angleToSeePlayer && distance < distanceToSeePlayer)
        {
            // Raycast is expensive, so check angle and distance first.
            if (raycastHitPlayer()) {
                currentState = State.CHASE;
            }
        }

        spotlight.color = Color.white;
    }

    private bool raycastHitPlayer() {
        LayerMask layers = ~(1 << gameObject.layer);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, distanceToSeePlayer + 1.0f, layers)) {
            if (hit.collider.gameObject.tag == "Player") {
                return true;
            }
        }

        return false;
    }

    private void newGoal()
    {
        NavMeshHit valid;

        NavMesh.SamplePosition(startPos + (Random.insideUnitSphere * wanderDistMax), out valid, wanderDistMax, -1);

        goal = valid.position;

        timeCountdown = Random.Range(timeRangeMin, timeRangeMax);
    }
}
