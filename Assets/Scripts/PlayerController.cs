using UnityEngine;
using TMPro;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System.Collections;
using System;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour {

    public const float ChainAttackTimeOut = 1f;
    protected float ChainAttackCountdown = 0;
    public float WalkSpeed;
    public Animator AnimController;
    public int Direction;    
    protected bool bCanKick = true;
    protected bool bCanPunch = true;
    public CameraPanner TheCameraPanner;
    public BoxCollider MainCollider;
    public AudioClip KickClip;
    public AudioSource EffectPlayer;    
    public InputContainer inputContainer = new InputContainer();
    public int PlayerNumber;
    public bool IsRealPlayer;
    public Canvas TheCanvas;
    public float Health;
    public GameObject GameObjectParent;
    public SpriteRenderer spriteRenderer;
    public bool InvincibleBecauseOfFlight;
    public AudioClip HitGroundClip;
    protected bool AttackConnectedThisFrame;
    protected WaveManager MyWaveManager;

    // Use this for initialization
    void Start ()
    {
        // Call set player!!!!!!!!
        if (IsRealPlayer)
        {
            GameManager.GetPlayerInput(PlayerNumber).Player = this;
        }

        AnimController = GetComponent<Animator>();
        MainCollider = GetComponent<BoxCollider>();

        GameObjectParent.transform.position = new Vector3(GameObjectParent.transform.position.x, GameObjectParent.transform.position.y, GameObjectParent.transform.position.y);

        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("AI Sense Layer"), LayerMask.NameToLayer("Hit Box"));
        Physics.IgnoreLayerCollision(LayerMask.NameToLayer("Ignore Raycast"), LayerMask.NameToLayer("Hit Box"));

        if (IsRealPlayer)
        {
            TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(Health.ToString());
        }
        else 
        {
            if (MyWaveManager == null || !MyWaveManager.HasBeenActivated)
            {
                gameObject.SetActive(false);
            }
        }        
    }

    public void SetWaveManager(WaveManager NewWaveManager)
    {
        MyWaveManager = NewWaveManager;
    }

    private void OnTriggerEnter(Collider collider)
    {
        PlayerController OwningPlayer = collider.gameObject.GetComponentInParent<PlayerController>();

        if (OwningPlayer != null)
        {
            string ParentTag = OwningPlayer.tag;
            if (tag == ParentTag)
            {
                return;
            }
        }
        
        if (Health > 0 && collider.tag == "HitBox" && !InvincibleBecauseOfFlight)
        {            
            collider.gameObject.GetComponentInParent<PlayerController>().OnAttackConnected(this);
            OnTakeDamageBegin();

            Health -= collider.GetComponentInParent<AttackInfo>().DamageAmount;
            if (IsRealPlayer)
            {
                TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText(Health.ToString());
            }

            if (Health <= 0)
            {
                AnimController.SetTrigger("Dying");
                AnimController.SetTrigger("Flying");                
                inputContainer.Reset();
            }

            if (collider.GetComponent<AttackInfo>().bCausesKnockDown)
            {
                InvincibleBecauseOfFlight = true;
                AnimController.SetTrigger("Flying");
                inputContainer.Reset();
            }

            if (collider.GetComponent<AttackInfo>().bCausesKnockDown || Health <= 0)
            {
                if (collider.transform.forward.z == 1)
                {
                    Quaternion NewRot = Quaternion.Euler(0, 180, 0);
                    GameObjectParent.transform.rotation = NewRot;
                }
                else
                {
                    Quaternion NewRot = Quaternion.Euler(0, 0, 0);
                    GameObjectParent.transform.rotation = NewRot;
                }
            }

        }
    }    

    // Update is called once per frame
    void Update ()
    {
        if (IsRealPlayer && Health <= 0 && inputContainer.AttackPressed)
        {
            GameManager.Restart();
            //foreach (GameObject Object in SceneManager.GetSceneByName("TopLevelShit").GetRootGameObjects())
            //{
            //    if (Object.GetComponent<GameManager>() != null)
            //    {
            //        Object.GetComponent<GameManager>().Restart();
            //    }
            //}
        }
        //if (!TheCameraPanner.IsOnScreen(gameObject))
        //{
        //    return;
        //}

        bool bWalkingThisFrame = false;
        Vector3 Velocity = new Vector3();

        if (Health > 0 && !InvincibleBecauseOfFlight)
        {
            if (!AnimController.GetBool("IsAttacking"))
            {
                if (inputContainer.MovementAxis.magnitude != 0)
                {
                    if (inputContainer.MovementAxis.x != 0)
                    {
                        ChangeDirection(inputContainer.MovementAxis.x < 0 ? 1 : 0);
                    }
                    Velocity = inputContainer.MovementAxis * WalkSpeed * Time.deltaTime;
                    bWalkingThisFrame = true;
                }
            }

            if (bCanKick && !AnimController.GetBool("IsAttacking") && !AnimController.GetCurrentAnimatorStateInfo(0).IsName("BillyHighKick") && !AnimController.GetCurrentAnimatorStateInfo(0).IsName("BillyKick") && inputContainer.AttackPressed)
            {
                AnimController.SetBool("IsRecovering", false);
                AnimController.SetBool("IsAttacking", true);
                AnimController.SetInteger("AttackType", 0);
                bCanKick = false;
                bWalkingThisFrame = false;
            }
            else
            {
                bCanKick = true;
            }

            if (bCanPunch && !AnimController.GetBool("IsAttacking") && !AnimController.GetCurrentAnimatorStateInfo(0).IsName("BillyPunch") && !AnimController.GetCurrentAnimatorStateInfo(0).IsName("BillyPunch2") && !AnimController.GetCurrentAnimatorStateInfo(0).IsName("BillyFinalPunch") && inputContainer.Attack2Pressed)
            {
                AnimController.SetBool("IsRecovering", false);
                AnimController.SetBool("IsAttacking", true);
                AnimController.SetInteger("AttackType", 1);
                bCanPunch = false;
                bWalkingThisFrame = false;
            }
            else
            {
                bCanPunch = true;
            }

            Move(Velocity);
            AnimController.SetBool("IsWalking", bWalkingThisFrame);
        }

        if (AnimController.GetInteger("ChainedAttacks") > 0)
        {
            ChainAttackCountdown -= Time.deltaTime;

            if (ChainAttackCountdown <= 0)
            {
                AnimController.SetInteger("ChainedAttacks", 0);
            }
        }
    }    

    void Move(Vector3 Velocity)
    {
        if (TheCameraPanner == null)
        TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText("FUCK");
        else
            TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText("GOOD");

        if (TheCameraPanner != null && IsRealPlayer)
        {
            //Replace with level min and max Y!!
            float ClampedY = Mathf.Clamp(GameObjectParent.transform.position.y, -0.708f, -0.222f);

            //Stay within the camera view limits
            var dist = (GameObjectParent.transform.position - Camera.main.transform.position).z;
            var leftBorder = Camera.main.ViewportToWorldPoint(new Vector3(0, 0, dist)).x + MainCollider.bounds.size.x / 2;
            var rightBorder = Camera.main.ViewportToWorldPoint(new Vector3(1, 0, dist)).x - MainCollider.bounds.size.x / 2; ;

            float ClampedX = Mathf.Clamp(GameObjectParent.transform.position.x, leftBorder, rightBorder);

            GameObjectParent.transform.position = new Vector3(ClampedX, ClampedY, ClampedY);

            if (Velocity != Vector3.zero && IsRealPlayer)
            {
                TheCameraPanner.UpdateScroll(this, Velocity);
            }
        }
        else
        {
            float ClampedY = Mathf.Clamp(GameObjectParent.transform.position.y, -0.708f, -0.222f);
            GameObjectParent.transform.position = new Vector3(GameObjectParent.transform.position.x, ClampedY, ClampedY);
        }

        if (!InvincibleBecauseOfFlight && Health > 0 && !AnimController.GetBool("IsRecovering") && !AnimController.GetBool("IsStunned") && AnimController.GetBool("IsWalking"))
        {
            if (Direction == 1)
            {
                Velocity.x *= -1;
            }
            
            GameObjectParent.transform.Translate(Velocity);
            Velocity.y = 0;

        }        
    }

    public void ChangeDirection(int NewDirection)
    {        
        if (!AnimController.GetBool("IsStunned"))
        {            
            if (NewDirection == 0)
            {
                //GetComponent<SpriteRenderer>().flipX = false;

                Quaternion NewRot = Quaternion.Euler(0, 0, 0);
                GameObjectParent.transform.rotation = NewRot;
            }
            else
            {
                Quaternion NewRot = Quaternion.Euler(0, 180, 0);
                GameObjectParent.transform.rotation = NewRot;
            }
        }

        Direction = NewDirection;
    }

    public void OnKickStart()
    {
        AttackConnectedThisFrame = false;
        AnimController.SetBool("IsRecovering", false);
        //EffectPlayer.PlayOneShot(KickClip);
    }

    public void OnKickEnd()
    {
        bCanKick = false;
        AttackConnectedThisFrame = false;
        AnimController.SetBool("IsAttacking", false);
        AnimController.SetBool("IsRecovering", true);
        //Debug.Log("Kick false");        
    }

    public void OnTakeDamageBegin()
    {
        if (IsRealPlayer)
        {
            //AnimController.SetInteger("ChainedAttacks", 0);
        }

        AttackConnectedThisFrame = false;
        AnimController.ResetTrigger("DoStun1");
        AnimController.SetBool("IsStunned", true);
        AnimController.SetBool("IsAttacking", false);
        AnimController.SetBool("IsRecovering", false);
        AnimController.SetBool("IsWalking", false);
        AnimController.SetTrigger("DoStun1");        
        //AnimController.ResetTrigger("IsStunned");
    }

    public void OnTakeDamageEnd()
    {
        AnimController.SetBool("IsStunned", false);
        
        //Debug.Log("Damage End");
    }

    public void OnRecoverEnd()
    {
        AnimController.SetBool("IsRecovering", false);
        AttackConnectedThisFrame = false;

    }

    public void OnAttackConnected(PlayerController Other)
    {
        if (!AttackConnectedThisFrame)
        {
            AttackConnectedThisFrame = true;
            ChainAttackCountdown = ChainAttackTimeOut;
            AnimController.SetInteger("ChainedAttacks", AnimController.GetInteger("ChainedAttacks")+1);
        }
    }

    public void OnFlyEnd()
    {
        Debug.Log("fly end");
        Vector3 What = transform.TransformPoint(Vector3.zero) - GameObjectParent.transform.position;
        //What.y = -What.y;
        GameObjectParent.transform.position = transform.TransformPoint(Vector3.zero);// - spriteRenderer.bounds;// / 2;
        transform.position = Vector3.zero;
        InvincibleBecauseOfFlight = false;        
        AnimController.SetBool("IsStunned", false);
    }

    public void OnHitGround()
    {
        if (EffectPlayer.isPlaying)
        {
            EffectPlayer.Stop();
        }
        EffectPlayer.PlayOneShot(HitGroundClip);
    }

    public void OnFinishDeath()
    {
        if (IsRealPlayer)
        {
            TheCanvas.GetComponentInChildren<TextMeshProUGUI>().SetText("GAME OVER\nPRESS ATTACK TO RESET");
            TheCanvas.GetComponentInChildren<TextMeshProUGUI>().transform.localScale = new Vector3(4, 4, 4);
            TheCanvas.GetComponentInChildren<TextMeshProUGUI>().rectTransform.localPosition = Vector3.zero;
            TheCanvas.GetComponentInChildren<TextMeshProUGUI>().alignment = TextAlignmentOptions.Center;
        }
        else
        {
            gameObject.SetActive(false);

        }
    }
}
