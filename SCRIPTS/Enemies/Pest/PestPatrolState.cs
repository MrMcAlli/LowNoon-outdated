using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PestBehavior;

public class PestPatrolState : PestBaseState
{
    NavMeshAgent agent;
    public bool needNewPos;
    bool needNewTimer;
    Vector3 nextPos;
    Vector3 currentPos;
    float patrolDistance = 5f;
    float stopTime = 1.25f;
    float StoppingDistance = 3f;
    Collider[] nearbyObjects;

    public override void EnterState(PestStateHandler pestHandler)
    {

        agent = pestHandler.pestAgent;


        needNewPos = true;
        CheckNextMove(pestHandler);
    }

    

    public override void UpdateState(PestStateHandler pestHandler)
    {
        Movement(pestHandler);
        SearchForTarget(pestHandler);
    }


/////////////////////////////////////////////////////////////////Movement\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    void Movement(PestStateHandler pestHandler)
    {
        if (agent.isOnNavMesh)
        {
            CheckNextMove(pestHandler);
            CheckDistanceToMove(pestHandler);
            DoMove(pestHandler);
        }
    }

    void CheckNextMove(PestStateHandler pestHandler)
    {
        if (needNewPos && !agent.hasPath)
        {
            nextPos = pestHandler.transform.position + Random.insideUnitSphere * patrolDistance;
            nextPos.y = 4f;
            needNewPos = false;
        }
    }

    void CheckDistanceToMove(PestStateHandler pestHandler)
    {
        bool crtnRunning = pestHandler.coroutineRunning;
        if (agent.remainingDistance < StoppingDistance && !crtnRunning)
        {
            pestHandler.StartCoroutine(pestHandler.PatrolTimer(stopTime));
        }
    }

    void DoMove(PestStateHandler pestHandler)
    {
        agent.destination = nextPos;
    }

////////////////////////////////////////////////////////////Enemy Detection\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    void SearchForTarget(PestStateHandler pestHandler)
    {
        nearbyObjects = Physics.OverlapSphere(pestHandler.transform.position, pestHandler.DetectionRange);

        foreach (Collider collider in nearbyObjects)
        {
            if (collider.tag == "Player")
            {
                if (Vector3.Distance(pestHandler.transform.position, collider.transform.position) <= 25f)
                {
                    pestHandler.SwitchStates(pestHandler.AttackState);
                }
                pestHandler.Alert(Vector3.Distance(pestHandler.transform.position, collider.transform.position));
            }
        }
    }

}
