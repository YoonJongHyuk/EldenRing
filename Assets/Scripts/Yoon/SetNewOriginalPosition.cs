using Cinemachine.Utility;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetNewOriginalPosition : MonoBehaviour
{
    private Vector3 newOriginalPosition;

    void Start()
    {
        newOriginalPosition = transform.position;
    }

    // 애니메이션 이벤트에서 호출될 함수
    public void SetNewPositionAsOriginal()
    {
        newOriginalPosition = transform.position;
    }

    public Vector3 GetNewOriginalPosition()
    {
        return newOriginalPosition;
    }

    public void SeePlayer()
    {
        // 플레이어의 위치를 찾음
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = playerObject.transform.position;


        // 플레이어를 향한 방향 벡터 계산
        Vector3 directionToPlayer = playerPosition - newOriginalPosition;
        directionToPlayer.y = 0; // y축 회전을 위해 y값을 0으로 설정

        // 방향 벡터를 회전값으로 변환
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

        // 오브젝트의 y축 회전 설정
        transform.rotation = Quaternion.Euler(0, rotationToPlayer.eulerAngles.y, 0);
    }
}
