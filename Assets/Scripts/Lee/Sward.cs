using System.Collections;
using Unity.VisualScripting;
using UnityEngine;
using yoon;

public class Sward : MonoBehaviour
{
    // 무기 유형을 나타내는 열거형
    public enum Type { Melee, Range };
    public Type type; // 무기 유형 (근접 또는 원거리)
    //public int damage; // 무기 피해량
    public CapsuleCollider meleeArea; // 근접 공격 범위
    public int attackPower = 10; // 공격력
    public int attackRange = 3; // 공격 범위
    public float rate; // 공격 속도
    public GameObject arrowPrefab; // 화살 오브젝트 프리팹
    public Transform arrowSpawnPosition; // 화살 발사 위치

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
