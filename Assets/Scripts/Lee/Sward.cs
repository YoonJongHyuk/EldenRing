using System.Collections;
using UnityEngine;
using yoon;

public class Sward : MonoBehaviour
{
    // ���� ������ ��Ÿ���� ������
    public enum Type { Melee, Range };
    public Type type; // ���� ���� (���� �Ǵ� ���Ÿ�)
    //public int damage; // ���� ���ط�
    public CapsuleCollider meleeArea; // ���� ���� ����
    public int attackPower = 10; // ���ݷ�
    public int attackRange = 3; // ���� ����
    public float rate; // ���� �ӵ�
    public GameObject arrowPrefab; // ȭ�� ������Ʈ ������
    public Transform arrowSpawnPosition; // ȭ�� �߻� ��ġ


    // ���� ��� �޼���
    public void Use()
    {
        // ���� ������Ʈ�� Ȱ��ȭ�� �������� Ȯ��
        if (!gameObject.activeInHierarchy)
        {
            Debug.LogWarning("Sward game object is inactive!");
            return;
        }

        if (type == Type.Melee)
        {
            if (meleeArea == null)
            {
                Debug.LogWarning("Melee area is not assigned!");
                return;
            }

            StopAllCoroutines(); // ������ ��� �ڷ�ƾ ����
            StartCoroutine(Swing()); // ���ο� Swing �ڷ�ƾ ����
        }
        else if (type == Type.Range)
        {
            if (arrowPrefab == null || arrowSpawnPosition == null)
            {
                Debug.LogWarning("Arrow prefab or spawn position is not assigned!");
                return;
            }

            StopAllCoroutines(); // ������ ��� �ڷ�ƾ ����
            StartCoroutine(Shot()); // ���ο� Shot �ڷ�ƾ ����
        }
    }

    // Swing �ڷ�ƾ - ���� ���� ó��
    IEnumerator Swing()
    {
        meleeArea.enabled = true; // ���� ���� Ȱ��ȭ

        yield return new WaitForSeconds(0.1f); // 0.1�� ���
        meleeArea.enabled = false; // ���� ���� ��Ȱ��ȭ
    }

    // Shot �ڷ�ƾ - ���Ÿ� ���� ó��
    IEnumerator Shot()
    {
        GameObject instantiatedArrow = Instantiate(arrowPrefab, arrowSpawnPosition.position, arrowSpawnPosition.rotation); // ȭ�� ����
        Rigidbody arrowRigidBody = instantiatedArrow.GetComponent<Rigidbody>(); // ȭ���� ������ٵ� ������Ʈ ��������
        arrowRigidBody.velocity = arrowSpawnPosition.forward * 50; // ȭ�� �ӵ� ����

        yield return null; // ���� �����ӱ��� ���
    }

}
