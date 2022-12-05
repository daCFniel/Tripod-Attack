using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIAlienChaser : PathFind
{
    public float attackDistance = 2.0f;
    public float attackTimeout = 1.25f; // This is from length of animation
    private float currentAttackTimeout = 0.0f;

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
        currentAttackTimeout += Time.deltaTime;
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
        currentAttackTimeout += Time.deltaTime;
    }

    protected override void StateReturn()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        goal = manager.enemySpawnPosition.transform.position;
        agent.destination = goal;
        hasReachedInitialGoal = false;

        if (CanSeePlayer())
        {
            currentState = State.CHASE;
        }

        if (Vector3.Distance(transform.position, manager.enemySpawnPosition.transform.position) <= 5.0f)
        {
            gameObject.SetActive(false);
            manager.chasersActive -= 1;
        }

        HandleMovementAnim();
        currentAttackTimeout += Time.deltaTime;
    }

    protected override void Attack()
    {
        // attack player!
        print("i am attacking the player");
        PlayAnimation("Attack_1", true);
        currentAttackTimeout = 0.0f;
    }

    private bool CanAttackPlayer()
    {
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);

        float distance = Vector2.Distance(vec2Player, vec2Us);

        return (distance <= attackDistance);
    }
}
