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
}
