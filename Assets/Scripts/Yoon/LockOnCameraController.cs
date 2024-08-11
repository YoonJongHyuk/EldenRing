using System.Collections.Generic;
using UnityEngine;
using Cinemachine;

public class LockOnCameraController : MonoBehaviour
{
    public CinemachineFreeLook freeLookCamera; // Cinemachine FreeLook ī�޶�
    public Transform player; // �÷��̾��� Transform
    public float cameraSlack = 0.1f; // ī�޶��� �ε巯�� �̵� ����
    public float cameraDistance = 5f; // ī�޶� ���� �÷��̾�� ������ �Ÿ�

    private Vector3 pivotPoint; // ī�޶��� �߽� ����Ʈ
    private List<Transform> monsterTargets = new List<Transform>(); // ���� Ÿ�� ����Ʈ
    private Transform currentTarget; // ���� �Ͽµ� Ÿ��
    private int currentTargetIndex = -1; // ���� Ÿ�� �ε��� (-1�̸� �Ͽ� ���� ����)

    void Start()
    {
        // ������ ���۵� �� ī�޶� �÷��̾ �ٶ󺸵��� ����
        freeLookCamera.LookAt = player;
        freeLookCamera.Follow = player;

        // Ÿ�� ����Ʈ�� ������Ʈ
        UpdateMonsterTargets();

        // ī�޶��� �ʱ� ��ġ ����
        pivotPoint = transform.position;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ToggleLockOn();
        }

        if (currentTarget != null)
        {
            // FreeLook ī�޶� ��Ȱ��ȭ
            freeLookCamera.enabled = false;

            // ī�޶��� ��ġ�� �ε巴�� ������Ʈ
            Vector3 current = pivotPoint;
            Vector3 target = player.transform.position + Vector3.up;
            pivotPoint = Vector3.MoveTowards(current, target, Vector3.Distance(current, target) * cameraSlack);

            // ī�޶� ��ġ�� ���� ����
            transform.position = pivotPoint;
            transform.LookAt(monsterTargets[currentTargetIndex]); // ���� �Ͽµ� ���͸� �ٶ�
            transform.position -= transform.forward * cameraDistance;
        }
        else
        {
            // FreeLook ī�޶� Ȱ��ȭ
            freeLookCamera.enabled = true;
        }
    }

    void UpdateMonsterTargets()
    {
        // "Monster" �±׸� ���� ��� ������Ʈ���� ã���ϴ�.
        GameObject[] monsters = GameObject.FindGameObjectsWithTag("Monster");

        // ����Ʈ�� �ʱ�ȭ�ϰ� �� ������Ʈ�� Transform�� ����Ʈ�� �߰��մϴ�.
        monsterTargets.Clear();
        foreach (GameObject monster in monsters)
        {
            monsterTargets.Add(monster.transform);
        }
    }

    void ToggleLockOn()
    {
        if (monsterTargets.Count == 0)
        {
            currentTarget = null;
            currentTargetIndex = -1;
            freeLookCamera.LookAt = player; // Ÿ���� ������ �÷��̾ LookAt���� ����
            return;
        }

        // ���� Ÿ������ �Ͽ�, ������ Ÿ�� ���Ŀ��� �Ͽ� ����
        currentTargetIndex = (currentTargetIndex + 1) % monsterTargets.Count;

        if (currentTargetIndex >= monsterTargets.Count)
        {
            currentTarget = null; // �Ͽ� ����
            freeLookCamera.LookAt = player; // �Ͽ� ���� �� �÷��̾ LookAt���� ����
        }
        else
        {
            currentTarget = monsterTargets[currentTargetIndex]; // ���ο� Ÿ�� �Ͽ�
            freeLookCamera.LookAt = currentTarget; // ī�޶��� LookAt�� Ÿ������ ����
        }
    }
}
