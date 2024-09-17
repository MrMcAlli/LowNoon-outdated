using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatsClass
{
    public float MaxHealth;
    public float Health;
    public float MoveSpeed;
    public float HoverDistance;
}

public interface EnemyStats
{
    void TakeDamage(float damage, Collider hitCollider);
}
