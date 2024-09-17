using System.Collections.Generic;
using System.Net.NetworkInformation;
using System.Collections;
using UnityEngine; 
using UnityEngine.AI;

namespace PestBehavior
    {
        public class PestStateHandler : MonoBehaviour, IDamageHandler, INeighborhoodWatch
        {
            //State Machine stuff
            public PestBaseState currentState;
            public PestBaseState lastState;
            public PestPatrolState PatrolState = new PestPatrolState();
            public PestChaseState ChaseState = new PestChaseState();
            public PestDamagedState DamagedState = new PestDamagedState();
            public PestAttackState AttackState = new PestAttackState();

            //AI
            public NavMeshAgent pestAgent;

            //Movement Variables
            public float MoveSpeed = 15f;
            [SerializeField] public float DetectionRange = 25f;
            public float Acceleration = 8f;
            public float StoppingDistance = 0.01f;

            public bool coroutineRunning;

            //Body
            public GameObject pestBody;

            //Neighbors
            Collider[] nearbyArray;
            public List<GameObject> neighborList = new List<GameObject>();

            //Combat Stats
            public float MaxHealth = 75;
            public float Health;
            public bool isVulnerable;

            //Animator & Animation
            public Animator animator;
            public float NTime = 0f;
            public GameObject clockHand;

            //Player
            public GameObject PlayerObject;
            public Vector3 PlayerPos;

            //Body
            public Collider bodyCollider;
            public Collider critCollider;

            //Prefabs
            public GameObject DeathVFX_Prefab;
            public Material mouthMaterial;

            public GameObject AttackFX_Prefab;
            public GameObject AttackMissileFX_Prefab;
            public GameObject AttackChargeFX_Prefab;
            public GameObject AttackEndChargeFX_Prefab; 

            //Firepoint
            public Vector3 firepoint;

            
            void Awake()
            {
                while (!pestAgent)
                {
                    pestAgent = GetComponent<NavMeshAgent>();
                }
                Health = MaxHealth;
                pestBody = this.transform.GetChild(0).gameObject;
                animator = this.transform.GetChild(1).GetComponent<Animator>();
                animator.applyRootMotion = true;
                firepoint = gameObject.transform.GetChild(1).transform.position;
                bodyCollider = pestBody.GetComponent<SphereCollider>();
                critCollider = pestBody.GetComponent<BoxCollider>();
                AnimatorStateInfo stateInfo = animator.GetCurrentAnimatorStateInfo(0);
                float NTime = stateInfo.normalizedTime;
            }

            void Start()
            {
                currentState = PatrolState;
                currentState.EnterState(this);

                PlayerObject = GameObject.FindGameObjectWithTag("Player").gameObject;
            }
            public void SwitchStates(PestBaseState state)
            {
                lastState = currentState;

                currentState = state;
                state.EnterState(this);

                //OLD:Maybe add Cleanup(); method to all pestbasestate inheritents
                
                
            }

            public IEnumerator PatrolTimer(float timer)
            {
                coroutineRunning = true;
                yield return new WaitForSeconds(timer);

                coroutineRunning = false;
                PatrolState.needNewPos = true;

                StopCoroutine(PatrolTimer(0f));
            }

            public IEnumerator AttackCooldown(float timer)
            {
                coroutineRunning = true;
                yield return new WaitForSeconds(timer);
                coroutineRunning = false;
                AttackState.canAttack = true;

                StopCoroutine(AttackCooldown(5f));
            }

            public void GetNeighbors(float radius)
            {
                nearbyArray = Physics.OverlapSphere(transform.position, radius);
                foreach (Collider neighbor in nearbyArray)
                {
                    if (neighbor.tag == "Enemy")
                    {
                        INeighborhoodWatch alertScript = neighbor.transform.GetComponent<INeighborhoodWatch>();
                        if (alertScript != null)
                        {
                            neighborList.Add(neighbor.transform.gameObject);
                        }
                    }
                }
            }

            public void Alert(float dist)
            {
                GetNeighbors(DetectionRange);

                if (dist <= 15f)
                {
                SwitchStates(AttackState);
                }
                else
                {
                SwitchStates(ChaseState);
                ChaseState.chaseTarget = PlayerObject;
                }
                

                for (int i = 0; i < neighborList.Count; i++)
                {
                    PestStateHandler stateHandler = neighborList[i].GetComponent<PestStateHandler>();
                    if (stateHandler)
                    {

                        stateHandler.SwitchStates(stateHandler.ChaseState);
                        stateHandler.ChaseState.chaseTarget = PlayerObject;
                    }
                }
            }

            public void LaunchMissile()
            {
                AttackState.ProjectileHandler(this);
            }

            public void SpawnEndChargeFX()
            {
                AttackState.StopChargeFX(this);
            }

            public void TakeDamage(float damage, Collider hitCollider)
            {                
                Debug.Log("Pest damaged");
                if (hitCollider == critCollider && critCollider.enabled)
                {
                    SwitchStates(DamagedState);
                    DamagedState.Explode(this);
                    Debug.Log("CRITCAL STRIKE!");
                }
                else
                {
                    SwitchStates(DamagedState);
                    DamagedState.DealDamage(damage, this);
                }
            }

           public void EndAttack()
            {
                SwitchStates(ChaseState);
                animator.enabled = false;
            }

            void UpdatePlayerLoc()
            {
                PlayerPos = PlayerObject.transform.position;
            }

            public void OnCrit()
            {
                SwitchStates(DamagedState);
                DamagedState.Die(this);
            }

            public void OnVulnerable()
            {
                critCollider.enabled = true;
            }

            public void OffVulnerable()
            {
                critCollider.enabled = false;
            }

        ///OLD: <summary>
        ///OLD: Animation Functions
        ///OLD: </summary> 
        public void ChargeAnim()
            {
                animator.Play("Charge");
            }

            public void ShootAnim()
            {
                animator.applyRootMotion = true;
                animator.Play("Shoot");
                Mathf.Lerp(clockHand.transform.rotation.x, clockHand.transform.rotation.x + 180f, Time.deltaTime / 2);
            }

            private void OnAnimatorMove()
            {
                Vector3 rootPosition = animator.rootPosition;
                rootPosition.y = pestAgent.nextPosition.y;
                transform.position = rootPosition;
            }

            public void CloseAnim()
                {
                    animator.Play("Close");
                }

                void Update()
                {
                    currentState.UpdateState(this);
                    Debug.Log("Current State: " + currentState);
                    Debug.Log("pestHandler Position: " + this.transform.position);
                    UpdatePlayerLoc();
                Debug.Log("NTime: " + NTime);
                }
            }    
    }

