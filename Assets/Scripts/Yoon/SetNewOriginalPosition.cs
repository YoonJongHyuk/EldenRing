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
}
