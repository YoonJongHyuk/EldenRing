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
    float currentHP = 10;
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
            case PlayerState.Damaged:
                Damaged();
                break;
            case PlayerState.Dead:
                
            default:
                break;
        }
        heal();
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

        if (Input.GetButtonDown("Fire1"))
        {
            //버튼을 눌렀을때 공격이나가고 
        }
        else
        {
            // 입력없으면 대기 상태로 전환
            mystate = PlayerState.Idle; // Idle상태로 전환
        }

    }
    private void AttackDelay()
    {
        // 이거 좀 이상함 
        currentTime += Time.deltaTime;
        if (currentTime > 0.5f)
        {
            currentTime = 0;
            mystate = PlayerState.Attack;
        }

    }
    private void Damaged()
    {

        transform.position = Vector3.Lerp(transform.position, hitDirection, 0.25f);
        if (Vector3.Distance(transform.position, hitDirection) < 0.1f)
        {
            //맞으면 피격 애니메이션이 동작한다 (잠깐대기)
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
        currentTime += Time.deltaTime;
        if(currentTime > 3.0f)
        {
            GetComponent<CharacterController>().enabled = false;
        }

    }

    private void heal()
    {
        if(Input.GetKeyDown(KeyCode.K))
        {
            
            //포션의 개수 -1 
            //체력회복
            currentHP = currentHP + healAmount;
            // 체력을 최대값으로 제한
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            if(currentHP >= maxHP)
            {
                //포션 비활성화
                
            }
            print(currentHP);
        }
    }
}