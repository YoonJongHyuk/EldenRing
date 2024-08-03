using System.Collections;
using System.Collections.Generic;
using UnityEditor.Overlays;
using UnityEngine;

public class PlayerContorler : MonoBehaviour
{
    //이동
    public float MoveSpeed;
    float h;
    float v;
    bool Run;
    Vector3 moveVec;
    //점프 
    bool isJump;
    public float jumpPower = 4;
    public float yVelocity = 2;
    Vector3 gravityPower;

    //구르기
    bool isDodge;
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

    // Start is called before the first frame update

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        animator = GetComponentInChildren<Animator>();

    }
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();
        Jump();
        Interation();
        Attack();
        // FireType2();
        Shield();
        Swap();
        

    }
    void Move()
    {

        h = Input.GetAxisRaw("Horizontal");
        v = Input.GetAxisRaw("Vertical");
        Run = Input.GetButton("Run");

        moveVec = new Vector3(h, 0, v).normalized;
       
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
        animator.SetBool("isRun", Run);

    }
    void Rotate()
    {
        transform.LookAt(transform.position + moveVec);

        //float mousex = Input.GetAxis("Mouse X");
        //float mousey = Input.GetAxis("Mouse Y");

        //rotX += mousey * rotSpeed * Time.deltaTime;
        //rotY += mousex * rotSpeed * Time.deltaTime;

        //if (rotX > 80)
        //{
        //    rotX = 80.0f;
        //}
        //else if (rotX > -80)
        //{
        //    rotX = -80.0f;
        //}
        //transform.eulerAngles = new Vector3(0, rotY, 0);
        
        //if(Input.GetButtonDown("Fire2"))
        //{

        //Ray ray = followcamera.ScreenPointToRay(Input.mousePosition);
        //Raycast rayHit;
        //if(Physics.Raycast(ray,out rayHit, 100.0f))
        //{
        //    Vector3 nextVec = rayHit.point - transform.position;
        //        nextVec.y = 0;
        //    transform.LookAt(transform.position + nextVec);
        //}
        //}
       
        
    }
    void Jump()
    {
        jumpPower = gravityPower.y * yVelocity * Time.deltaTime;

        if (Input.GetButtonDown("Jump") && !isJump)
        {
            rb.AddForce(Vector3.up * jumpPower, ForceMode.Impulse);
            isJump = true;
            print("점프중");
            // animator.SetBool("",);
            // animator.SetBool("", ); -- 쌤 애니메이션 보고 다시 

        }
        // 반복이 안됨 1회만 가능 그마저도 떠오르지도 않음 
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Ground")
        {
            //  animator.SetBool("",);
            isJump = false;
        }
    }

        void Attack()
    {
        if (Input.GetButtonDown("Fire1"))
        {
            if (equipWeapon == null)
                return;

            AttackDelay += Time.deltaTime;
            isAttackReady = equipWeapon.rate < AttackDelay;

            if (Input.GetButtonDown("Fire1") && isAttackReady && !isDodge && !isSwap)
            {
                equipWeapon.use();
                animator.SetTrigger("isAttack");
                AttackDelay = 0;
            }
            
        }
        if (Input.GetButtonUp("Fire2"))
        {
            equipWeapon.use();
            animator.SetTrigger("isArrow");
            AttackDelay = 0;
        }

    }

    //void FireType2()
    //{
    //    if(Input.GetMouseButtonUp(0))
    //    {
    //    Ray ray = new Ray(Camera.main.transform.position, Camera.main.transform.forward);

            
    //    }
    //}
 
    void Shield()
    {
        
    }
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
            }
            print(currentHP);
        }
    }
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
                Weapon[weaponIndex].gameObject. SetActive(true);

                animator.SetTrigger("Swap");
                isSwap = true;

                Invoke("SwapOut", 0.4f);
            }
        }
    }
    void Swapout()
    {
        isSwap = false;
    }
    void Interation()
    {
        if(iDown && nearObject != null && !isJump && !isDodge)
        {
            if(nearObject.tag == "Weapon")
            {
                Item item = nearObject.GetComponent<Item>();
                int weaponIndex = item.Value;
                hasWeapone[weaponIndex] = true;

                Destroy(nearObject);
            }
        }
    }    
    private void OnTriggerStay(Collider other)
    {
        iDown = Input.GetButtonDown("Interation");
        if (other.tag == "Weapon")
        {
            nearObject = other.gameObject;


        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.tag == "Weapon")
        {
            nearObject = null;

        }
    }
}
