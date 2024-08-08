using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target; // ī�޶� ���� ��� ������Ʈ.
    [SerializeField] Vector3 offset; // ī�޶�� ��� ������ �Ÿ� �� ��ġ ������.
    [SerializeField] Vector2 clampAxis = new Vector2(60, 60); // ī�޶��� ���� ȸ�� ������ �����ϴ� ��.

    [SerializeField] float follow_smoothing = 5; // ī�޶� ����� ���� ���� �ε巯�� ����.
    [SerializeField] float rotate_Smoothing = 5; // ī�޶��� ȸ�� �� �ε巯�� ����.
    [SerializeField] float senstivity = 60; // ���콺 �̵��� ���� ī�޶��� ���� �ΰ���.

    float rotX, rotY; // ī�޶��� ���� ȸ�� ����.
    bool cursorLocked = false; // Ŀ���� ����ִ��� ���θ� ��Ÿ���� �÷���.
    Transform cam; // ���� ī�޶��� Transform.

    public bool lockedTarget; // ī�޶� ����� ����� ���θ� �����ϴ� ����.

    void Start()
    {
        // Ŀ���� ����� ȭ�� �߾ӿ� ��޴ϴ�.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main.transform; // ���� ī�޶��� Transform�� �����ɴϴ�.
    }

    void Update()
    {
        // ����� �������� ī�޶��� ���ϴ� ��ġ�� ����մϴ�.
        Vector3 target_P = target.position + offset;
        // ī�޶� �ε巴�� ���ϴ� ��ġ�� �̵���ŵ�ϴ�.
        transform.position = Vector3.Lerp(transform.position, target_P, follow_smoothing * Time.deltaTime);

        // ������� �Է¿� ���� ī�޶� ȸ����Ű�ų� ����� �ٶ󺸰� �մϴ�.
        if (!lockedTarget)
            CameraTargetRotation();
        else
            LookAtTarget();

        // Escape Ű�� ������ �� Ŀ���� ���ü� �� ��� ���¸� ��ȯ�մϴ�.
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            if (cursorLocked)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
            else
            {
                Cursor.visible = false;
                Cursor.lockState = CursorLockMode.Locked;
            }
            cursorLocked = !cursorLocked; // Ŀ�� ��� ���¸� ����մϴ�.
        }
    }

    void CameraTargetRotation()
    {
        // ���콺 �̵� �Է��� �����ɴϴ�.
        Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // ���콺 �Է� �� �ΰ����� ���� ȸ�� ������ ������Ʈ�մϴ�.
        rotX += (mouseAxis.x * senstivity) * Time.deltaTime;
        rotY -= (mouseAxis.y * senstivity) * Time.deltaTime;

        // ���� ȸ�� ������ ���� �� ���� Ŭ�����մϴ�.
        rotY = Mathf.Clamp(rotY, clampAxis.x, clampAxis.y);

        // ī�޶��� ���ο� ȸ������ ����Ͽ� �ε巴�� �����մϴ�.
        Quaternion localRotation = Quaternion.Euler(rotY, rotX, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, localRotation, Time.deltaTime * rotate_Smoothing);
    }

    void LookAtTarget()
    {
        // ī�޶��� ȸ���� ���� ī�޶�� ���� �����մϴ�.
        transform.rotation = cam.rotation;
        Vector3 r = cam.eulerAngles;
        rotX = r.y; // ī�޶��� ���� ȸ�� ������ �����մϴ�.
        rotY = 1.8f; // ���� ȸ�� ������ ���������� �����մϴ�.
    }
}