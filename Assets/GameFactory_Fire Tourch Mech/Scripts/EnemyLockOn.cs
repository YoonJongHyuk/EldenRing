using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyLockOn : MonoBehaviour
{
    Transform currentTarget; // ���� ��� ���.
    Animator anim; // ĳ���� �ִϸ�����.

    [SerializeField] LayerMask targetLayers; // ��� ������ ���� ���̾�.
    [SerializeField] Transform enemyTarget_Locator; // ���� ��ġ�� ǥ���ϴ� ��ġ.

    [Tooltip("StateDrivenMethod for Switching Cameras")]
    [SerializeField] Animator cinemachineAnimator; // ī�޶� ��ȯ �ִϸ�����.

    [Header("Settings")]
    [SerializeField] bool zeroVert_Look; // ���� ������ 0���� �������� ����.
    [SerializeField] float noticeZone = 10; // ���� �ν��� ����.
    [SerializeField] float lookAtSmoothing = 2; // ���� �ٶ� ���� �ε巯�� ����.
    [Tooltip("Angle_Degree")][SerializeField] float maxNoticeAngle = 60; // ���� �ν��� �ִ� ����.
    [SerializeField] float crossHair_Scale = 0.1f; // ���ؼ� ũ�� ����.

    Transform cam; // ���� ī�޶��� Transform.
    bool enemyLocked; // ���� ��� �������� ����.
    float currentYOffset; // ���� ���� ������.
    Vector3 pos; // ���� ��ġ.

    [SerializeField] CameraFollow camFollow; // ī�޶� ���󰡴� ��ũ��Ʈ.
    [SerializeField] Transform lockOnCanvas; // ��� ������ �� ǥ���� UI ĵ����.
    DefMovement defMovement; // ĳ���� �̵� ��ũ��Ʈ.

    void Start()
    {
        defMovement = GetComponent<DefMovement>(); // ĳ���� �̵� ��ũ��Ʈ�� �����ɴϴ�.
        anim = GetComponent<Animator>(); // �ִϸ����͸� �����ɴϴ�.
        cam = Camera.main.transform; // ���� ī�޶��� Transform�� �����ɴϴ�.
        lockOnCanvas.gameObject.SetActive(false); // ��� ���� UI�� ��Ȱ��ȭ�մϴ�.
    }

    void Update()
    {
        camFollow.lockedTarget = enemyLocked; // ī�޶� ���� ����� ���θ� �����մϴ�.
        defMovement.lockMovement = enemyLocked; // ĳ���� �̵� ��� ���θ� �����մϴ�.
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            if (currentTarget)
            {
                // �̹� ����� ������ �����մϴ�.
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
                ResetTarget(); // ���� ������ ����� �����մϴ�.
            LookAtTarget(); // ���� �ٶ󺾴ϴ�.
        }
    }

    void FoundTarget()
    {
        lockOnCanvas.gameObject.SetActive(true); // ��� ���� UI�� Ȱ��ȭ�մϴ�.
        anim.SetLayerWeight(1, 1); // �ִϸ����� ���̾��� ����ġ�� �����մϴ�.
        cinemachineAnimator.Play("TargetCamera"); // ���� ���� ī�޶� �ִϸ��̼��� ����մϴ�.
        enemyLocked = true; // ���� ��� ���·� �����˴ϴ�.
    }

    void ResetTarget()
    {
        lockOnCanvas.gameObject.SetActive(false); // ��� ���� UI�� ��Ȱ��ȭ�մϴ�.
        currentTarget = null; // ���� ����� null�� �����մϴ�.
        enemyLocked = false; // �� ��� ���¸� �����մϴ�.
        anim.SetLayerWeight(1, 0); // �ִϸ����� ���̾��� ����ġ�� �����մϴ�.
        cinemachineAnimator.Play("FollowCamera"); // �Ϲ� ī�޶� �ִϸ��̼��� ����մϴ�.
    }

    private Transform ScanNearBy()
    {
        // ���� ���� ������ �˻��մϴ�.
        Collider[] nearbyTargets = Physics.OverlapSphere(transform.position, noticeZone, targetLayers);
        float closestAngle = maxNoticeAngle; // ���� ����� ������ �ʱ�ȭ�մϴ�.
        Transform closestTarget = null; // ���� ����� ���� Transform�� �ʱ�ȭ�մϴ�.
        if (nearbyTargets.Length <= 0) return null; // ���� ���� ���� ������ null�� ��ȯ�մϴ�.

        for (int i = 0; i < nearbyTargets.Length; i++)
        {
            // ī�޶�� �� ������ ���� ���͸� ����մϴ�.
            Vector3 dir = nearbyTargets[i].transform.position - cam.position;
            dir.y = 0; // ���� ������ �����մϴ�.
            float _angle = Vector3.Angle(cam.forward, dir); // ī�޶��� ���� ����� �� ������ ������ ����մϴ�.

            if (_angle < closestAngle)
            {
                closestTarget = nearbyTargets[i].transform; // ���� ����� ������ �����մϴ�.
                closestAngle = _angle;
            }
        }

        if (!closestTarget) return null; // ���� ����� ���� ������ null�� ��ȯ�մϴ�.

        // ���� ���̸� ����մϴ�.
        float h1 = closestTarget.GetComponent<CapsuleCollider>().height;
        float h2 = closestTarget.localScale.y;
        float h = h1 * h2;
        float half_h = (h / 2) / 2;
        currentYOffset = h - half_h;
        if (zeroVert_Look && currentYOffset > 1.6f && currentYOffset < 1.6f * 3)
            currentYOffset = 1.6f; // ���� ������ ������ ������ ������ ���������� �����մϴ�.
        Vector3 tarPos = closestTarget.position + new Vector3(0, currentYOffset, 0);
        if (Blocked(tarPos)) return null; // ���� ������ ������ null�� ��ȯ�մϴ�.
        return closestTarget; // ���� ����� ���� ��ȯ�մϴ�.
    }

    bool Blocked(Vector3 t)
    {
        RaycastHit hit;
        // ���� ��ġ���� �� ��ġ������ ���� ĳ�����Ͽ� ��ֹ��� �ִ��� �˻��մϴ�.
        if (Physics.Linecast(transform.position + Vector3.up * 0.5f, t, out hit))
        {
            if (!hit.transform.CompareTag("Monster")) return true; // ���� �ƴϸ� ������ ������ �����մϴ�.
        }
        return false; // ��ֹ��� ������ false�� ��ȯ�մϴ�.
    }

    bool TargetOnRange()
    {
        // ���� ��ġ�� ���� ��ġ ������ �Ÿ��� ����մϴ�.
        float dis = (transform.position - pos).magnitude;
        if (dis / 2 > noticeZone) return false; // �Ÿ��� ������ �ʰ��ϸ� false�� ��ȯ�մϴ�.
        return true; // ���� ���� ������ true�� ��ȯ�մϴ�.
    }

    private void LookAtTarget()
    {
        if (currentTarget == null)
        {
            ResetTarget(); // ���� ����� ������ �����մϴ�.
            return;
        }
        pos = currentTarget.position + new Vector3(0, currentYOffset, 0); // ���� ��ġ�� �����մϴ�.
        lockOnCanvas.position = pos; // UI ĵ������ ��ġ�� ���� ��ġ�� �����մϴ�.
        lockOnCanvas.localScale = Vector3.one * ((cam.position - pos).magnitude * crossHair_Scale); // ���ؼ� ũ�⸦ �����մϴ�.

        enemyTarget_Locator.position = pos; // �� ��ġ ǥ�ñ��� ��ġ�� �����մϴ�.
        Vector3 dir = currentTarget.position - transform.position; // ĳ���Ϳ� �� ������ ���� ���͸� ����մϴ�.
        dir.y = 0; // ���� ������ �����մϴ�.
        Quaternion rot = Quaternion.LookRotation(dir); // �� ������ �ٶ󺸴� ȸ���� ����մϴ�.
        transform.rotation = Quaternion.Lerp(transform.rotation, rot, Time.deltaTime * lookAtSmoothing); // ĳ������ ȸ���� �ε巴�� �����մϴ�.
    }

    private void OnDrawGizmos()
    {
        // �����Ϳ��� �ν� ������ �ð�ȭ�մϴ�.
        Gizmos.DrawWireSphere(transform.position, noticeZone);
    }
}