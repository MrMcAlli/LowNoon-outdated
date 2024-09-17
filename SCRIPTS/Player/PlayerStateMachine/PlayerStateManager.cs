using UnityEngine;
using UnityEngine.InputSystem;
using System.Collections;
using UnityEngine.UI;
using Cinemachine;

//Requiring necessary components for player functions and input
[RequireComponent(typeof(CharacterController), typeof(PlayerInput))]

public class PlayerStateManager : MonoBehaviour, IDamageHandler
{

    ////////////////////////////////////////////////////////////Variables\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    //Character Controller
    public CharacterController Controller;


    //Camera
    public Camera PlayerCamera;
    public CinemachineVirtualCamera vCam;
    public GameObject headPoint;


    //WalkState Variables


    //DashState Variables
    public int NumDashes = 1;
    private bool canDash = true;

    //Jump Variables
    public float JumpHeight;

    public int NumJumps;

    //IdleState Variables


    //General Movement Variables
    private bool groundedPlayer;
    public float playerBaseSpeed = 11f;
    public float playerSpeed = 11f;
    public Vector3 PlayerVelocity;
    public float GravityValue;

    //General Input Variables
    PlayerInput playerInput;
    public InputAction MoveAction;
    public InputAction JumpAction;
    public InputAction DashAction;
    public Vector2 MoveInputValue;

    //State Machine Variables
    PlayerBaseState currentState;
    public PlayerIdleState IdleState = new PlayerIdleState();
    public PlayerWalkState WalkState = new PlayerWalkState();
    public PlayerDashState DashState = new PlayerDashState();
   
    //Stats
    private float MaxHealth = 10000f;
    private float Health;

    //UI
    public Texture2D DashIcon_Ready;
    public Texture2D DashIcon_NotReady;
    public RawImage DashIcon;





    ///////////////////////////////////////////////////////Script Begin\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


    void Start()
    {
        //Locking player cursor
        Cursor.lockState = CursorLockMode.Locked;

        //starting the state machine (with the idle state)
        currentState = IdleState;
        currentState.EnterState(this);

        //Getting necessary components and assigning them : )
        GetComponents();
        Health = MaxHealth;
    }

    public void SwitchStates(PlayerBaseState state)
    {
        //Updating the state machine's state
        currentState = state;
        state.EnterState(this);
    }

    void GetComponents()
    {
        //Camera
        PlayerCamera = transform.Find("Main Camera").GetComponent<Camera>();
        vCam = this.GetComponent<CinemachineVirtualCamera>();


        //Player Input variables :)
        Controller = GetComponent<CharacterController>();
        playerInput = GetComponent<PlayerInput>();
        MoveAction = playerInput.actions["Move"];
        MoveInputValue = MoveAction.ReadValue<Vector2>();
        JumpAction = playerInput.actions["Jump"];
        DashAction = playerInput.actions["Dash"];

        //Player Movement variables 
        JumpHeight = 3f;
        GravityValue = -85f;
        NumJumps = 2;
    }

    public void TakeDamage(float damage, Collider hitCollider)
    {
        if (damage < Health)
        {
            Health -= damage;
        }
        else
        {
            Die();
        }
    }

    void Die()
    {
        Destroy(this.gameObject);
    }

    void UpdatePlayerValues()
    {
        //Updating Player Input Value
        MoveInputValue = MoveAction.ReadValue<Vector2>();

        //Updating Player Rotation
        gameObject.transform.rotation = Quaternion.Euler(new Vector3(0,PlayerCamera.transform.eulerAngles.y, 0));
    }

///////////////////////////////////////////////////Movement, Jump, and Gravity\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


    //Handling the Jump state from the State Manager lets the player jump no matter what state they are in
    //TLDR; I am too lazy to learn how to + create a hierarchical state machine 
    //Basically I will be handling things that are needed to be done regardless of state through the StateManager Examples: Jumping, ?
    void JumpAndGravity()
    {
        //Checking for player grounded state
        groundedPlayer = Controller.isGrounded;

        //Checking player input and jump count
        if (JumpAction.triggered && NumJumps > 0)
        {
            if (currentState == DashState)
            {
                JumpHeight = 10f;
                playerSpeed += 18f;
            }
            else
            {
                JumpHeight = 3f;
                playerSpeed += 2f;
            }
            //Removing a jump from the jumpcounter and increasing the player speed when jumping
            NumJumps--;

            //Applying jump velocity
            PlayerVelocity.y = Mathf.Sqrt(JumpHeight * -3.0f * GravityValue);
        }
        if (groundedPlayer && PlayerVelocity.y < 0)
        {
            //Resetting jump variables once the player lands
            NumDashes = 1;
            NumJumps = 2;
            PlayerVelocity.y = 0f;
            playerSpeed = playerBaseSpeed;
        }
        PlayerVelocity.y += GravityValue * Time.deltaTime;
        //Compiling Move Values and moving the character
        Controller.Move(WalkState.move.normalized * Time.deltaTime * playerSpeed);
        //Compiling Jump Values and jumping the character
        Controller.Move(PlayerVelocity * Time.deltaTime);
    }

    void DoCameraLean()
    {
        //Side Lean
        if (WalkState.MoveValue.x > 0.5f)
        {
            Debug.Log(vCam.AddCinemachineComponent<CinemachinePOV>().m_HorizontalAxis);
        }
        else if (WalkState.MoveValue.x < -0.5f)
        {
            
        }

    }

////////////////////////////////////////////////////// Dash \\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\

    void CheckDash()
    {
        //Pretty straight forward
        if (DashAction.triggered && canDash)
        {
            SwitchStates(DashState);
            StartCoroutine(DashCooldown());
        } 
    }

    IEnumerator DashCooldown()
    {
        //Also pretty straight forward, starts a quick timer that can be changed and stops the coroutine when its finished
        canDash = false;
        DashIcon.GetComponent<RawImage>().texture = DashIcon_NotReady;
        yield return new WaitForSeconds(1.5f);
        canDash = true;
        DashIcon.GetComponent<RawImage>().texture = DashIcon_Ready;
        StopCoroutine(DashCooldown());
    }

    void Update()
    {
        //Updating the current state by calling its "UpdateState" method, anything in that method will run every frame as long as the script is active
        currentState.UpdateState(this);

        //Calling all StateMachine methods
        UpdatePlayerValues();
        JumpAndGravity();
        CheckDash();
        DoCameraLean();
    }
}
