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

    // �ִϸ��̼� �̺�Ʈ���� ȣ��� �Լ�
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
        // �÷��̾��� ��ġ�� ã��
        GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
        Vector3 playerPosition = playerObject.transform.position;


        // �÷��̾ ���� ���� ���� ���
        Vector3 directionToPlayer = playerPosition - newOriginalPosition;
        directionToPlayer.y = 0; // y�� ȸ���� ���� y���� 0���� ����

        // ���� ���͸� ȸ�������� ��ȯ
        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);

        // ������Ʈ�� y�� ȸ�� ����
        transform.rotation = Quaternion.Euler(0, rotationToPlayer.eulerAngles.y, 0);
    }
}
