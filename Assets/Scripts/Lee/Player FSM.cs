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
    public PlayerState mystate = PlayerState.Idle; // �⺻��
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
            mystate = PlayerState.Move; // �̵����·� �Ѿ -- �ٵ� �̵����°� �ʿ��ұ�?
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
            //��ư�� �������� �����̳����� 
        }
        else
        {
            // �Է¾����� ��� ���·� ��ȯ
            mystate = PlayerState.Idle; // Idle���·� ��ȯ
        }

    }
    private void AttackDelay()
    {
        // �̰� �� �̻��� 
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
            //������ �ǰ� �ִϸ��̼��� �����Ѵ� (�����)
        }

    }
    private void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
    {
      
        if (mystate == PlayerState.Dead || mystate == PlayerState.Damaged)
        {
            return;
        }

        currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP);

        if (currentHP <= 0) // HP�� 0�϶�
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
            
            //������ ���� -1 
            //ü��ȸ��
            currentHP = currentHP + healAmount;
            // ü���� �ִ밪���� ����
            currentHP = Mathf.Clamp(currentHP, 0, maxHP);

            if(currentHP >= maxHP)
            {
                //���� ��Ȱ��ȭ
                
            }
            print(currentHP);
        }
    }
}