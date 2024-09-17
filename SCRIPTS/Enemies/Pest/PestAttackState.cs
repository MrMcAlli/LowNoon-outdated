using System.Threading;
using System.Xml.Schema;
using System;
using System.Collections;
using UnityEngine;
using PestBehavior;
public class PestAttackState : PestBaseState
{
    public bool canAttack;
    float attackCD = 6f;
    float distToTarget;
    GameObject AttackFX_Prefab;

    GameObject AttackMissileFX_Prefab;
    GameObject MissileOBJ;
    GameObject MuzzFlashOBJ;
    GameObject AttackChargeFX_Prefab;
    GameObject AttackEndChargeFX_Prefab;
    GameObject ChargeFXOBJ;
    GameObject EndChargeFXOBJ;
    Rigidbody rb_Missile;
    public override void EnterState(PestStateHandler pestHandler)
    {
        AttackFX_Prefab = pestHandler.AttackFX_Prefab;
        AttackMissileFX_Prefab = pestHandler.AttackMissileFX_Prefab;
        AttackChargeFX_Prefab = pestHandler.AttackChargeFX_Prefab;
        AttackEndChargeFX_Prefab = pestHandler.AttackEndChargeFX_Prefab;
    }

    void Attack(PestStateHandler pestHandler)
    {
        if (canAttack && !pestHandler.coroutineRunning)
        {
            canAttack = false;
            pestHandler.StartCoroutine(pestHandler.AttackCooldown(attackCD));
            Debug.Log("I: "+pestHandler.transform.name+"ATTACKED ! ! !");
            AttackAnimation(pestHandler);
        }
    }

    void AttackAnimation(PestStateHandler pestHandler)
    { 
            pestHandler.animator.enabled = true;
            pestHandler.animator.Play("Open");
    }

    public void ProjectileHandler(PestStateHandler pestHandler)
    {
        MuzzFlashOBJ = GameObject.Instantiate(AttackFX_Prefab, pestHandler.pestBody.transform.position, Quaternion.identity);
        MuzzFlashOBJ.transform.LookAt(pestHandler.PlayerObject.transform);
        MuzzFlashOBJ.transform.SetParent(pestHandler.transform);
        GameObject.Destroy(MuzzFlashOBJ, 2f);

        MissileOBJ = GameObject.Instantiate(AttackMissileFX_Prefab, pestHandler.pestBody.transform.position, Quaternion.identity);
        MissileOBJ.transform.LookAt(pestHandler.PlayerObject.transform);
        GameObject.Destroy(MissileOBJ, 2f);
    }

    void CheckCanAttack(PestStateHandler pestHandler)
    {
        distToTarget = Vector3.Distance(pestHandler.transform.position, pestHandler.PlayerObject.transform.position);
        if (distToTarget < pestHandler.DetectionRange)
        {
            pestHandler.pestAgent.isStopped = true;
            canAttack = true;
            Attack(pestHandler);
                   
        }
        else
        {
            pestHandler.pestAgent.isStopped = false;
            canAttack = false;
            //pestHandler.EndAttack();
            pestHandler.SwitchStates(pestHandler.ChaseState);
            pestHandler.ChaseState.chaseTarget = pestHandler.PlayerObject;
        }
    }

    public void StopChargeFX(PestStateHandler pestHandler)
    {
        GameObject.Destroy(ChargeFXOBJ);
        EndChargeFXOBJ = GameObject.Instantiate(AttackEndChargeFX_Prefab, pestHandler.pestBody.transform.position, Quaternion.identity);
        EndChargeFXOBJ.transform.SetParent(pestHandler.transform); 
        GameObject.Destroy(EndChargeFXOBJ, 1f);
    }

    public override void UpdateState(PestStateHandler pestHandler)
    {
        if (pestHandler)
        {
            pestHandler.transform.LookAt(pestHandler.ChaseState.chaseTarget.transform);
            
            //pestHandler.transform.eulerAngles = pestHandler.transform.eulerAngles - pestHandler.ChaseState.chaseTarget.transform.eulerAngles;

            CheckCanAttack(pestHandler);
        }

    }
}
