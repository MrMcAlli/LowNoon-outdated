using System.Runtime.InteropServices;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerWalkState : PlayerBaseState
{
    //Controller
    CharacterController controller;

    //Camera
    Camera PlayerCamera;
    Transform cameraTransform;

    //Movement
    public Vector3 MoveValue;
    public Vector3 move;



    public override void EnterState(PlayerStateManager player)
    {
        GetComponents(player);
    }
    
    public override void UpdateState(PlayerStateManager player)
    {
        //Moving player and checking if the player is moving or not, if the player is not moving the Idle state will become active
        MovePlayer(player);

        if (player.MoveInputValue.magnitude == 0f)
        {
            player.SwitchStates(player.IdleState);
        }
    }

    void GetComponents(PlayerStateManager player)
    {
        PlayerCamera = player.PlayerCamera;
        cameraTransform = PlayerCamera.transform;
        controller = player.Controller;
    }

    public void MovePlayer(PlayerStateManager player)
    {
        //Getting player input and setting the values of input variables
        Vector2 input = player.MoveInputValue;
        MoveValue = new Vector3(input.x, 0, input.y);

        //Getting Movement Values to be collected by the main script
        move = (cameraTransform.forward * MoveValue.z + cameraTransform.right * MoveValue.x);
        move.y = 0f;
    }
}
