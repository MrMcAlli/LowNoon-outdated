using UnityEngine;
using UnityEngine.InputSystem;

public abstract class PlayerBaseState
{
    //Setting up the base functions for the player state scripts
    public abstract void EnterState(PlayerStateManager player);
    
    public abstract void UpdateState(PlayerStateManager player);
}
