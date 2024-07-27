using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public Vector3 offset;
    float rotX;
    float rotY;
    public float rotSpeed = 200.0f;
    
    void Update()
    {
        transform.position = target.position + offset;
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

        //  ĳ���͸� ���� ���ƾ��ϴµ� ĳ���Ͷ� ������ ���������� �������� �� farPos �ϳ� ����� �־�Ѱ�
        // �ϴ� �ӽ� 

    }
}
