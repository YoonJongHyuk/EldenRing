using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour
{
    public int MoveSpeed = 5;
    public float rotSpeed = 200.0f;
    public float yVelocity = 2;
    public float jumpPower = 4;
    public int MaxJumpCounter = 1;
    public float Stamina = 100;

    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;
    bool isRun;
    bool isDodge;

    Animator anim;
    CharacterController cc;


    Vector3 gravityPower;
    
    private void Awake()
    {
        anim = GetComponentInChildren<Animator>();
    }

    void Start()
    {
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        cc = GetComponent<CharacterController>();

        gravityPower = Physics.gravity;
        Stamina = 100;
    }

    void Update()
    {
        Move();
        rotate();
        

    }
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        // 수평이동 계산
        Vector3 dir = new Vector3(h, 0, v);

        dir = transform.TransformDirection(dir);
        //transform.position += dir * MoveSpeed * Time.deltaTime;

        // 수직이동 계산
        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        if (cc.collisionFlags == CollisionFlags.CollidedBelow)
        {
            yPos = 0;
            currentJumpCount = 0;
        }
        if (Input.GetButtonDown("Jump") && currentJumpCount < MaxJumpCounter)
        {
            yPos = jumpPower;
            currentJumpCount++;
        }
        dir.y = yPos;

        // run이 눌렸을때 스피드가 2배 되어야한다.
        if (Input.GetButton("Run")) 
        {
            cc.Move(dir * (MoveSpeed * 2) * Time.deltaTime);
        }
        else
        {
            cc.Move(dir * (MoveSpeed * 1) * Time.deltaTime);
        }
    
        if(Input.GetButtonDown("Dodge"))
        {
            cc.Move(dir * (MoveSpeed * 5) * Time.deltaTime);
            isDodge = true;

            if(isDodge == true)
            {
                yPos = 0;
                currentJumpCount = 0;

            }
            
        }
    }
  
    void rotate()
    {
        float mousex = Input.GetAxis("Mouse X");
        float mousey = Input.GetAxis("Mouse Y");

        rotX += mousey * rotSpeed * Time.deltaTime;
        rotY += mousex * rotSpeed * Time.deltaTime;

        if (rotX > 80)
        {
            rotX = 80.0f;
        }
        else if (rotX > -80)
        {
            rotX = -80.0f;
        }
        transform.eulerAngles = new Vector3(0, rotY, 0);

    }

    


    //void MoveAni()
    //{

    //}

    //void Idle()
    //{
    //    if(Vector3.Distance(transform.position, PlayerMove) < findDistance)
    //    {

    //    }
    //}
}


