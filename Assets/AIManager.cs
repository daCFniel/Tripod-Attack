using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class AIManager : MonoBehaviour
{
    public float scoutBaseSpeed = 3.5f;
    public float chaserBaseSpeed = 6.0f;
    public GameObject enemySpawnPosition;
    public GameObject prefabEnemyChaser;

    public int chaserCount = 10;
    private bool chasersAreActive = false;

    private void Start()
    {
        // ChangeSpeedMultiplier(5.0f);
        // SpawnEnemy();
        foreach (Transform child in transform)
        {
            if (child.CompareTag("EnemyScout"))
            {
                SetAgentSpeed(child, scoutBaseSpeed);
            }

            if (child.CompareTag("EnemyChaser"))
            {
                SetAgentSpeed(child, chaserBaseSpeed);
            }
        }
    }

    private void SetAgentSpeed(Transform agentTransform, float speedMult)
    {
        NavMeshAgent agent = agentTransform.GetComponent<NavMeshAgent>();

        agent.speed = scoutBaseSpeed;

        // Keep acceleration relative to speed.
        agent.acceleration = scoutBaseSpeed * (8f / 3.5f);
    }

    private void SpawnEnemy()
    {
        GameObject enemy = Instantiate(prefabEnemyChaser, transform);
        SetAgentSpeed(enemy.transform, chaserBaseSpeed);
        enemy.transform.position = enemySpawnPosition.transform.position;
    }

    public void SendChasers(Vector3 playerPosition)
    {
        if (!chasersAreActive)
        {
            SpawnChasers();
        }

        // AI
        UpdatePlayerLocationInChasers(playerPosition);
    }

    private void SpawnChasers()
    {
        chasersAreActive = true;

        for (int i = 0; i < chaserCount; i++)
        {
            SpawnEnemy();
        }
    }

    private void UpdatePlayerLocationInChasers(Vector3 playerPos)
    {
        foreach (Transform child in transform)
        {
            if (!child.CompareTag("EnemyChaser"))
            {
                continue;
            }

            child.GetComponent<PathFind>().goal = playerPos;
        }
    }
}
