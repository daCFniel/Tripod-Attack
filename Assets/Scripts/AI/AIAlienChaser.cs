using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAlienChaser : PathFind
{
    public float attackDistance = 2.0f;
    public float attackTimeout = 1.25f; // This is from length of animation
    private float currentAttackTimeout = 0.0f;

    public float timeBetweenJumps = 10.0f;
    private float timeSinceLastJump = 0.0f;
    public bool isJumping = false;

    public float jumpForceForward = 15.0f;
    public float jumpForceUpward = 7.5f;

    public AudioSource attackAudio;

    private void Tick()
    {
        currentAttackTimeout += Time.deltaTime;
        timeSinceLastJump += Time.deltaTime;
    }

    public override void OnEnable()
    {
        base.OnEnable();

        currentAttackTimeout = 0.0f;
        timeSinceLastJump = timeBetweenJumps;
    }

    protected override void Wander()
    {
        base.Wander();

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

        HandleMovementAnim();
        Tick();
    }

    protected override void Chase()
    {
        base.Chase();

        timeSinceLastSeenPlayer = 0.0f;

        if (CanAttackPlayer())
        {
            if (currentAttackTimeout >= attackTimeout)
            {
                Attack();
            }

        }
        else
        {
            HandleMovementAnim();
        }

        if (timeSinceLastJump >= timeBetweenJumps && !isJumping)
        {
            Jump();
        }

        Tick();
    }

    private void Jump()
    {
        print("Jumping!");

        PlayAnimation("Attack_4", true);

        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        Rigidbody rb = GetComponent<Rigidbody>();
        agent.ResetPath();
        agent.enabled = false;

        rb.isKinematic = false;
        rb.useGravity = true;

        rb.AddForce((player.transform.position - this.transform.position).normalized * jumpForceForward, ForceMode.Impulse);
        rb.AddForce(Vector3.up * jumpForceUpward, ForceMode.Impulse);

        timeSinceLastJump = 0.0f;
        isJumping = true;
    }

    protected override void StateReturn()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        goal = manager.enemySpawnPosition.transform.position;
        agent.destination = goal;
        hasReachedInitialGoal = false;

        if (CanSeePlayer())
        {
            PlaySpottedNoise();
            currentState = State.CHASE;
        }

        if (Vector3.Distance(transform.position, manager.enemySpawnPosition.transform.position) <= 5.0f)
        {
            gameObject.SetActive(false);
            manager.chasersActive -= 1;
        }

        HandleMovementAnim();
        Tick();
    }

    protected override void Attack()
    {
        // attack player!
        // print("i am attacking the player");
        PlayAnimation("Attack_1", true);

        attackAudio.Play();

        HealthSystem.OnDamageTaken(25);

        currentAttackTimeout = 0.0f;
    }

    private bool CanAttackPlayer()
    {
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);

        float distance = Vector2.Distance(vec2Player, vec2Us);

        return (distance <= attackDistance);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (isJumping)
        {
            print("Landed!");
            isJumping = false;

            NavMeshAgent agent = GetComponent<NavMeshAgent>();
            agent.enabled = true;

            Rigidbody rb = GetComponent<Rigidbody>();
            rb.isKinematic = true;
            rb.useGravity = false;
        }
    }
}
