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
        // ���� ������ŭ Vector3 �迭 ũ�� ����
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
        // �� ������ ��ġ�� �� �����Ӹ��� monsterDir�� ������Ʈ
        for (int i = 0; i < Monster.Length; i++)
        {
            if (Monster[i] != null) // ��ü�� �����ϴ��� Ȯ��
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
            currentTarget = null; // �Ͽ� ����
        }
        else
        {
            currentTarget = monsterDir[monsterIndex++]; // ���ο� Ÿ�� �Ͽ�
            print(monsterIndex);
        }
    }

}
