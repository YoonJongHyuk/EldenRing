//using UnityEngine;
//using System.Collections.Generic;

//public class RespawnManager : MonoBehaviour
//{
//    public Transform respawnPoint; // Respawn 태그를 가진 오브젝트의 위치
//    private Vector3 savedPosition; // 저장된 위치
//    private static Dictionary<string, float[]> positionHashMap = new Dictionary<string, float[]>();

//    PlayerContorler Player;
    

//    void Update()
//    {
//        // 플레이어가 Respawn 태그를 가진 오브젝트와 충돌 중인지 확인
//        if (Input.GetKeyDown(KeyCode.E) && IsPlayerOnRespawnPoint())
//        {
//            // 현재 위치를 저장
//            SavePosition();
//        }

//        // 플레이어가 죽었을 때 (예: 다른 스크립트에서 호출)
//        if (IsDead == true)
//        {
//            LoadPosition();
//        }
//    }

//    private bool IsPlayerOnRespawnPoint()
//    {
//        // 플레이어와 Respawn 포인트 간의 거리 체크 (이 부분은 실제 구현에 맞게 조정)
//        return Vector3.Distance(transform.position, respawnPoint.position) < 1.0f;
//    }

//    private void SavePosition()
//    {
//        // 현재 위치를 float 배열로 변환
//        Vector3 currentPosition = transform.position;
//        float[] positionArray = new float[3] { currentPosition.x, currentPosition.y, currentPosition.z };

//        // 위치를 Hash128으로 저장
//        string hashKey = "playerRespawnPosition";
//        positionHashMap[hashKey] = positionArray;

//        Debug.Log("Position saved: x=" + positionArray[0] + ", y=" + positionArray[1] + ", z=" + positionArray[2]);
//    }

//    private void LoadPosition()
//    {

//        // 저장된 위치 불러오기
//        string hashKey = "playerRespawnPosition";
//        if (positionHashMap.TryGetValue(hashKey, out float[] positionArray))
//        {
//            Vector3 restoredPosition = new Vector3(positionArray[0], positionArray[1], positionArray[2]);
//            transform.position = restoredPosition;

//            Debug.Log("Position loaded: x=" + restoredPosition.x + ", y=" + restoredPosition.y + ", z=" + restoredPosition.z);
//        }
//        else
//        {
//            Debug.LogWarning("No saved position found!");
//        }
//    }
//}
