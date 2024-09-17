using UnityEngine;
using UnityEngine.InputSystem;

public class WeaponStateManager : MonoBehaviour
{
///////////////////////////////////////////////Variables\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    //Player
    public GameObject Player;

    //Camera
    public Camera PlayerCam;

    //Crosshair
    public GameObject crosshair;

    //State Machine Variables
    WeaponBaseState currentState;
    WeaponBaseState lastState;
    public WeaponMeleeState MeleeState = new WeaponMeleeState();
    public WeaponMachineGunState MachineGunState = new WeaponMachineGunState();
    public WeaponSixShooterState SixShooterState = new WeaponSixShooterState();
    
    //Input Variables
    PlayerInput playerInput;
    public InputAction PrimaryFireAction;
    public InputAction ReloadAction;
    public InputAction SwapSlot1;
    public InputAction SwapSlot2;
    public InputAction SwapSlot3;
    public InputAction SwapLastAction;
    public bool PrimaryFireDown;

    //Prefabs

        //Weapon Prefabs
        public GameObject MeleePrefab;
        public GameObject MachineGunPrefab;
        public GameObject currentWeaponModel;

        //WeaponFX Prefabs
        public GameObject MG_MuzzleFlashPrefab;
        public GameObject MG_BulletHitPrefab;
        public GameObject MG_BulletMissPrefab;

    //End prefabs

    //Current Weapon Variables
    public int currentAmmo;

///////////////////////////////////////////////Script Begin\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\


    void Start()
    {
        GetComponents();
        //Starting the State Machine
        //Base State -->  ↓ ↓ ↓ ↓
        currentState = MachineGunState;
        currentState.EnterState(this);
    }

    private void GetComponents()
    {
        //Setting input Variables
        playerInput = GetComponent<PlayerInput>();
        PrimaryFireAction = playerInput.actions["Primary Fire"];
        ReloadAction = playerInput.actions["Reload"];
        SwapLastAction = playerInput.actions["Last Weapon"];
        SwapSlot1 = playerInput.actions["Slot 1"];
        SwapSlot2 = playerInput.actions["Slot 2"];
        SwapSlot3 = playerInput.actions["Slot 3"];
        
        //Setting Player
        Player = GameObject.FindWithTag("Player");

        //Setting Camera
        PlayerCam = this.transform.parent.GetComponent<Camera>();

        //Setting Crosshair
        crosshair = GameObject.Find("Crosshair");
    }

    //Allows me to swap a state by calling this method and passing a state as an arguement
    //Ex: To swap to the melee state call SwitchStates(MeleeState);
    public void SwitchStates(WeaponBaseState state)
    {
        //Setting the last state
        lastState = currentState;

        //Destroy weapon prefabs and switch states
        Destroy(currentWeaponModel);
        currentState = state;
        state.EnterState(this);
    }

    void PlayerWeaponInput()
    {
        ReloadAction.performed += ctx => currentState.Reload(this);
        //This method constantly checks if the player is swapping weapons or firing
        SwapLastAction.performed += ctx => SwitchStates(lastState);
        SwapSlot1.performed += ctx => SwitchStates(MeleeState);
        SwapSlot2.performed += ctx => SwitchStates(MachineGunState);
        SwapSlot3.performed += ctx => SwitchStates(SixShooterState);

        CheckPrimaryFire();
    }

    void CheckPrimaryFire()
    {
        //For full auto weapons, I am checking if the button is currently held down or not, instead of seeing if the player has clicked
        //If the player is not currently equipping an automatic weapon then it will perfrom as normal
        if (currentState == MachineGunState)
        {
            PrimaryFireAction.performed += _ => PrimaryFireDown = true;
            PrimaryFireAction.canceled += _ => PrimaryFireDown = false;
            if (PrimaryFireDown)
            {
                currentState.PrimaryFire(this);
            }
        }
        else
        {
            PrimaryFireAction.performed += ctx => currentState.PrimaryFire(this);
        }
    }

    ///////////////////////////////////////////////////////////////////////////////Pucnch\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    
    private float n = 0f;

    //private float lastCalled = 0f;
    //private float currentTime = 0f;
    private float FocusMeterValue()
    {
        //Raycast

        //if tag == enemy then

        RaycastHit hit;
        if (Physics.Raycast(PlayerCam.transform.position, PlayerCam.transform.forward, out hit, Mathf.Infinity))
        {
            if (hit.transform.tag == "Test Subject")
            {
                n += Time.deltaTime;



                Debug.Log(hit);


                // float difference = currentTime - lastCalled;
                // if (difference >= 0.1f)
                // {
                //     n += Time.deltaTime;
                // }
                // else
                // {
                //     n = Time.deltaTime;
                //     currentTime = Time.time;
                //     lastCalled = currentTime;
                // }
            }
            else
            {
                n = 0f;
            }
        }

        return n;

        //get the time between now (t) and the last time this function was called (lastCalled)
        //if lastCalled is less than a threshold (x) (~0.1 seconds?) add the difference.
    }
    
    
    void Update()
    {
        currentState.UpdateState(this);
        PlayerWeaponInput();
        MachineGunState.CheckGunRecoil();

        //Current Weapon variables
        currentAmmo = currentState.AmmoCount;

        //Punch
        Debug.Log("Focus Meter: "+FocusMeterValue());
    }
}
