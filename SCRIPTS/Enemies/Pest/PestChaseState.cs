using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using PestBehavior;

public class PestChaseState : PestBaseState
{
    public GameObject chaseTarget;
    NavMeshAgent agent;
    Vector3 targetPos;
    float StoppingDistance = 7f;
    float MoveSpeed = 10f;
    float Acceleration = 8f;

    public override void EnterState(PestStateHandler pestHandler)
    {
        agent = pestHandler.pestAgent;

        agent.stoppingDistance = StoppingDistance;
        agent.speed = MoveSpeed;
        agent.acceleration = Acceleration;

        Chase(pestHandler);
    }

    public override void UpdateState(PestStateHandler pestHandler)
    {
        Chase(pestHandler);
        LookAtTarget(pestHandler);
        targetPos = pestHandler.PlayerPos;
    }

    void Chase(PestStateHandler pestHandler)
    {
        if (chaseTarget)
        {
            float distance = Vector3.Distance(pestHandler.PlayerPos, pestHandler.transform.position);
            if (distance < pestHandler.DetectionRange)
            {
                pestHandler.SwitchStates(pestHandler.AttackState);
            }
            else
            {
                agent.SetDestination(pestHandler.PlayerPos);
            }
        }
    }

    void LookAtTarget(PestStateHandler pestHandler)
    {
        if (pestHandler)
        {
            pestHandler.transform.LookAt(chaseTarget.transform.position);
            //pestHandler.transform.rotation = Quaternion.LookRotation(targetPos);
        }
    }
}
