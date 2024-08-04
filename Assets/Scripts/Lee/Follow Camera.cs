using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FollowCamera : MonoBehaviour
{
    public Transform target; // 따라갈 타겟
    public float followSpeed = 10.0f; // 따라가는 속도
    public float Sensitivity = 100.0f; // 마우스 감도
    public float ClampAngle = 70.0f; // 회전 제한 각도

    float rotX; // 카메라의 X축 회전 값
    float rotY; // 카메라의 Y축 회전 값

    public Transform realCamera; // 실제 카메라
    public Vector3 dirNormalized; // 정규화된 방향 벡터
    public Vector3 finalDir; // 최종 방향 벡터
    public float minDistance; // 최소 거리
    public float maxDistance; // 최대 거리
    public float finalDistance = 10f; // 최종 거리
    public float smoothness = 10f; // 부드러움 정도

    // 초기 설정
    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x; // 초기 X축 회전 값 설정
        rotY = transform.localRotation.eulerAngles.y; // 초기 Y축 회전 값 설정

        dirNormalized = realCamera.localPosition.normalized; // 카메라 위치를 정규화
        finalDistance = realCamera.localPosition.magnitude; // 카메라 위치의 크기 설정

        // 커서 잠금 및 숨기기 (주석 처리됨)
        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    // 매 프레임 업데이트
    private void Update()
    {
        rotX += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime; // 마우스 X축 입력으로 회전 값 증가
        rotY += Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime; // 마우스 Y축 입력으로 회전 값 증가

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle); // 회전 값을 제한 각도 내로 클램프
        Quaternion rot = Quaternion.Euler(rotY, rotX, 0); // 회전 값으로 쿼터니언 생성
        transform.rotation = rot; // 회전 적용
    }

    // 업데이트 이후 처리
    private void LateUpdate()
    {
        // target 변수가 할당되지 않았을 때 업데이트 중지
        if (target == null)
        {
            return;
        }

        // 타겟 위치로 부드럽게 이동
        transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);

        // 최종 방향 설정
        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit; // 레이캐스트 히트 정보

        // 레이캐스트를 사용하여 충돌 체크
        if (Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance); // 충돌한 경우 거리 조정
        }
        else
        {
            finalDistance = maxDistance; // 충돌하지 않은 경우 최대 거리로 설정
        }

        // 실제 카메라 위치를 부드럽게 조정
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
