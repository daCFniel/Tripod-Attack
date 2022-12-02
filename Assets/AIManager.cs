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
    public int chasersActive = 0;

    private GameObject[] chasers;

    private void Start()
    {
        // ChangeSpeedMultiplier(5.0f);
        // SpawnEnemy();

        chasers = new GameObject[chaserCount];
        PopulateChaserArray();

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

    private void PopulateChaserArray()
    {
        for (int i = 0; i < chaserCount; i++)
        {
            GameObject enemy = Instantiate(prefabEnemyChaser, transform);
            enemy.SetActive(false);
            SetAgentSpeed(enemy.transform, chaserBaseSpeed);
            chasers[i] = enemy;
        }
    }

    private void SetAgentSpeed(Transform agentTransform, float speedMult)
    {
        NavMeshAgent agent = agentTransform.GetComponent<NavMeshAgent>();

        agent.speed = scoutBaseSpeed;

        // Keep acceleration relative to speed.
        agent.acceleration = scoutBaseSpeed * (8f / 3.5f);
    }

    private void SpawnEnemy(int index)
    {
        GameObject enemy = chasers[index];
        enemy.transform.position = enemySpawnPosition.transform.position;
        enemy.SetActive(true);
        chasersActive += 1;
    }

    public void SendChasers(Vector3 playerPosition)
    {
        if (chasersActive != chaserCount)
        {
            SpawnChasers();
        }

        // AI
        UpdatePlayerLocationInChasers(playerPosition);
    }

    private void SpawnChasers()
    {
        for (int i = 0; i < chaserCount; i++)
        { 
            GameObject enemy = chasers[i];

            if (enemy.activeInHierarchy == true) continue;

            SpawnEnemy(i);
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
            child.GetComponent<PathFind>().startPos = playerPos;
            child.GetComponent<PathFind>().lastSeenPlayerPos = playerPos;

            child.GetComponent<PathFind>().currentState = PathFind.State.WANDER;
            child.GetComponent<PathFind>().timeSinceLastSeenPlayer = 0.0f;
            child.GetComponent<PathFind>().hasReachedInitialGoal = false;
        }
    }
}
