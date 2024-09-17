using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PestBehavior;

public class PestDamagedState : PestBaseState
{
    GameObject DeathVFX_Prefab;
    GameObject DeathVFX;
    GameObject playerObject;
    List<GameObject> neighbors = new List<GameObject>();
    public override void EnterState(PestStateHandler pestHandler)
    {
        DeathVFX_Prefab = pestHandler.DeathVFX_Prefab;
    }

    public void DealDamage(float damage, PestStateHandler pestHandler)
    {
        if (damage < pestHandler.Health)
        {
            pestHandler.Health -= damage;
            pestHandler.Alert((Vector3.Distance(pestHandler.transform.position, pestHandler.PlayerObject.transform.position)));
            pestHandler.SwitchStates(pestHandler.lastState);
        } 
        else
        {
            Die(pestHandler);
        }
    }

    public void Die(PestStateHandler pestHandler)
    {
        GameObject.Destroy(pestHandler.gameObject);
        Debug.Log(DeathVFX_Prefab);
        DeathVFX = GameObject.Instantiate(DeathVFX_Prefab, pestHandler.pestBody.transform.position, Quaternion.identity);
    }

    public void Explode(PestStateHandler pestHandler)
    {
        pestHandler.GetNeighbors(5.5f);
        neighbors = pestHandler.neighborList;
        for (var i = 0; i < neighbors.Count; i++)
        {
            PestStateHandler pestStateHandler = neighbors[i].GetComponent<PestStateHandler>();
            pestStateHandler.OnCrit();
        }
    }

    public override void UpdateState(PestStateHandler pestHandler)
    {

    }
}
