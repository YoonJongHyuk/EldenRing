using UnityEngine;

public class LookAtYOnly : MonoBehaviour
{
    Transform target; // �ٶ� ���
    private Quaternion initialRotation;

    void Start()
    {
        // ������Ʈ�� �ʱ� ȸ������ ����
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // Ÿ���� ���� ���� ���� ���
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Y���� �������� ���� ���⸸ ���

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, targetRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        }
    }
}
