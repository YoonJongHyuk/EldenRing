using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockOnCameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Cinemachine FreeLook 카메라
    public Transform player; // 플레이어의 Transform
    public float cameraSlack = 0.1f; // 카메라의 부드러운 이동 정도
    public float cameraDistance = 5f; // 카메라가 적과 플레이어에서 떨어질 거리

    private Vector3 pivotPoint; // 카메라의 중심 포인트
    private List<Transform> monsterTargets = new List<Transform>(); // 몬스터 타겟 리스트
    private Transform currentTarget; // 현재 록온된 타겟
    private int currentTargetIndex = -1; // 현재 타겟 인덱스 (-1이면 록온 해제 상태)

    void Start()
    {
        // 게임이 시작될 때 카메라가 플레이어를 바라보도록 설정
        freeLookCamera.LookAt = player;
        freeLookCamera.Follow = player;

        // 타겟 리스트를 업데이트
        UpdateMonsterTargets();

        // 카메라의 초기 위치 설정
        pivotPoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleLockOn();
        }

        if (currentTarget != null)
        {
            // FreeLook 카메라 비활성화
            freeLookCamera.enabled = false;

            // 카메라의 위치를 부드럽게 업데이트
            Vector3 current = pivotPoint;
            Vector3 target = player.transform.position + Vector3.up;
            pivotPoint = Vector3.MoveTowards(current, target, Vector3.Distance(current, target) * cameraSlack);

            // 카메라 위치와 방향 설정
            transform.position = pivotPoint;
            transform.LookAt(monsterTargets[currentTargetIndex]); // 현재 록온된 몬스터를 바라봄
            transform.position -= transform.forward * cameraDistance;
        }
        else
        {
            // FreeLook 카메라 활성화
            freeLookCamera.enabled = true;
        }
    }

    void UpdateMonsterTargets()
    {
        // "Monster" 태그를 가진 모든 오브젝트들을 찾습니다.
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        // 리스트를 초기화하고 각 오브젝트의 Transform을 리스트에 추가합니다.
        monsterTargets.Clear();
        foreach (GameObject monster in monsters)
        {
            monsterTargets.Add(monster.transform);
        }
    }

    void ToggleLockOn()
    {
        if (monsterTargets.Count == 0)
        {
            currentTarget = null;
            currentTargetIndex = -1;
            freeLookCamera.LookAt = player; // 타겟이 없으면 플레이어를 LookAt으로 설정
            return;
        }

        // 다음 타겟으로 록온, 마지막 타겟 이후에는 록온 해제
        currentTargetIndex = (currentTargetIndex + 1) % monsterTargets.Count;

        if (currentTargetIndex >= monsterTargets.Count)
        {
            currentTarget = null; // 록온 해제
            freeLookCamera.LookAt = player; // 록온 해제 시 플레이어를 LookAt으로 설정
        }
        else
        {
            currentTarget = monsterTargets[currentTargetIndex]; // 새로운 타겟 록온
            freeLookCamera.LookAt = currentTarget; // 카메라의 LookAt을 타겟으로 설정
        }
    }
}
