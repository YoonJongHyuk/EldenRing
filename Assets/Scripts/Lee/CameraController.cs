using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CamRotate_4th : MonoBehaviour
{
    public float rotSpeed = 350.0f;
    public Transform target;
    public float angleLimit = 80;

    float my = 0;

    void Start()
    {
        transform.rotation = target.rotation;
    }

    void LateUpdate()
    {
        FollowTarget(target);

        // 마우스 상하 회전 함수
        RotatePitch();
    }

    void FollowTarget(Transform target)
    {
        transform.position = target.position;
    }

    void RotatePitch()
    {
       
        float mouseY = Input.GetAxis("Mouse Y");

        my += mouseY * rotSpeed * Time.deltaTime;

        if (my > angleLimit)
        {
            my = angleLimit;
        }
        else if (my < -angleLimit)
        {
            my = -angleLimit;
        }
        transform.eulerAngles = new Vector3(my, 0, 0);


    }

}
