using System.Collections;
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


    // 무기 사용 메서드
    public void Use()
    {
        // 게임 오브젝트가 활성화된 상태인지 확인
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

            StopAllCoroutines(); // 기존의 모든 코루틴 중지
            StartCoroutine(Swing()); // 새로운 Swing 코루틴 시작
        }
        else if (type == Type.Range)
        {
            if (arrowPrefab == null || arrowSpawnPosition == null)
            {
                Debug.LogWarning("Arrow prefab or spawn position is not assigned!");
                return;
            }

            StopAllCoroutines(); // 기존의 모든 코루틴 중지
            StartCoroutine(Shot()); // 새로운 Shot 코루틴 시작
        }
    }

    // Swing 코루틴 - 근접 공격 처리
    IEnumerator Swing()
    {
        meleeArea.enabled = true; // 공격 범위 활성화

        yield return new WaitForSeconds(0.1f); // 0.1초 대기
        meleeArea.enabled = false; // 공격 범위 비활성화
    }

    // Shot 코루틴 - 원거리 공격 처리
    IEnumerator Shot()
    {
        GameObject instantiatedArrow = Instantiate(arrowPrefab, arrowSpawnPosition.position, arrowSpawnPosition.rotation); // 화살 생성
        Rigidbody arrowRigidBody = instantiatedArrow.GetComponent<Rigidbody>(); // 화살의 리지드바디 컴포넌트 가져오기
        arrowRigidBody.velocity = arrowSpawnPosition.forward * 50; // 화살 속도 설정

        yield return null; // 다음 프레임까지 대기
    }

}
