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
    Vector3 moveVec; // 이동 벡터
    public Transform cameraTransform; // 카메라 Transform

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
    // 기본 움직임 처리
    void Move()
    {
        h = Input.GetAxisRaw("Horizontal"); // 수평 입력 값 가져오기
        v = Input.GetAxisRaw("Vertical"); // 수직 입력 값 가져오기


        //// 카메라의 forward와 right 방향 가져오기
        Vector3 cameraForward = cameraTransform.forward;
        Vector3 cameraRight = cameraTransform.right;

        //// y축 값 0으로 설정하여 수평 이동만 고려
        cameraForward.y = 0;
        cameraRight.y = 0;

        //// 벡터 정규화
        cameraForward.Normalize();
        cameraRight.Normalize();

        //// 이동 벡터 계산
        moveVec = (cameraForward * v + cameraRight * h).normalized;

        transform.position += moveVec * moveSpeed * Time.deltaTime; // 걷기 속도로 이동
    }
    public void InputFuc()
    {
        
    }

}
