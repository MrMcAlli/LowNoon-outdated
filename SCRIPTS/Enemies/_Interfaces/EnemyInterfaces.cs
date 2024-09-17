using System.Collections;
using System.Collections.Generic;
using UnityEngine;

    public interface IDamageHandler
    {
        void TakeDamage(float damage, Collider hitCollider);
    }

    public interface INeighborhoodWatch
    {
        void Alert(float dist);
    }
