//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using yoon;

//public class PlayerMove : MonoBehaviour
//{
//    //이동 관련속도
//    public float speed;
//    public int MoveSpeed = 5;
//    public int sprintSpeed = 10;
//    public int dodgeSpeed = 10;
//    public float rotSpeed = 200.0f;
//    bool IsWalking;
//    bool isRun;
//    bool isDodge;
//    //점프
//    public float yVelocity = 2;
//    public float jumpPower = 4;
//    public int MaxJumpCounter = 1;
//    int currentJumpCount = 0;
//    float yPos;
//    Vector3 gravityPower;
//    //스테미나
//    //public float Stamina = 100;
//    //공격
//    public float attackRange = 1.0f;
//    public float attackPower = 10.0f;
//    float fireDelay;
//    bool isattack;
//    GameObject Sward;
//    //체력
//    float MaxHP = 100;
//    float currentHP;
//    float nextHP;

//    float currentTime;
//    float rotX;
//    float rotY;

//    Animator animator;
//    CharacterController cc;

//    private void Awake()
//    {
//        animator = GetComponent<Animator>();

//    }

//    void Start()
//    {
//        Cursor.lockState = CursorLockMode.Locked;

//        rotX = transform.eulerAngles.x;
//        rotY = transform.eulerAngles.y;

//        cc = GetComponent<CharacterController>();

//        gravityPower = Physics.gravity;
        
//    }


//    void Update()
//    {
//        Idle();
//        Move();
//        rotate();
//        Attack();

//    }
//    void Idle()
//    {
//       // animator.SetBool("Idle", true);
//    }
//    void Move()
//    {
//        float h = Input.GetAxisRaw("Horizontal");
//        float v = Input.GetAxisRaw("Vertical");

//        // 수평이동 계산
//        Vector3 dir = new Vector3(h, 0, v);
//        dir = transform.TransformDirection(dir);
//        animator.SetBool("IsWalking", true);
        
//        // float speed = 5.0f;
//        //animator.set


//        //transform.position += dir * MoveSpeed * Time.deltaTime;

//        // 수직이동 계산
//        yPos += gravityPower.y * yVelocity * Time.deltaTime;
//        //currentJumpCount = 1;

//        if (cc.collisionFlags == CollisionFlags.CollidedBelow)
//        {
//            animator.SetBool("IsJump", true);
//            yPos = 0;
//            currentJumpCount = 0;
//        }
//        if (Input.GetButtonDown("Jump") && currentJumpCount < MaxJumpCounter)
//        {
//            animator.SetBool("IsJump", false);
//            yPos = jumpPower;
//            currentJumpCount++;
//        }
//        dir.y = yPos;


//        // run이 눌렸을때 스피드가 2배 되어야한다.
//        if (Input.GetButton("Run"))
//        {
//            animator.SetBool("IsRunning", true);
//            cc.Move(dir * sprintSpeed * Time.deltaTime);

//        }
//        else
//        {
//            cc.Move(dir * MoveSpeed * Time.deltaTime);
//            animator.SetBool("IsWalking", true);
//            animator.SetBool("IsRunning", false);

           
//        }

//        if (Input.GetButtonDown("Dodge"))
//        {
//            cc.Move(dir * dodgeSpeed *3  * Time.deltaTime);
//            isDodge = true;

//        }
//    }

//    void rotate()
//    {
//        float mousex = Input.GetAxis("Mouse X");
//        float mousey = Input.GetAxis("Mouse Y");

//        rotX += mousey * rotSpeed * Time.deltaTime;
//        rotY += mousex * rotSpeed * Time.deltaTime;

//        if (rotX > 80)
//        {
//            rotX = 80.0f;
//        }
//        else if (rotX > -80)
//        {
//            rotX = -80.0f;
//        }
//        transform.eulerAngles = new Vector3(0, rotY, 0);

//    }
//    void Attack()
//    {
//        if (Sward == null)
//            return;

//        fireDelay += Time.deltaTime;
//        isattack = false;
       
//        if (Input.GetMouseButtonDown(0) && !isDodge)  // Left mouse button
//        {// 어택구현 아직안됨

//            fireDelay = 0;
//            isattack = true;



//            if(Sward != null)
//            {
//                // Sward = Sward.GetComponent<>();
//                Sward.gameObject.SetActive(true);

//            }
            

//        }
//        else
//        {
//            isattack = false;
//        }
//        //공격 시 데미지 부여 -- 콜라이더 충돌시 상대방에게 데미지 부여
//    }
//    void shield()
//    {
//        if (Input.GetMouseButton(1))
//        {
//            // 쉴드 모션

//            //쉴드에 상대방 콜라이더가 충돌하면 설정된 데미지의 *0.5


//        }
        
//       // if(Input.GetKeyDown())
//        {
//            //F를 누르면 쉴드로 공격하는 모션

//            //방패와 적 콜라이더가 부딪히면 상대방은 10의 데미지를 입는다


//        }


//    }
//    public void GetDamage(int damage)
//    {
//        if (currentHP > 0)
//        {
//            currentHP -= damage;
//            nextHP = currentHP;
//            currentTime = 0.0f; // 새 데미지를 받을 때마다 currentTime을 초기화
//            //totalDamageTaken += damage; // 누적 데미지 업데이트
            
//        }
//    }
//    //IEnumerator Stamina()
    
//}
    



