using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMovve : MonoBehaviour
{
    public float MoveSpeed = 7.0f;
    public float rotSpeed = 200.0f;
    public float yVelocity = 2;
    public float jumpPower = 4;
    public int MaxJumpCounter = 1;

    float rotX;
    float rotY;
    float yPos;
    int currentJumpCount = 0;


    CharacterController cc;

    Vector3 gravityPower;

    void Start()
    {
        rotX = transform.eulerAngles.x;
        rotY = transform.eulerAngles.y;

        cc = GetComponent<CharacterController>();

        gravityPower = Physics.gravity;
    }

    // Update is called once per frame
    void Update()
    {
        Move();
        Rotate();


    }
    void Move()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        dir = transform.TransformDirection(dir);
        dir.Normalize();
      

        yPos += gravityPower.y * yVelocity * Time.deltaTime;

        if(cc.collisionFlags == CollisionFlags.CollidedBelow)
        {
            yPos = 0;
            currentJumpCount = 0;
        }

        if(Input.GetButtonDown("Jump")&& currentJumpCount < MaxJumpCounter)
        {
            yPos = jumpPower;
            currentJumpCount++;
        }

        dir.y = yPos;

        cc.Move(dir * MoveSpeed * Time.deltaTime);
    }
    void Rotate()
    {
        float mouseX = Input.GetAxis("Mouse X");
        float mouseY = Input.GetAxis("Mouse Y");

        rotX += mouseY * rotSpeed * Time.deltaTime;
        rotY += mouseX * rotSpeed * Time.deltaTime;

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


}


