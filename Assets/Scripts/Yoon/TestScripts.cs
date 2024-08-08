using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;

public class TestScripts : MonoBehaviour, IState, IInput
{
    public bool hiding;
    public int hpCount = 20;
    public float moveSpeed = 5;
    float h, v;
    Vector3 moveVec; // �̵� ����
    public Transform cameraTransform; // ī�޶� Transform

    void Awake()
    {
        hiding = true;
        EnterState();
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        Move();
    }



    void HidingChange()
    {
        hiding = !hiding;
    }

    public void EnterState()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    public void UpdateState()
    {
        InputFuc();
    }

    public void ExitState()
    {

    }

    private void OnDisable()
    {
        ExitState();
    }
    // �⺻ ������ ó��
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // ���� �Է� �� ��������
        v = Input.GetAxisRaw("Vertical"); // ���� �Է� �� ��������


        //// ī�޶��� forward�� right ���� ��������
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        //// y�� �� 0���� �����Ͽ� ���� �̵��� ���
        cameraForward.y = 0;
        cameraRight.y = 0;

        //// ���� ����ȭ
        cameraForward.Normalize();
        cameraRight.Normalize();

        //// �̵� ���� ���
        moveVec = (cameraForward * v + cameraRight * h).normalized;

        transform.position += moveVec * moveSpeed * Time.deltaTime; // �ȱ� �ӵ��� �̵�
    }
    public void InputFuc()
    {
        
    }

}
