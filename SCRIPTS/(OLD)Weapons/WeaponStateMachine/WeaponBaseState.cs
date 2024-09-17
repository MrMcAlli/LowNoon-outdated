using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBaseState
{
    public int AmmoCount;

    public abstract void EnterState(WeaponStateManager weapon);
    
    public abstract void UpdateState(WeaponStateManager weapon);

    public abstract void PrimaryFire(WeaponStateManager weapon);
    public abstract void Reload(WeaponStateManager weapon);
    public abstract void AccessEnemyScript(WeaponStateManager weapon, RaycastHit hit);
}
