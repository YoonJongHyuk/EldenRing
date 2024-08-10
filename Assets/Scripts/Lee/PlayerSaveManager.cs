//using UnityEngine;
//using System.Collections.Generic;

//public class RespawnManager : MonoBehaviour
//{
//    public Transform respawnPoint; // Respawn �±׸� ���� ������Ʈ�� ��ġ
//    private Vector3 savedPosition; // ����� ��ġ
//    private static Dictionary<string, float[]> positionHashMap = new Dictionary<string, float[]>();

//    PlayerContorler Player;
    

//    void Update()
//    {
//        // �÷��̾ Respawn �±׸� ���� ������Ʈ�� �浹 ������ Ȯ��
//        if (Input.GetKeyDown(KeyCode.E) && IsPlayerOnRespawnPoint())
//        {
//            // ���� ��ġ�� ����
//            SavePosition();
//        }

//        // �÷��̾ �׾��� �� (��: �ٸ� ��ũ��Ʈ���� ȣ��)
//        if (IsDead == true)
//        {
//            LoadPosition();
//        }
//    }

//    private bool IsPlayerOnRespawnPoint()
//    {
//        // �÷��̾�� Respawn ����Ʈ ���� �Ÿ� üũ (�� �κ��� ���� ������ �°� ����)
//        return Vector3.Distance(transform.position, respawnPoint.position) < 1.0f;
//    }

//    private void SavePosition()
//    {
//        // ���� ��ġ�� float �迭�� ��ȯ
//        Vector3 currentPosition = transform.position;
//        float[] positionArray = new float[3] { currentPosition.x, currentPosition.y, currentPosition.z };

//        // ��ġ�� Hash128���� ����
//        string hashKey = "playerRespawnPosition";
//        positionHashMap[hashKey] = positionArray;

//        Debug.Log("Position saved: x=" + positionArray[0] + ", y=" + positionArray[1] + ", z=" + positionArray[2]);
//    }

//    private void LoadPosition()
//    {

//        // ����� ��ġ �ҷ�����
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
