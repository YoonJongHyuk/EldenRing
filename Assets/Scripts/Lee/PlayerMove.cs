//using System.Collections;
//using System.Collections.Generic;
//using Unity.VisualScripting;
//using UnityEngine;
//using yoon;

//public class PlayerMove : MonoBehaviour
//{
//    //�̵� ���üӵ�
//    public float speed;
//    public int MoveSpeed = 5;
//    public int sprintSpeed = 10;
//    public int dodgeSpeed = 10;
//    public float rotSpeed = 200.0f;
//    bool IsWalking;
//    bool isRun;
//    bool isDodge;
//    //����
//    public float yVelocity = 2;
//    public float jumpPower = 4;
//    public int MaxJumpCounter = 1;
//    int currentJumpCount = 0;
//    float yPos;
//    Vector3 gravityPower;
//    //���׹̳�
//    //public float Stamina = 100;
//    //����
//    public float attackRange = 1.0f;
//    public float attackPower = 10.0f;
//    float fireDelay;
//    bool isattack;
//    GameObject Sward;
//    //ü��
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

//        // �����̵� ���
//        Vector3 dir = new Vector3(h, 0, v);
//        dir = transform.TransformDirection(dir);
//        animator.SetBool("IsWalking", true);
        
//        // float speed = 5.0f;
//        //animator.set


//        //transform.position += dir * MoveSpeed * Time.deltaTime;

//        // �����̵� ���
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


//        // run�� �������� ���ǵ尡 2�� �Ǿ���Ѵ�.
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
//        {// ���ñ��� �����ȵ�

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
//        //���� �� ������ �ο� -- �ݶ��̴� �浹�� ���濡�� ������ �ο�
//    }
//    void shield()
//    {
//        if (Input.GetMouseButton(1))
//        {
//            // ���� ���

//            //���忡 ���� �ݶ��̴��� �浹�ϸ� ������ �������� *0.5


//        }
        
//       // if(Input.GetKeyDown())
//        {
//            //F�� ������ ����� �����ϴ� ���

//            //���п� �� �ݶ��̴��� �ε����� ������ 10�� �������� �Դ´�


//        }


//    }
//    public void GetDamage(int damage)
//    {
//        if (currentHP > 0)
//        {
//            currentHP -= damage;
//            nextHP = currentHP;
//            currentTime = 0.0f; // �� �������� ���� ������ currentTime�� �ʱ�ȭ
//            //totalDamageTaken += damage; // ���� ������ ������Ʈ
            
//        }
//    }
//    //IEnumerator Stamina()
    
//}
    



