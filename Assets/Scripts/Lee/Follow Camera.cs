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

        //  캐릭터를 따라 돌아야하는데 캐릭터랑 별개로 같은값으로 돌고있음 즉 farPos 하나 만들어 넣어둘것
        // 일단 임시 

    }
}
