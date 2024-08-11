using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{
    // ������ ����Ʈ�� ����� �� �ִ� ��ũ��Ʈ ����
    private static Respawn activeRespawnPoint;

    // ������ ����Ʈ�� �÷��̾ ������ �� ȣ��Ǵ� �޼���
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            activeRespawnPoint = this; // ���� ������ ����Ʈ�� ����
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
            // �⺻������ 0,0,0 ��ġ�� ��ȯ (Ȥ�� ���ϴ� �⺻ ������ ��ġ)
            return Vector3.zero;
        }
    }
}
