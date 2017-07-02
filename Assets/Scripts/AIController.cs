using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AIController : MonoBehaviour {

    public float ChoiceTime = 0.25f;
    protected float ChoiceCountDown;
    public PlayerController Player;
    float AttackAgainCountDown = 0;
    InputContainer inputContainer = new InputContainer();
    public float AttackChainRate;
    public bool bPlayerInRange;
    public SphereCollider SenseSphere;

    void Start ()
    {
        ChoiceCountDown = ChoiceTime;
        Player.inputContainer.Reset();
        SenseSphere = GetComponentInChildren<SphereCollider>();        
    }

    private void OnTriggerEnter(Collider collision)
    {
        if (collision.tag == "Player")
        {
            if (Player.AnimController.GetInteger("ChainedAttacks") < 3)
            {
                AttackAgainCountDown = AttackChainRate;
                Debug.Log("Attack again");
            }
            else
                Debug.Log("end combo");
        }
        else if (collision.tag == "Enemy")
        {
            inputContainer.Reset();
            if (Random.Range(0, 2) == 0)
            {
                inputContainer.MovementAxis.x = collision.transform.position.x < transform.position.x ? 0.75f : -0.75f;
            }
            else
            {
                inputContainer.MovementAxis.y = collision.transform.position.y < transform.position.y ? 1.25f : -1.25f;
            }
            ChoiceCountDown = ChoiceTime;
        }
        
    }

    
    // Update is called once per frame
    void Update ()
    {
        inputContainer.AttackPressed = false;
        ChoiceCountDown -= Time.deltaTime;
        
        if (Player.AnimController.GetBool("IsStunned") && Player.Health <= 0)
        {
            return;
        }
        
        if (AttackAgainCountDown > 0)
        {
            AttackAgainCountDown -= Time.deltaTime;

            if (AttackAgainCountDown < 0)
            {
                inputContainer.Reset();
                inputContainer.AttackPressed = true;
            }
            Player.inputContainer = inputContainer;

            return;
        }

        Vector3 PlayerPos = FindObjectOfType<PlayerInput>().transform.position;

        if (Mathf.Abs(PlayerPos.y - transform.position.y) <= 0.022)
        {
            inputContainer.MovementAxis.y = 0;
        }

        if (Mathf.Abs(PlayerPos.x - transform.position.x) <= 0.25)
        {
            inputContainer.MovementAxis.x = 0;
        }

        if (Random.Range(0, 10) > 4 && ChoiceCountDown <= ChoiceTime / 2 && Mathf.Abs(PlayerPos.y - transform.position.y) <= 0.022 && Mathf.Abs(PlayerPos.x - transform.position.x) <= 0.23)
        {
            inputContainer.Reset();
            inputContainer.AttackPressed = true;
        }

        if (ChoiceCountDown < 0)
        {

            inputContainer.Reset();
                        
            if (Mathf.Abs(PlayerPos.x - transform.position.x) <= 0.6)
            {
                inputContainer.MovementAxis.y = PlayerPos.y < transform.position.y ? -1 : 1;
            }
                   
            if (Mathf.Abs(PlayerPos.x - transform.position.x) > 0.25)
            {
                inputContainer.MovementAxis.x = PlayerPos.x < transform.position.x ? -1 : 1;
            }

            if (Random.Range(0, 10) > 7)
            {
                inputContainer.Reset();
            }
            
            ChoiceCountDown = ChoiceTime;
        }

        Player.inputContainer = inputContainer;
	}
}
