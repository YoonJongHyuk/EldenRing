using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;
using yoon;
using static PlayerFSM;

public class PlayerContorler : MonoBehaviour
{
    //이동
    public float MoveSpeed;
    float h, v;
    bool Run;
    Vector3 moveVec;
    //점프 
    bool isJump = false;
    Vector3 gravityPower;

    //구르기
    bool isDodge;
    Vector3 dodgeVec;
    //무기
    public GameObject nearObject;
    public GameObject[] Weapon;
    public Sward equipWeapon;
    public bool[] hasWeapone;
    bool iDown;
    bool Swap1;
    bool Swap2;
    bool Swap3;
    bool isSwap;

    //회복아이템 - 포션
    public bool CanHeal;
    public GameObject potionprefab;
    public float healAmount = 20.0f;
    public float maxHP = 100;
    public float currentHP = 10; // 회복아이템 테스트할려고 일부러 낮춤 

    //공격
    bool isAttackReady;
    bool MeleeAttack;
    float AttackDelay;
    //화살
    public Transform ArrowPos;
    public GameObject Arrow2;

    // 기능
    Animator animator;
    Rigidbody rb;
    //카메라 
    public Camera followcamera;

    public float maxStamina = 10.0f;
    public float currentStamina = 10.0f;
    public float staminaRecoveryRate = 4.0f; // 이동 및 대기 상태에서 스테미나 회복 
    public float staminaDrainRateAttack = 2.0f; // 공격 시 스테미나 소모 
    public float staminaDrainRateDodge = 3.0f; // 구르기 시 스테미나 소모 


    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();


    }
    void Start()
    {
        // currentHP = maxHP; // 회복을 보이기 위해 초기화 x
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
    void Move() // 기본 움직임 
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
    //회전 아직 마우스 인풋 안받아서 시도 안해봄 
    void Rotate()
    {

        transform.LookAt(transform.position + moveVec);

    }
    //점프 애니메이션 exit문제 
    void Jump()
    {
        if (Input.GetButtonDown("Jump") && !isJump)
        {
            rb.AddForce(Vector3.up * 5, ForceMode.Impulse);
            animator.SetBool("isJUmp", true);
            isJump = true;
        }
    }
    // 땅에닿으면 점프 
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            animator.SetBool("isJUmp", false);
            isJump = false;
        }

       
    }

    //구르기 //애니메이션 없음
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
    //공격 
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
            //화살 아직 안됨
            //if (Input.GetButtonUp("Fire2"))
            //{
            //    equipWeapon.use();
            //    animator.SetTrigger("isArrow");
            //    AttackDelay = 0;
            //}
            
        }
    }
   //회복아이템 K누르면 작동 //current HP 실험 해볼려고 10으로 일부로 줄임
        void potion()
        {

            if (Input.GetKeyDown(KeyCode.K))
            {
                CanHeal = true;
                potionprefab.SetActive(true);

                //체력회복
                currentHP = currentHP + healAmount;
                // 체력을 최대값으로 제한
                currentHP = Mathf.Clamp(currentHP, 0, maxHP);

                if (currentHP >= maxHP)
                {
                    CanHeal = false;
                    potionprefab.SetActive(false);
                }
                print(currentHP);
            }
        }// 무기를 주운다음 바꾸는 내용인데 줍지도 않고 바로 차고있어서 없앨지 말지 고민중 // 바꾸는 애니메이션 없음
        void Swap() // 제대로 작동안함 // 주운게없어서 // 불값을 잘 바꾸면 바꿀수있을듯
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
    // 바로 위 코드 빠져나오는 코드
        void Swapout()
        {
            isSwap = false;
        }
    // 아이템 마다 Value 정보가져오기 
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
    //데미지 받는 곳인데 몬스터의 데미지가 어떻게 들어오는지 태그로 찾을려고 했으나 모르겠음 
     void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Monster")
        {
            // currentHP -= Monster.damage;
            animator.SetTrigger("Hit");
        }
        //방패로 막았을때 절반 데미지 
        if(Input.GetButton("Fire2"))
        {
            animator.SetBool("shild",true);
            //Monster.damage * 0.5 = currentHP;
        }

    }
    
    // 근처 아이템 인식해서 주워먹을려는 용도
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
    // 데미지 입는거 , 그대로 들고왔는데 characterController가 없은데 리지드바디 안들어가고 뭘 해야할지 모르겠음
        void TakeDamage(float atkPower, Vector3 hitDir, Transform attacker)
        {
            currentHP = Mathf.Clamp(currentHP - atkPower, 0, maxHP);

            if (currentHP <= 0) // HP가 0일때
            {
                currentHP = 0;

                animator.SetTrigger("Die");
                GetComponent<CharacterController>().enabled = false;

            }
            else
            {
            //쳐맞는 애니메이션
                animator.SetTrigger("Hit");


            }
        }
    }


