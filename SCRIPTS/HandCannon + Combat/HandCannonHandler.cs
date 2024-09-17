using System.Collections;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
public class HandCannonHandler : MonoBehaviour
{
    
    //  Weapon Stats  \\
    float Damage = 33.34f;
    public int currentAmmo;
    int MaxAmmo = 7;
    [SerializeField] public bool CanShoot = true;
    //End Weapon Stats
    public Camera cam;
    public LayerMask playerLayer;
    public GameObject Player;
    private CharacterController pController;
    private PlayerStateManager playerStateManager;
    private InputAction PFireAction;
    private InputAction PunchAction;
    private InputAction ReloadAction;
    PlayerInput playerInput;

    //UI
    public Scrollbar FocusMeterUI;

    //Animation
    public Animator gunAnimator;
    private string ANIM_FIRE = "Shoot";

    //VFX
    public GameObject MuzzleFX_Gun;
    public GameObject MuzzleFX_Laser;


    //Punch
    private GameObject focusTarget;
    public float LastFocusValue;

    
    void OnEnable()
    {
        //To Do:
        //Alt Fire Hold Interaction: Charged shot
        //Alt Fire Tap Interaction: Tap up to (currentAmmo) times at the end of a (timer) second timer an Overwatch McCree-eqsue aimbot interaction will begin
        PFireAction.performed += _ => Shoot();
        ReloadAction.performed += _ => DoReload();
        PunchAction.performed += _ => Punch();
    }

    void OnDisable()
    {
        PFireAction.performed -= _ => Shoot();
        ReloadAction.performed -= _ => DoReload();
        PunchAction.performed -= _ => Punch();
    }

    void Awake()
    {
        SetValues();
    }

    void SetValues()
    {
        currentAmmo = MaxAmmo;

        playerInput = Player.GetComponent<PlayerInput>();
        PFireAction = playerInput.actions["Primary Fire"];
        PunchAction = playerInput.actions["Alternate Fire"];
        ReloadAction = playerInput.actions["Reload"];

        pController = Player.GetComponent<CharacterController>();
        playerStateManager = Player.GetComponent<PlayerStateManager>();
    }

    void Update()
    {
        Punch();
    }
    

    /////////////////////////////////////////////////////////////////////////Shooting\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    void Shoot()
    {
        if (CanShoot && currentAmmo > 0)
        {
            n = 0f;
            currentAmmo--;
            ShotHitscan();
            gunAnimator.Play(ANIM_FIRE);
        }
    }

    void ShotHitscan()
    {
        RaycastHit hit;
        Vector3 ray = Camera.main.ViewportToWorldPoint(new Vector3(0,0,0));
        if (Physics.Raycast(ray, Camera.main.transform.forward, out hit, Mathf.Infinity))
        {
            AssessAndAccessHit(hit, Damage);
        }
    }
    
    void AssessAndAccessHit(RaycastHit hit, float DmgToDo)
    {
        //This is a seperate method to keep methods small and readable
        Transform enemyTransform = hit.transform;
        Collider hitCollider = hit.collider;
        if(enemyTransform.TryGetComponent(out IDamageHandler dh))    //If I can somehow try and get the damage handler without having to reference it below that would be nice
        {
            //Damageable
            Debug.Log(enemyTransform+" Registered as Enemy");
            IDamageHandler enemyDamageHandler = enemyTransform.GetComponent<IDamageHandler>();                            //                            ^ this
            DoDamage(enemyDamageHandler, hitCollider, DmgToDo);
        }
        else
        {
            //Not Damageable
            DoHitVisuals();
            Debug.Log(enemyTransform+" Is not an enemy");
        }
    }
    void DoDamage(IDamageHandler damageHandler, Collider collider, float Damage)
    {
        damageHandler.TakeDamage(Damage, collider);
    }

    void DoHitVisuals()
    {
        //To do
    }

    /////////////////////////////////////////////////////////////Reload\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    void DoReload()
    {
        currentAmmo = MaxAmmo;
    }

    ///////////////////////////////////////////////////////////////////////////////Pucnch\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\\
    
    private float n = 0f;

    //private float lastCalled = 0f;
    //private float currentTime = 0f;

    private void Punch()
    {
        RaycastHit hit;
        if (Physics.Raycast(cam.transform.position, cam.transform.forward, out hit, Mathf.Infinity))
        {
            FocusMeterValue(hit);
            if (FocusMeterValue(hit) >= 0.65f && PunchAction.triggered)
            {
                Debug.Log("Punch");
                n = 0f;
                StartCoroutine(LeapLerp(hit));
            }
            else
            {
                StopCoroutine(LeapLerp(hit));
                Debug.Log("No Punch");
            }
        }
    }

    private IEnumerator LeapLerp(RaycastHit hit)
    {
        playerStateManager.playerSpeed = 0f;
        playerStateManager.playerBaseSpeed = 0f;

        gunAnimator.Play("Punch Start");
        while (Vector3.Distance(Player.transform.position,focusTarget.transform.position) > 5f)
        {
            Debug.Log("DIst: "+Vector3.Distance(Player.transform.position,focusTarget.transform.position));
            Vector3 a = Player.transform.position;
            Vector3 b = focusTarget.transform.position;
            Vector3 direction = b - a;
            pController.Move(direction * 10f * Time.deltaTime); 
            yield return new WaitForEndOfFrame();
        }

        //kill enemy here
        gunAnimator.Play("Punch Hit");
        AssessAndAccessHit(hit, 10000f);
        playerStateManager.playerBaseSpeed = 14.5f;
        playerStateManager.playerSpeed = playerStateManager.playerBaseSpeed;
        
        StopCoroutine(LeapLerp(hit));
    }


    public float FocusMeterValue(RaycastHit hit)
    {
    //If the raycast hits the target a frame add time
    //if the raycast doesnt hit a target subtract time
        
        if (hit.transform.tag == "Enemy")
        {
            {
                n += Time.deltaTime;

                focusTarget = hit.transform.gameObject;

                Debug.Log(hit);
            }
        }
        else if (n < Time.deltaTime)
        {
            n = 0f;
        }
        else
        {
            n -= Time.deltaTime * 5f;
        }



        FocusMeterUI.GetComponent<Scrollbar>().size = n;        

        Debug.Log(n);

        LastFocusValue = n;
        return n;

    //get the time between now (t) and the last time this function was called (lastCalled)
    //if lastCalled is less than a threshold (x) (~0.1 seconds?) add the difference.
    }


    public void MuzzleVisuals()
    {
        MuzzleFX_Gun.GetComponent<ParticleSystem>().Play();
        MuzzleFX_Laser.GetComponent<ParticleSystem>().Play();
    }
    
    public void ChangeShootState()
    {
        if (CanShoot)
        {
            CanShoot = false;
        } else
        {
            CanShoot = true;
        }
    }

    void ResetTimeScale()
    {
        Time.timeScale = 1f;
    }

}