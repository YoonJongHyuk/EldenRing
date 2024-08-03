using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using yoon;
using static PlayerFSM;

public class PlayerContorler : MonoBehaviour
{
    //�̵�
    public float MoveSpeed;
    float h, v;
    bool Run;
    Vector3 moveVec;
    //���� 
    bool isJump = false;
    Vector3 gravityPower;

    //������
    bool isDodge;
    Vector3 dodgeVec;
    //����
    public GameObject nearObject;
    public GameObject[] Weapon;
    public Sward equipWeapon;
    public bool[] hasWeapone;
    bool iDown;
    bool Swap1;
    bool Swap2;
    bool Swap3;
    bool isSwap;

    //ȸ�������� - ����
    public bool CanHeal;
    public GameObject potionprefab;
    public float healAmount = 20.0f;
    public float maxHP = 100;
    public float currentHP = 10; // ȸ�������� �׽�Ʈ�ҷ��� �Ϻη� ���� 

    //����
    bool isAttackReady;
    bool MeleeAttack;
    float AttackDelay;
    //ȭ��
    public Transform ArrowPos;
    public GameObject Arrow2;

    // ���
    Animator animator;
    Rigidbody rb;
    //ī�޶� 
    public Camera followcamera;

    public float maxStamina = 10.0f;
    public float currentStamina = 10.0f;
    public float staminaRecoveryRate = 4.0f; // �̵� �� ��� ���¿��� ���׹̳� ȸ�� 
    public float staminaDrainRateAttack = 2.0f; // ���� �� ���׹̳� �Ҹ� 
    public float staminaDrainRateDodge = 3.0f; // ������ �� ���׹̳� �Ҹ� 


    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();


    }
    void Start()
    {
        // currentHP = maxHP; // ȸ���� ���̱� ���� �ʱ�ȭ x
        currentStamina = maxStamina;
}

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        Jump();
        Interation();
        Attack();
        Swap();
        Swapout();
        potion();


    }
    void Move() // �⺻ ������ 
    {

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        Run = Input.GetButton("Run");

        moveVec = new Vector3(h, 0, v).normalized;
        if (isDodge)
            moveVec = dodgeVec;

        if (Input.GetButton("Run"))
        {
            transform.position += moveVec * MoveSpeed * 2.0f * Time.deltaTime;
        }
        else
        {
            transform.position += moveVec * MoveSpeed * Time.deltaTime;
        }

        transform.position += moveVec * MoveSpeed * Time.deltaTime;

        animator.SetBool("isWalk", moveVec != Vector3.zero);
        animator.SetBool("isRun", false);

        currentStamina = Mathf.Clamp(currentStamina, maxStamina, staminaRecoveryRate * Time.deltaTime / maxStamina);
        print("Test"+currentStamina);
    }
    //ȸ�� ���� ���콺 ��ǲ �ȹ޾Ƽ� �õ� ���غ� 
    void Rotate()
    {

        transform.LookAt(transform.position + moveVec);

    }
    //���� �ִϸ��̼� exit���� 
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            animator.SetBool("isJUmp", true);
            isJump = true;
        }
    }
    // ���������� ���� 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isJUmp", false);
            isJump = false;
        }

       
    }

    //������ //�ִϸ��̼� ����
    void Dodge()
    {
        if (Input.GetButtonDown("Dodge") && !isJump)
        {
            dodgeVec = moveVec;
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            animator.SetBool("isDodge", true);
            isDodge = true;
            Invoke("DodgeOut", 0.5f);
            currentStamina -= currentStamina -3;

        }
        isDodge = false;
    }
    //���� 
    void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (equipWeapon == null)
                return;
            if (equipWeapon != null)
            {

                AttackDelay += Time.deltaTime;
                isAttackReady = equipWeapon.rate < AttackDelay;

                if (Input.GetButtonDown("Fire1") && isAttackReady && !isDodge && !isSwap)
                {
                    equipWeapon.use();
                    animator.SetTrigger("isAttack");
                    AttackDelay = 0;

                    currentStamina -= currentStamina - 4;

                }


            }
            //ȭ�� ���� �ȵ�
            //if (Input.GetButtonUp("Fire2"))
            //{
            //    equipWeapon.use();
            //    animator.SetTrigger("isArrow");
            //    AttackDelay = 0;
            //}
            
        }
    }
   //ȸ�������� K������ �۵� //current HP ���� �غ����� 10���� �Ϻη� ����
        void potion()
        {

            if (Input.GetKeyDown(KeyCode.K))
            {
                CanHeal = true;
                potionprefab.SetActive(true);

                //ü��ȸ��
                currentHP = currentHP + healAmount;
                // ü���� �ִ밪���� ����
                currentHP = Mathf.Clamp(currentHP, 0, maxHP);

                if (currentHP >= maxHP)
                {
                    CanHeal = false;
                    potionprefab.SetActive(false);
                }
                print(currentHP);
            }
        }// ���⸦ �ֿ���� �ٲٴ� �����ε� ������ �ʰ� �ٷ� �����־ ������ ���� ����� // �ٲٴ� �ִϸ��̼� ����
        void Swap() // ����� �۵����� // �ֿ�Ծ�� // �Ұ��� �� �ٲٸ� �ٲܼ�������
        {
            int weaponIndex = -1;
            if (Input.GetButtonDown("Swap1")) weaponIndex = 0;
            if (Input.GetButtonDown("Swap2")) weaponIndex = 1;
            if (Input.GetButtonDown("Swap3")) weaponIndex = 2;

            if ((Input.GetButtonDown("Swap1") || Input.GetButtonDown("Swap2") || Input.GetButtonDown("Swap3")) && !isJump && !isDodge)
            {
                if (equipWeapon != null)
                {
                    Weapon[weaponIndex].gameObject.SetActive(false);
                    equipWeapon = Weapon[weaponIndex].GetComponent<Sward>();
                    Weapon[weaponIndex].gameObject.SetActive(true);

                    animator.SetTrigger("Swap");
                    isSwap = true;

                    Invoke("SwapOut", 0.4f);
                }
            }
        }
    // �ٷ� �� �ڵ� ���������� �ڵ�
        void Swapout()
        {
            isSwap = false;
        }
    // ������ ���� Value ������������ 
        void Interation()
        {
            if (iDown && nearObject != null && !isJump && !isDodge)
            {
                if (nearObject.tag == "Weapon")
                {
                    Item item = nearObject.GetComponent<Item>();
                    int weaponIndex = item.Value;
                    hasWeapone[weaponIndex] = true;

                    Destroy(nearObject);
                }
            }
        }
    //������ �޴� ���ε� ������ �������� ��� �������� �±׷� ã������ ������ �𸣰��� 
     void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Monster")
        {
            // currentHP -= Monster.damage;
            animator.SetTrigger("Hit");
        }
        //���з� �������� ���� ������ 
        if(Input.GetButton("Fire2"))
        {
            animator.SetBool("shild",true);
            //Monster.damage * 0.5 = currentHP;
        }

    }
    
    // ��ó ������ �ν��ؼ� �ֿ��������� �뵵
    void OnTriggerStay(Collider other)
        {
            iDown = Input.GetButtonDown("Interation");
            if (other.tag == "Weapon")
            {
                nearObject = other.gameObject;


            }
        }

        void OnTriggerExit(Collider other)
        {
            if (other.tag == "Weapon")
            {
                nearObject = null;

            }
        }
    // ������ �Դ°� , �״�� ���Դµ� characterController�� ������ ������ٵ� �ȵ��� �� �ؾ����� �𸣰���
        void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
        {
            currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP);

            if (currentHP <= 0) // HP�� 0�϶�
            {
                currentHP = 0;

                animator.SetTrigger("Die");
                GetComponent<CharacterController>().enabled = false;

            }
            else
            {
            //�ĸ´� �ִϸ��̼�
                animator.SetTrigger("Hit");


            }
        }
    }


