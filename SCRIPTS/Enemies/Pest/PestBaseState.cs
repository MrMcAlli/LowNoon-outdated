using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PestBehavior;

public abstract class PestBaseState
{
    public abstract void EnterState(PestStateHandler pestHandler);
    public abstract void UpdateState(PestStateHandler pestHandler);
}
