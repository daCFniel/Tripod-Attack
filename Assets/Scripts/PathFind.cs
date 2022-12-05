using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFind : MonoBehaviour
{
    public Vector3 goal;
    private GameObject player;
    public Light spotlight;

    public Vector3 startPos;
    public float wanderDistMax = 250.0f;
    public float timeRangeMin = 15.0f;
    public float timeRangeMax = 30.0f;
    private float timeCountdown;

    public float timeSinceLastSeenPlayer = 0.0f;
    public float chaserTimeoutUnseen = 10.0f;
    public bool hasReachedInitialGoal = false;
    public Vector3 lastSeenPlayerPos = Vector3.zero;

    public float angleToSeePlayer = 25.0f;
    public float distanceToSeePlayer = 15.0f;
    public float attackDistance = 2.0f;
    public float attackTimeout = 1.25f; // This is from length of animation
    private float currentAttackTimeout = 0.0f;

    public AlienAnimator animator;

    public State currentState;
    private AIManager manager;

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
                wander();
                break;
            case State.CHASE:
                chase();
                break;
            case State.RETURN:
                state_return();
                break;
            default:
                break;
        }

        currentAttackTimeout += Time.deltaTime;
    }

    private void state_return() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        goal = manager.enemySpawnPosition.transform.position;
        agent.destination = goal;
        hasReachedInitialGoal = false;

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
        }

        if (Vector3.Distance(transform.position, manager.enemySpawnPosition.transform.position) <= 5.0f) {
            gameObject.SetActive(false);
            manager.chasersActive -= 1;
        }

        HandleMovementAnim();
    }


    private void chase() {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        timeCountdown = Random.Range(timeRangeMin, timeRangeMax);
        goal = player.transform.position;
        agent.destination = goal;

        manager.SendChasers(player.transform.position);

        if (!RaycastHitPlayer()) {
            currentState = State.WANDER;
        }

        if (spotlight) spotlight.color = Color.red;

        if (gameObject.CompareTag("EnemyChaser"))
        {
            timeSinceLastSeenPlayer = 0.0f;

            if (CanAttackPlayer())
            {
                if (currentAttackTimeout >= attackTimeout)
                {
                    Attack();
                }
                
            } else
            {
                HandleMovementAnim();
            }
        }
        else
        {
            HandleMovementAnim();
        }
    }

    private bool CanAttackPlayer()
    {
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);

        float distance = Vector2.Distance(vec2Player, vec2Us);

        return (distance <= attackDistance);
    }

    private void Attack()
    {
        // attack player!
        print("i am attacking the player");
        PlayAnimation("Attack_1", true);
        currentAttackTimeout = 0.0f;
    }

    private void wander() {
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

        if (spotlight) spotlight.color = Color.white;

        if (gameObject.CompareTag("EnemyChaser"))
        {
            if (Vector3.Distance(transform.position, lastSeenPlayerPos) <= 10.0f)
            {
                hasReachedInitialGoal = true;
            }

            if (hasReachedInitialGoal)
            {
                timeSinceLastSeenPlayer += Time.deltaTime;
                if (timeSinceLastSeenPlayer >= chaserTimeoutUnseen)
                {
                    currentState = State.RETURN;
                }
            }
        }

        HandleMovementAnim();
    }

    private void HandleMovementAnim()
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

    private bool CanSeePlayer()
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

    private bool RaycastHitPlayer() {
        LayerMask layers = ~(1 << gameObject.layer);
        RaycastHit hit;

        if (Physics.Raycast(transform.position, player.transform.position - transform.position, out hit, distanceToSeePlayer + 1.0f, layers)) {
            if (hit.collider.gameObject.tag == "Player") {
                return true;
            }
        }

        return false;
    }

    private void NewGoal()
    {
        NavMeshHit valid;

        NavMesh.SamplePosition(startPos + (Random.insideUnitSphere * wanderDistMax), out valid, wanderDistMax, -1);

        goal = valid.position;

        timeCountdown = Random.Range(timeRangeMin, timeRangeMax);
    }

    private void PlayAnimation(string animName, bool force)
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
