using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    [SerializeField] Transform target; // 카메라가 따라갈 대상 오브젝트.
    [SerializeField] Vector3 offset; // 카메라와 대상 사이의 거리 및 위치 오프셋.
    [SerializeField] Vector2 clampAxis = new Vector2(60, 60); // 카메라의 수직 회전 범위를 제한하는 값.

    [SerializeField] float follow_smoothing = 5; // 카메라가 대상을 따라갈 때의 부드러움 정도.
    [SerializeField] float rotate_Smoothing = 5; // 카메라의 회전 시 부드러움 정도.
    [SerializeField] float senstivity = 60; // 마우스 이동에 대한 카메라의 반응 민감도.

    float rotX, rotY; // 카메라의 현재 회전 각도.
    bool cursorLocked = false; // 커서가 잠겨있는지 여부를 나타내는 플래그.
    Transform cam; // 메인 카메라의 Transform.

    public bool lockedTarget; // 카메라가 대상을 잠글지 여부를 결정하는 변수.

    void Start()
    {
        // 커서를 숨기고 화면 중앙에 잠급니다.
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
        cam = Camera.main.transform; // 메인 카메라의 Transform을 가져옵니다.
    }

    void Update()
    {
        // 대상을 기준으로 카메라의 원하는 위치를 계산합니다.
        Vector3 target_P = target.position + offset;
        // 카메라를 부드럽게 원하는 위치로 이동시킵니다.
        transform.position = Vector3.Lerp(transform.position, target_P, follow_smoothing * Time.deltaTime);

        // 사용자의 입력에 따라 카메라를 회전시키거나 대상을 바라보게 합니다.
        if (!lockedTarget)
            CameraTargetRotation();
        else
            LookAtTarget();

        // Escape 키를 눌렀을 때 커서의 가시성 및 잠금 상태를 전환합니다.
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
            cursorLocked = !cursorLocked; // 커서 잠금 상태를 토글합니다.
        }
    }

    void CameraTargetRotation()
    {
        // 마우스 이동 입력을 가져옵니다.
        Vector2 mouseAxis = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));
        // 마우스 입력 및 민감도에 따라 회전 각도를 업데이트합니다.
        rotX += (mouseAxis.x * senstivity) * Time.deltaTime;
        rotY -= (mouseAxis.y * senstivity) * Time.deltaTime;

        // 수직 회전 각도를 제한 값 내로 클램프합니다.
        rotY = Mathf.Clamp(rotY, clampAxis.x, clampAxis.y);

        // 카메라의 새로운 회전값을 계산하여 부드럽게 적용합니다.
        Quaternion localRotation = Quaternion.Euler(rotY, rotX, 0);
        transform.rotation = Quaternion.Slerp(transform.rotation, localRotation, Time.deltaTime * rotate_Smoothing);
    }

    void LookAtTarget()
    {
        // 카메라의 회전을 메인 카메라와 같게 설정합니다.
        transform.rotation = cam.rotation;
        Vector3 r = cam.eulerAngles;
        rotX = r.y; // 카메라의 수평 회전 각도를 저장합니다.
        rotY = 1.8f; // 수직 회전 각도를 고정값으로 설정합니다.
    }
}