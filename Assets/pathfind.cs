using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class pathfind : MonoBehaviour
{
    public Transform goal;

    void Update()
    {
        NavMeshAgent agent = GetComponent<NavMeshAgent>();
        agent.destination = GameObject.FindGameObjectsWithTag("Player")[0].transform.position;
    }
}
