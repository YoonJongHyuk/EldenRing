using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLockOn : MonoBehaviour
{
    Transform currentTarget; // 현재 잠금 대상.
    Animator anim; // 캐릭터 애니메이터.

    [SerializeField] LayerMask targetLayers; // 잠금 가능한 적의 레이어.
    [SerializeField] Transform enemyTarget_Locator; // 적의 위치를 표시하는 위치.

    [Tooltip("StateDrivenMethod for Switching Cameras")]
    [SerializeField] Animator cinemachineAnimator; // 카메라 전환 애니메이터.

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look; // 수직 방향을 0으로 설정할지 여부.
    [SerializeField] float noticeZone = 10; // 적을 인식할 범위.
    [SerializeField] float lookAtSmoothing = 2; // 적을 바라볼 때의 부드러움 정도.
    [Tooltip("Angle_Degree")][SerializeField] float maxNoticeAngle = 60; // 적을 인식할 최대 각도.
    [SerializeField] float crossHair_Scale = 0.1f; // 조준선 크기 조절.

    Transform cam; // 메인 카메라의 Transform.
    bool enemyLocked; // 적이 잠금 상태인지 여부.
    float currentYOffset; // 적의 높이 조정값.
    Vector3 pos; // 적의 위치.

    [SerializeField] CameraFollow camFollow; // 카메라를 따라가는 스크립트.
    [SerializeField] Transform lockOnCanvas; // 잠금 상태일 때 표시할 UI 캔버스.
    DefMovement defMovement; // 캐릭터 이동 스크립트.

    void Start()
    {
        defMovement = GetComponent<DefMovement>(); // 캐릭터 이동 스크립트를 가져옵니다.
        anim = GetComponent<Animator>(); // 애니메이터를 가져옵니다.
        cam = Camera.main.transform; // 메인 카메라의 Transform을 가져옵니다.
        lockOnCanvas.gameObject.SetActive(false); // 잠금 상태 UI를 비활성화합니다.
    }

    void Update()
    {
        camFollow.lockedTarget = enemyLocked; // 카메라가 적을 잠글지 여부를 설정합니다.
        defMovement.lockMovement = enemyLocked; // 캐릭터 이동 잠금 여부를 설정합니다.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentTarget)
            {
                // 이미 대상이 있으면 리셋합니다.
                ResetTarget();
                return;
            }

            if (currentTarget = ScanNearBy())
                FoundTarget();
            else
                ResetTarget();
        }

        if (enemyLocked)
        {
            if (!TargetOnRange())
                ResetTarget(); // 적이 범위를 벗어나면 리셋합니다.
            LookAtTarget(); // 적을 바라봅니다.
        }
    }

    void FoundTarget()
    {
        lockOnCanvas.gameObject.SetActive(true); // 잠금 상태 UI를 활성화합니다.
        anim.SetLayerWeight(1, 1); // 애니메이터 레이어의 가중치를 설정합니다.
        cinemachineAnimator.Play("TargetCamera"); // 적을 위한 카메라 애니메이션을 재생합니다.
        enemyLocked = true; // 적이 잠금 상태로 설정됩니다.
    }

    void ResetTarget()
    {
        lockOnCanvas.gameObject.SetActive(false); // 잠금 상태 UI를 비활성화합니다.
        currentTarget = null; // 현재 대상을 null로 설정합니다.
        enemyLocked = false; // 적 잠금 상태를 해제합니다.
        anim.SetLayerWeight(1, 0); // 애니메이터 레이어의 가중치를 설정합니다.
        cinemachineAnimator.Play("FollowCamera"); // 일반 카메라 애니메이션을 재생합니다.
    }

    private Transform ScanNearBy()
    {
        // 범위 내의 적들을 검사합니다.
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle; // 가장 가까운 각도를 초기화합니다.
        Transform closestTarget = null; // 가장 가까운 적의 Transform을 초기화합니다.
        if (nearbyTargets.Length <= 0) return null; // 범위 내에 적이 없으면 null을 반환합니다.

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            // 카메라와 적 사이의 방향 벡터를 계산합니다.
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0; // 수직 방향은 무시합니다.
            float _angle = Vector3.Angle(cam.forward, dir); // 카메라의 정면 방향과 적 사이의 각도를 계산합니다.

            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform; // 가장 가까운 적으로 설정합니다.
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null; // 가장 가까운 적이 없으면 null을 반환합니다.

        // 적의 높이를 계산합니다.
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3)
            currentYOffset = 1.6f; // 수직 방향이 설정된 범위에 있으면 고정값으로 설정합니다.
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null; // 적이 가려져 있으면 null을 반환합니다.
        return closestTarget; // 가장 가까운 적을 반환합니다.
    }

    bool Blocked(Vector3 t)
    {
        RaycastHit hit;
        // 현재 위치에서 적 위치까지의 선을 캐스팅하여 장애물이 있는지 검사합니다.
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
        {
            if (!hit.transform.CompareTag("Monster")) return true; // 적이 아니면 가려진 것으로 간주합니다.
        }
        return false; // 장애물이 없으면 false를 반환합니다.
    }

    bool TargetOnRange()
    {
        // 현재 위치와 적의 위치 사이의 거리를 계산합니다.
        float dis = (transform.position - pos).magnitude;
        if (dis / 2 > noticeZone) return false; // 거리가 범위를 초과하면 false를 반환합니다.
        return true; // 범위 내에 있으면 true를 반환합니다.
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget(); // 현재 대상이 없으면 리셋합니다.
            return;
        }
        pos = currentTarget.position + new Vector3(0, currentYOffset, 0); // 적의 위치를 설정합니다.
        lockOnCanvas.position = pos; // UI 캔버스의 위치를 적의 위치로 설정합니다.
        lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale); // 조준선 크기를 설정합니다.

        enemyTarget_Locator.position = pos; // 적 위치 표시기의 위치를 설정합니다.
        Vector3 dir = currentTarget.position - transform.position; // 캐릭터와 적 사이의 방향 벡터를 계산합니다.
        dir.y = 0; // 수직 방향은 무시합니다.
        Quaternion rot = Quaternion.LookRotation(dir); // 적 방향을 바라보는 회전을 계산합니다.
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing); // 캐릭터의 회전을 부드럽게 적용합니다.
    }

    private void OnDrawGizmos()
    {
        // 에디터에서 인식 범위를 시각화합니다.
        Gizmos.DrawWireSphere(transform.position, noticeZone);
    }
}