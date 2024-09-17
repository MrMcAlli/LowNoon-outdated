using System.ComponentModel;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WeaponMeleeState : WeaponBaseState
{
    GameObject weaponPrefab;
    GameObject meleeObject;
    Vector3 weaponPosition;
    float damage = 10f;
    public override void EnterState(WeaponStateManager weapon)
    {
        Debug.Log("Welcome to the Melee State");

        GetComponents(weapon);

        if (!weaponPrefab.scene.IsValid())
        {

            meleeObject = GameObject.Instantiate(weaponPrefab, weapon.transform.position, weapon.transform.rotation);
            meleeObject.transform.SetParent(weapon.transform);
            weapon.currentWeaponModel = meleeObject; 
        }
    }

    void GetComponents(WeaponStateManager weapon)
    {
        weaponPosition = new Vector3(0,0,0);
        weaponPrefab = weapon.MeleePrefab;
    }
    
    public override void UpdateState(WeaponStateManager weapon)
    {

    }

    public override void PrimaryFire(WeaponStateManager weapon)
    {
        Debug.Log("Melee'd");
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
