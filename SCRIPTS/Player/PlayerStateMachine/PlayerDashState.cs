using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Cinemachine;

public class PlayerDashState : PlayerBaseState
{
    //////////////////////////////////////////////////////Code Description Below\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //Script Called in mother script --> Takes current movement position --> Starts State Timer --> Begins Dashing --> Timer Ends, Stops Dashing --> Leave
    //



    PlayerWalkState WalkState;
    PlayerStateManager playerManager;

    CinemachineVirtualCamera vcam;

    GameObject headPoint;

    float dashTime = 0.35f;
    float elapsedTime = 0.0f;
    float dashSpeed = 60f;
    Vector3 dashDirection;



    public override void EnterState(PlayerStateManager player)
    {
        WalkState = player.WalkState;
        playerManager = player;
        dashDirection = player.WalkState.move;
        player.NumDashes -= 1;
        headPoint = player.headPoint;

        while (vcam = null)
        {
            vcam = player.vCam;
        }

    }

    void Dash(PlayerStateManager player)
    {
        //1st Run, Begins timer & dash
        //After Timer is up, the timer will reset and the player will switch back to the Walk State
        if (elapsedTime < dashTime)
        {
            elapsedTime += Time.deltaTime;
            player.Controller.Move(dashDirection * dashSpeed * Time.deltaTime);
        }
        else
        {
            elapsedTime = 0f;
            headPoint.transform.eulerAngles = new Vector3(0f, 0f, 0f);
            player.SwitchStates(WalkState);
        }
    }
    public override void UpdateState(PlayerStateManager player)
    {
        Dash(player);
    }
}
