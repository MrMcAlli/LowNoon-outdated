using PestBehavior;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PestAnimatorEvents : MonoBehaviour
{
    PestStateHandler pestHandler;
    void Awake()
    {
        pestHandler = transform.parent.transform.GetComponent<PestStateHandler>();
    }
    public void AttackFXStart()
    {
        pestHandler.LaunchMissile();
    }
    
}
