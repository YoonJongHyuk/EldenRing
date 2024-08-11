using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // 리스폰 포인트를 기억할 수 있는 스크립트 변수
    private static Respawn activeRespawnPoint;

    // 리스폰 포인트에 플레이어가 들어왔을 때 호출되는 메서드
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeRespawnPoint = this; // 현재 리스폰 포인트를 저장
            Debug.Log("Respawn point activated: " + transform.position);
        }
    }

    public static Vector3 GetRespawnPosition()
    {
        if (activeRespawnPoint != null)
        {
            return activeRespawnPoint.transform.position;
        }
        else
        {
            // 기본값으로 0,0,0 위치를 반환 (혹은 원하는 기본 리스폰 위치)
            return Vector3.zero;
        }
    }
}
