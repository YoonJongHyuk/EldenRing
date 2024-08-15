using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class LockOnCamera : MonoBehaviour
{
    public Camera lockOnCamera;

    GameObject[] Monster;

    Vector3 dir;
    Transform currentTarget;
    Transform[] monsterDir;

    int monsterIndex;

    // Start is called before the first frame update
    void Start()
    {
        // 몬스터 개수만큼 Vector3 배열 크기 지정
        monsterDir = new Transform[Monster.Length];
    }

    // Update is called once per frame
    void Update()
    {
        dir = transform.position;
        MonsterDir();
        if(Input.GetKeyDown(KeyCode.Q))
        {
            ToggleLockOn();
        }
    }

    void MonsterDir()
    {
        // 각 몬스터의 위치를 매 프레임마다 monsterDir에 업데이트
        for (int i = 0; i < Monster.Length; i++)
        {
            if (Monster[i] != null) // 객체가 존재하는지 확인
            {
                monsterDir[i] = Monster[i].transform;
            }
        }
    }

    void ToggleLockOn()
    {
        if (monsterDir.Length == 0)
        {
            currentTarget = null;
            return;
        }

        if (monsterIndex >= monsterDir.Length)
        {
            currentTarget = null; // 록온 해제
        }
        else
        {
            currentTarget = monsterDir[monsterIndex++]; // 새로운 타겟 록온
            print(monsterIndex);
        }
    }

}
