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
        // �÷��̾ ã�� ��ġ���� �޾� Ÿ������ ������.
        target = GameObject.Find("Player").transform;
    }
    void Update()
    {
       if(target != null )
        {
            // ī�޶��� ��ġ�� Ÿ�� Ʈ�������� ��ġ�� �����Ѵ�.
            if(!dynamicCam)
            {
                transform.position = target.position;
            }
            else
            {
                transform.position = Vector3.Lerp(transform.position, target.position, followSpeed * Time.deltaTime);
            }
            // ī�޶��� ���� ������ Ÿ���� ���� �������� �����Ѵ�
            transform.forward = target.forward;
            // ������� ���콺 ���� ȸ�� ���� x�� ȸ������ �ִ´�
            transform.eulerAngles = new Vector3(-rotX, transform.eulerAngles.y, transform.eulerAngles.z);
        }

    }
}
