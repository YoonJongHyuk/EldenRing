using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DefMovement : MonoBehaviour
{
    CharacterController controller; // 캐릭터 이동을 제어하는 CharacterController.
    Animator anim; // 캐릭터 애니메이션을 제어하는 Animator.
    Transform cam; // 메인 카메라의 Transform.

    float speedSmoothVelocity; // 속도 보정에 사용되는 변수.
    float speedSmoothTime; // 속도 보정 시간.
    float currentSpeed; // 현재 속도.
    float velocityY; // 수직 속도 (중력 등).
    Vector3 moveInput; // 사용자의 이동 입력.
    Vector3 dir; // 이동 방향.

    [Header("Settings")]
    [SerializeField] float gravity = 25f; // 중력의 크기.
    [SerializeField] float moveSpeed = 2f; // 이동 속도.
    [SerializeField] float rotateSpeed = 3f; // 회전 속도.

    public bool lockMovement; // 이동 잠금 여부.

    void Start()
    {
        anim = GetComponent<Animator>(); // Animator 컴포넌트를 가져옵니다.
        controller = GetComponent<CharacterController>(); // CharacterController 컴포넌트를 가져옵니다.
        cam = Camera.main.transform; // 메인 카메라의 Transform을 가져옵니다.
    }

    void Update()
    {
        GetInput(); // 사용자 입력을 받아옵니다.
        PlayerMovement(); // 캐릭터 이동을 처리합니다.
        if (!lockMovement) PlayerRotation(); // 이동 잠금이 없을 경우 캐릭터 회전을 처리합니다.
    }

    private void GetInput()
    {
        // 사용자 입력을 받아옵니다.
        moveInput = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"));

        // 카메라의 정면 방향과 오른쪽 방향을 가져옵니다.
        Vector3 forward = cam.forward;
        Vector3 right = cam.right;
        forward.y = 0; // Y축 방향은 무시합니다.
        right.y = 0; // Y축 방향은 무시합니다.
        forward.Normalize(); // 정규화하여 방향만 가져옵니다.
        right.Normalize(); // 정규화하여 방향만 가져옵니다.

        // 사용자의 입력에 따라 이동 방향을 계산합니다.
        dir = (forward * moveInput.y + right * moveInput.x).normalized;
    }

    private void PlayerMovement()
    {
        // 현재 속도를 부드럽게 보정합니다.
        currentSpeed = Mathf.SmoothDamp(currentSpeed, moveSpeed, ref speedSmoothVelocity, speedSmoothTime * Time.deltaTime);

        // 중력에 의한 수직 속도를 적용합니다.
        if (velocityY > -10) velocityY -= Time.deltaTime * gravity;
        Vector3 velocity = (dir * currentSpeed) + Vector3.up * velocityY;

        // 캐릭터를 이동시킵니다.
        controller.Move(velocity * Time.deltaTime);

        // 애니메이션 상태를 업데이트합니다.
        anim.SetFloat("Movement", dir.magnitude, 0.1f, Time.deltaTime);
        anim.SetFloat("Horizontal", moveInput.x, 0.1f, Time.deltaTime);
        anim.SetFloat("Vertical", moveInput.y, 0.1f, Time.deltaTime);
    }

    private void PlayerRotation()
    {
        // 이동 방향이 없으면 회전하지 않습니다.
        if (dir.magnitude == 0) return;

        // 이동 방향에 따라 캐릭터의 회전을 부드럽게 적용합니다.
        Vector3 rotDir = new Vector3(dir.x, dir.y, dir.z);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(rotDir), Time.deltaTime * rotateSpeed);
    }
}