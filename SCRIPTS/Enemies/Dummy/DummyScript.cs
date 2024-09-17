using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DummyScript : MonoBehaviour, EnemyBaseScript.Enemy
{
    bool beenDamaged;
    public float MaxHP;
    public float currentHP;

    void Start()
    {
        MaxHP = 1000f;
        currentHP = MaxHP;
    }
    public void OnHit(WeaponStateManager weapon, float hitDamage)
    {
        if (beenDamaged == false)
        {
            beenDamaged = true;
            currentHP -= hitDamage;
            if( currentHP <= 0)
            {
                Destroy(gameObject);
            } else
            {
                beenDamaged = false;
            }
            Debug.Log(currentHP);
        }
    }
}
