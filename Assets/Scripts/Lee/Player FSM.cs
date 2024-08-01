using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.Processors;

public class PlayerFSM : MonoBehaviour
{
    public enum PlayerState
    {
        Idle = 0,
        Move = 1,
        Attack = 2,
        AttackDelay = 4,
        Damaged = 8,
        Dead = 16,
        heal = 32 
    }
    public PlayerState mystate = PlayerState.Idle; // 기본값
    public Animator animator;
    public float maxHP = 100;
    public GameObject potionprefab;
    public float healAmount = 20.0f;

    float currentTime = 0;
    float currentHP = 10; // 회복아이템 테스트할려고 일부러 낮춤 
    bool CanHeal = true; 
    Transform Player;

    CharacterController cc;

    Vector3 hitDirection;
    private void Start()
    {
        Player = GameObject.Find("Player").transform;
        cc = GetComponent<CharacterController>();
        currentHP = 10;
    }

    private void Update()
    {
        switch (mystate)
        {
            case PlayerState.Idle:
                Idle();
                break;
            case PlayerState.Move:
                Move();
                break;
            case PlayerState.Attack:
                Attack();
                break;
            case PlayerState.AttackDelay:
                AttackDelay();
                break;
           // case PlayerState.Damaged:
             //   Damaged();
             //   break;
            case PlayerState.Dead:
                Dead();
                break;
            case PlayerState.heal:
                heal();
                break;
            default:
                break;
        }
    }


    private void Idle()
    {
        
        if (mystate==PlayerState.Idle)
        {
        }
        else
        {
            mystate = PlayerState.Move; // 이동상태로 넘어감 -- 근데 이동상태가 필요할까?
        }
    }
    private void Move()
    {
        
        if (Player.position != Vector3.zero)
        {
            animator.SetFloat("IsWalking", 1.0f);
        }
        else
        {
            animator.SetFloat("IsWalking", 0.0f);

            mystate = PlayerState.Idle;
        }

    }
    private void Attack()
    {

        if (Input.GetButtonDown("Fire1")) // 무기에 콜라이더 충돌해서 불러오자
        {
            //버튼을 눌렀을때 공격 애니메이션이나가고 
        }
        else
        {
            // 입력없으면 대기 상태로 전환
            mystate = PlayerState.Idle; // Idle상태로 전환
        }

    }
    private void AttackDelay() // 존재이유 잘 모르겠음
    {
        // 이거 좀 이상함 
        currentTime += Time.deltaTime;
        if (currentTime > 0.5f)
        {
            currentTime = 0;
            mystate = PlayerState.Attack;
        }

    }
   
    private void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
      
        if (mystate == PlayerState.Dead || mystate == PlayerState.Damaged)
        {
            return;
        }

        currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP);

        if (currentHP <= 0) // HP가 0일때
        {
            mystate = PlayerState.Dead;
            currentTime = 0;

            GetComponent<CharacterController>().enabled = false;

        }
        else
        {
            mystate = PlayerState.Damaged;

        }
    }
    private void Dead()
    {
        if(currentHP <= 0)
        {
           
            GetComponent<CharacterController>().enabled = false;
           // yield return new WaitForSeconds(2f);
            Destroy(gameObject);
        }

    }

    private void heal()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            CanHeal = true;
            potionprefab.SetActive(true);
             
            //체력회복
            currentHP = currentHP + healAmount;
            // 체력을 최대값으로 제한
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            if(currentHP >= maxHP)
            {
                CanHeal = false;
            }
            print(currentHP);
        }
    }
    //void Stamina()
    //{
    //    GameObject Player = GameObject.Find("Player");

    //    if(Player != null)
    //    {
            
    //    }
    //}
}