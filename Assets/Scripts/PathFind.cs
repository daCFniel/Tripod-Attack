using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class PathFind : MonoBehaviour
{
    public Vector3 goal;
    private GameObject player;
    private Light light;

    public float wanderDistMax = 250.0f;
    public float wanderDistMin = 150.0f;
    public float distToStop = 2.5f;

    private void Start()
    {
        player = GameObject.FindGameObjectsWithTag("Player")[0];
        newGoal();
        light = this.transform.Find("Light").GetComponent<Light>();
    }

    void Update()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = goal;

        Vector2 vec2goal = new Vector2(goal.x, goal.z);
        Vector2 vec2Player = new Vector2(player.transform.position.x, player.transform.position.z);
        Vector2 vec2Us = new Vector2(transform.position.x, transform.position.z);
        Vector2 vec2Forward = new Vector2(transform.forward.x, transform.forward.z);

        print(agent.pathStatus);
        print(agent.remainingDistance);

        if (agent.remainingDistance <= distToStop)
        {
            newGoal();
        }

       // if (agent.pathStatus == NavMeshPathStatus.Path)
       // {
       //     newGoal();
       // }

        Vector2 playerDirection = vec2Player - vec2Us;
        float angle = Vector2.Angle(playerDirection, vec2Forward);

        if (angle <= 25.0f)
        {
            light.color = Color.red;
        }
        else
        {
            light.color = Color.white;
        }
    }

    private Vector2 randomPointInRadius()
    {
        return new Vector2(Random.Range(wanderDistMin, wanderDistMax), Random.Range(wanderDistMin, wanderDistMax));
    }

    private void newGoal()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();

        Vector2 circle = Random.insideUnitCircle.normalized + randomPointInRadius();

        goal = new Vector3(
            transform.position.x + circle.x,
            transform.position.y,
            transform.position.z + circle.y
        );
        
    }
}
