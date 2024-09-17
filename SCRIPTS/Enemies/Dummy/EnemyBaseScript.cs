using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBaseScript
{
    public interface Enemy
    {
        public abstract void OnHit(WeaponStateManager weapon, float hitDamage);
    }
}
