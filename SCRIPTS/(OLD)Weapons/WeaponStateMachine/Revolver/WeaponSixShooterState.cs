using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponSixShooterState : WeaponBaseState
{
    public GameObject WeaponPrefab;
    float damage = 10.0f;

    public override void EnterState(WeaponStateManager weapon)
    {
        Debug.Log("Welcome to the Six Shooter State");
    }
    
    public override void UpdateState(WeaponStateManager weapon)
    {

    }

    public override void PrimaryFire(WeaponStateManager weapon)
    {
        
    }
    public override void Reload(WeaponStateManager weapon)
    {
        
    }

    public override void AccessEnemyScript(WeaponStateManager weapon, RaycastHit hit)
    {
        if (hit.transform.tag == "Enemy")
        {
            EnemyBaseScript.Enemy enemyBaseScript = hit.transform.GetComponent<EnemyBaseScript.Enemy>();
            enemyBaseScript.OnHit(weapon, damage);
        }
    }
}
