using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowCamera : MonoBehaviour
{
    public Transform target;
    public float rotX;
    public bool dynamicCam = true;
    public float followSpeed = 3.0f;

    private void Start()
    {
        // 플레이어를 찾고 위치값을 받아 타겟으로 덮어씌운다.
        target = GameObject.Find("Player").transform;
    }
    void Update()
    {
       if(target != null )
        {
            // 카메라의 위치를 타겟 트랜스폼의 위치로 지정한다.
            if(!dynamicCam)
            {
                transform.position = target.position;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
            }
            // 카메라의 정면 방향을 타겟의 정면 방향으로 설정한다
            transform.forward = target.forward;
            // 사용자의 마우스 상하 회전 값을 x축 회전으로 넣는다
            transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y, transform.eulerAngles.z);
        }

    }
}
