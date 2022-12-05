using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFind : MonoBehaviour
{
    public Vector3 goal;
    protected GameObject player;

    public Vector3 startPos;
    public float wanderDistMax = 250.0f;
    public float timeRangeMin = 15.0f;
    public float timeRangeMax = 30.0f;
    protected float timeCountdown;

    public float timeSinceLastSeenPlayer = 0.0f;
    public float chaserTimeoutUnseen = 10.0f;
    public bool hasReachedInitialGoal = false;
    public Vector3 lastSeenPlayerPos = Vector3.zero;

    public float angleToSeePlayer = 25.0f;
    public float distanceToSeePlayer = 15.0f;

    public AlienAnimator animator;

    public State currentState;
    protected AIManager manager;

    public enum State
    {
        WANDER,
        CHASE,
        RETURN
    }


    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        startPos = transform.position;
        currentState = State.WANDER;
        manager = GameObject.FindGameObjectsWithTag("AIManager")[0].GetComponent<AIManager>();

        if (gameObject.CompareTag("EnemyScout"))
        {
            hasReachedInitialGoal = true;
        }

        NewGoal();
    }

    private void OnEnable()
    {
        timeSinceLastSeenPlayer = 0.0f;
        hasReachedInitialGoal = false;
        currentState = State.WANDER;
        lastSeenPlayerPos = Vector3.zero;
    }

    void Update()
    {
        // print(name + " is in state " + currentState.ToString());

        switch (currentState)
        {
            case State.WANDER:
                Wander();
                break;
            case State.CHASE:
                Chase();
                break;
            case State.RETURN:
                StateReturn();
                break;
            default:
                break;
        }
    }

    protected virtual void Attack() { }
    protected virtual void Wander() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal;

        timeCountdown -= Time.deltaTime;
        if (timeCountdown <= 0 && hasReachedInitialGoal)
        {
            NewGoal();
        }

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
        }
    }

    protected virtual void Chase() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        timeCountdown = Random.Range(timeRangeMin, timeRangeMax);
        goal = player.transform.position;
        agent.destination = goal;

        manager.SendChasers(player.transform.position);

        if (!RaycastHitPlayer())
        {
            currentState = State.WANDER;
        }
    }

    protected virtual void StateReturn() { }

    protected void HandleMovementAnim()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        if (agent.velocity.magnitude <= 0.025)
        {
            PlayAnimation("Fight_Idle_1", false);
        }
        else
        {
            PlayAnimation("Walk_Cycle_1", false);
        }
    }

    protected bool CanSeePlayer()
    {
        Vector2 vec2goal = new Vector2(goal.x, goal.z);
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);
        Vector2 vec2Forward = new Vector2(transform.forward.x, transform.forward.z);

        Vector2 playerDirection = vec2Player - vec2Us;
        float angle = Vector2.Angle(playerDirection, vec2Forward);
        float distance = Vector2.Distance(vec2Player, vec2Us);

        if (angle < angleToSeePlayer && distance < distanceToSeePlayer)
        {
            // Raycast is expensive, so check angle and distance first.
            if (RaycastHitPlayer())
            {
                return true;
            }
        }

        return false;
    }

    protected bool RaycastHitPlayer() {
        LayerMask layers = ~(1 << gameObject.layer);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, distanceToSeePlayer + 1.0f, layers)) {
            if (hit.collider.gameObject.tag == "Player") {
                return true;
            }
        }

        return false;
    }

    protected void NewGoal()
    {
        NavMeshHit valid;

        NavMesh.SamplePosition(startPos + (Random.insideUnitSphere * wanderDistMax), out valid, wanderDistMax, -1);

        goal = valid.position;

        timeCountdown = Random.Range(timeRangeMin, timeRangeMax);
    }

    protected void PlayAnimation(string animName, bool force)
    {
        if (animator == null) return;

        if (!force)
        {
            animator.PlayAnim(animName);
        } else
        {
            animator.animator.SetTrigger(animName);
        }
    }
}
