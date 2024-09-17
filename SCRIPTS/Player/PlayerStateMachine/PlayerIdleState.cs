using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerIdleState : PlayerBaseState
{
    public override void EnterState(PlayerStateManager player)
    {

    }
    
    public override void UpdateState(PlayerStateManager player)
    {
        //If the player's Move input beings, the state will switch from Idle to Walk
        if (player.MoveInputValue.magnitude > 0f)
        {
            player.SwitchStates(player.WalkState);
        } else
        {
            //Idle
        }
    }
}
