using UnityEngine;

public class LookAtYOnly : MonoBehaviour
{
    Transform target; // 바라볼 대상
    private Quaternion initialRotation;

    void Start()
    {
        // 오브젝트의 초기 회전값을 저장
        initialRotation = transform.rotation;
    }

    void Update()
    {
        // 타겟을 향한 방향 벡터 계산
        Vector3 direction = target.position - transform.position;
        direction.y = 0; // Y축을 고정시켜 수평 방향만 고려

        if (direction != Vector3.zero)
        {
            Quaternion targetRotation = Quaternion.LookRotation(direction);
            transform.rotation = Quaternion.Euler(initialRotation.eulerAngles.x, targetRotation.eulerAngles.y, initialRotation.eulerAngles.z);
        }
    }
}
