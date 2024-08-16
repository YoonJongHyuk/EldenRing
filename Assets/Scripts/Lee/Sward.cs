using System.Collections;
using Unity.VisualScripting;
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

    private void Update()
    {
        PowerAttack();
    }


    public void PowerAttack()
    {
        if(Input.GetButtonDown("PowerAttack"))
        {
            attackPower = 20;
            print("20");
        }
        else
        {
            attackPower = 10;
            print("10");
        }
    }
}
