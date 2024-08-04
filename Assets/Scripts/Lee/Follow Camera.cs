using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // ���� Ÿ��
    public float followSpeed = 10.0f; // ���󰡴� �ӵ�
    public float Sensitivity = 100.0f; // ���콺 ����
    public float ClampAngle = 70.0f; // ȸ�� ���� ����

    float rotX; // ī�޶��� X�� ȸ�� ��
    float rotY; // ī�޶��� Y�� ȸ�� ��

    public Transform realCamera; // ���� ī�޶�
    public Vector3 dirNormalized; // ����ȭ�� ���� ����
    public Vector3 finalDir; // ���� ���� ����
    public float minDistance; // �ּ� �Ÿ�
    public float maxDistance; // �ִ� �Ÿ�
    public float finalDistance = 10f; // ���� �Ÿ�
    public float smoothness = 10f; // �ε巯�� ����

    // �ʱ� ����
    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x; // �ʱ� X�� ȸ�� �� ����
        rotY = transform.localRotation.eulerAngles.y; // �ʱ� Y�� ȸ�� �� ����

        dirNormalized = realCamera.localPosition.normalized; // ī�޶� ��ġ�� ����ȭ
        finalDistance = realCamera.localPosition.magnitude; // ī�޶� ��ġ�� ũ�� ����

        // Ŀ�� ��� �� ����� (�ּ� ó����)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // �� ������ ������Ʈ
    private void Update()
    {
        rotX += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime; // ���콺 X�� �Է����� ȸ�� �� ����
        rotY += Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime; // ���콺 Y�� �Է����� ȸ�� �� ����

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle); // ȸ�� ���� ���� ���� ���� Ŭ����
        Quaternion rot = Quaternion.Euler(rotY, rotX, 0); // ȸ�� ������ ���ʹϾ� ����
        transform.rotation = rot; // ȸ�� ����
    }

    // ������Ʈ ���� ó��
    private void LateUpdate()
    {
        // target ������ �Ҵ���� �ʾ��� �� ������Ʈ ����
        if (target == null)
        {
            return;
        }

        // Ÿ�� ��ġ�� �ε巴�� �̵�
        transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);

        // ���� ���� ����
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit; // ����ĳ��Ʈ ��Ʈ ����

        // ����ĳ��Ʈ�� ����Ͽ� �浹 üũ
        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance); // �浹�� ��� �Ÿ� ����
        }
        else
        {
            finalDistance = maxDistance; // �浹���� ���� ��� �ִ� �Ÿ��� ����
        }

        // ���� ī�޶� ��ġ�� �ε巴�� ����
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
