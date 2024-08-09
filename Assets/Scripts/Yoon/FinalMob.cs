using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static yoon.Mob;

public class FinalMob : MonoBehaviour
{

    float distanceToPlayer; // �÷��̾���� �Ÿ�

    public float moveSpeed = 1f;
    float currentSpeed;

    public TMP_Text hpText;

    public int thisHP;

    public bool isDead;
    public bool isAttackTrue; // ���� ���� üũ


    public float attackRange; // ���� ����
    public float checkRange; // �þ� ����

    public bool isFrontTrue = false;
    // �齺�ܿ� �Լ�
    public bool isBackPlayer = false;
    public bool isJumpTrue = false;
    public float curveTime = 5.0f;
    public float interval = 0.1f;
    public float jumpPower = 10f;
    public float mass = 5;
    float maxJumpDistance = 10;

    public Animator anim;


    float damping = 3f;
    float rotationSpeedMultiplier = 1f; // �̵� �ӵ��� ���� ȸ�� �ӵ� ���� ����
    private int nextIndex = 0;

    Transform player;

    public Transform wayPointGroup; // �ν����� â���� �Ҵ��� �� �ֵ��� public���� ����
    private Transform[] points;

    public int scorpionDamage = 5;
    int scorpionHP;
    int nextHP;
    float currentTime = 0;
    private float lerpDuration = 1.5f; // ü���� õõ�� ���� �ð� (1.5��)
    private float delayDuration = 1.5f; // ������
    private int totalDamageTaken = 0; // ���� ������
    int wayPointNum = 0;


    PlayerContorler playerScript;
    TestScripts playerTestScript;
    public bool playerHideTrue;

    NavMeshAgent navMeshAgent;

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP �� �����̴� �߰�

    [SerializeField]
    Transform target;


    public int ScorpionHP
    {
        get => scorpionHP;
        private set => scorpionHP = Mathf.Clamp(value, 0, scorpionHP);
    }

    private Camera mainCamera; // ���� ī�޶� ����
    public float hideDistance = 10f; // ü�¹ٸ� ���� �Ÿ�

    private void Start()
    {
        mainCamera = Camera.main; // ���� ī�޶� �ʱ�ȭ
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;

        if (wayPointGroup != null)
        {
            points = wayPointGroup.GetComponentsInChildren<Transform>();

            // ù ��° ���(�θ�)�� �ǳʶٰ� �迭�� �ʱ�ȭ
            List<Transform> pointList = new List<Transform>(points);
            pointList.RemoveAt(0);
            points = pointList.ToArray();
        }

    }

    private void Update()
    {
        if (!isAttackTrue)
        {
            RangeCheck();
        }

        if (isFrontTrue && !isAttackTrue)
        {
            anim.SetTrigger("JumpAttack");
        }

        else if (isFrontTrue && isBackPlayer && isAttackTrue)
        {
            FrontJump();
        }

        if (moveSpeed != 0)
        {
            anim.SetBool("Move", true);
        }
        else if (moveSpeed == 0)
        {
            anim.SetBool("Move", false);
        }
        UpdateHPBarVisibility();
    }

    void DelayTime()
    {
        print("DelayTime");
        currentTime += Time.deltaTime;
        if (currentTime > 3f)
        {
            currentTime = 0;
            isBackPlayer = false;
            moveSpeed = currentSpeed;
        }
    }

    void FrontJump()
    {
        if (!isJumpTrue) // �̹� ���� ���̸� �߰� ������ ����
        {
            navMeshAgent.enabled = false;
            print("BackJump");

            Vector3 dir = transform.forward + transform.up;
            dir.Normalize();
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.AddForce(dir * jumpPower, ForceMode.Impulse);
            
            StartCoroutine(CheckLanding()); // ���� üũ�� �ڷ�ƾ���� ����
        }
    }

    IEnumerator CheckLanding()
    {
        isJumpTrue = true; // ���� ���·� ����
        yield return new WaitForSeconds(0.1f); // ���� �� 0.1�� ���
        while (isJumpTrue)
        {
            // �Ʒ� �������� Raycast�� ��� �ٴ� �浹 Ȯ��
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            {
                if (hit.distance < 0.1f) // �ٴڿ� ���� ����� ���
                {
                    // �ٴڿ� ����� ��� ���� ó��
                    transform.position = hit.point; // �ٴڿ� ��Ȯ�� ����
                    isBackPlayer = false; // ������ ����
                    isJumpTrue = false; // ���� ���� ����
                    moveSpeed = currentSpeed; // �̵� �ӵ� ����
                    print("���� isAttackTrue " + isAttackTrue);
                    navMeshAgent.enabled = true; // �׺���̼� ��Ȱ��ȭ
                }
            }
            if(Physics.Raycast(transform.position, Vector3.forward, out RaycastHit hitFront, 1f, (1 << 10)))
            {
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
            yield return new WaitForSeconds(0.1f); // ���� ���θ� �ݺ������� Ȯ��
        }
    }


    
    

    void NavMeshPlayer()
    {
        
        print("NavMeshPlayer");
        navMeshAgent.enabled = true;
        moveSpeed = currentSpeed;
        
        if (target != null)
        {
            navMeshAgent.SetDestination(target.position);
        }
    }

    void RangeCheck()
    {
        if (isJumpTrue)
            return;
        print("RangeCheck");
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Ray ���� ��ġ�� ���� transform.position���� y���� 0.5�� ����
        Vector3 rayOrigin = transform.position;
        rayOrigin.y += 0.5f; // y���� 0.5�� ����

        Ray ray = new Ray(rayOrigin, transform.forward);
        Ray backRay = new Ray(rayOrigin, -transform.forward);


        RaycastHit hitInfo;
        

        distanceToPlayer = Vector3.Distance(target.position, transform.position);



        // �þ߹����� �������鼭 ���ݹ������� �ָ� ������...
        if(checkRange >= distanceToPlayer && attackRange < distanceToPlayer && !isBackPlayer)
        {
            NavMeshPlayer();
        }
        else if (checkRange < distanceToPlayer)
        {
            WayPointMove();
        }



        if (Physics.Raycast(ray, out hitInfo, attackRange, (1 << 11)))
        {
            print("frontRay �ȴ�");
            isFrontTrue = true;
        }
        else
        {
            isFrontTrue = false;
        }


        if (Physics.Raycast(backRay, out hitInfo, attackRange, (1 << 11)))
        {
            print("backRay �ȴ�");
            if (!isAttackTrue )
            {
                isBackPlayer = true;
            }
        }
        else
        {
            isBackPlayer = false;
        }
        
    }


    public void Attack()
    {
        print("Attack");
        isAttackTrue = true;
    }

    public void AttackAfter()
    {
        print("AttackAfter");
        isAttackTrue = false;
    }



    #region ��������Ʈ �Լ�
    void WayPointMove()
    {
        print("WayPointMove");
        moveSpeed = currentSpeed;
        
        navMeshAgent.enabled = false; // NavMeshAgent ��Ȱ��ȭ
        if (points.Length == 0) return;

        Vector3 direction = points[nextIndex].position - transform.position;
        Quaternion rot = Quaternion.LookRotation(direction);
        float adjustedDamping = damping * (moveSpeed * rotationSpeedMultiplier); // �̵� �ӵ��� ���� ȸ�� �ӵ� ����
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * adjustedDamping);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // ���� ��������Ʈ�� �̵��� ���� Ȯ��
        if (Vector3.Distance(transform.position, points[nextIndex].position) < 0.1f)
        {
            nextIndex = (++nextIndex >= points.Length) ? 0 : nextIndex;
            wayPointNum++;
            if (wayPointNum % 3 == 0)
            {
                wayPointNum = 0;
            }
        }
        if (!playerHideTrue)
        {

        }
    }
    #endregion

    // ���� ����� ��������Ʈ�� ���� ��������Ʈ�� ����
    private int GetClosestWayPointIndex()
    {
        int closestIndex = 0;
        float closestDistance = float.MaxValue;

        for (int i = 0; i < points.Length; i++)
        {
            float distance = Vector3.Distance(transform.position, points[i].position);
            if (distance < closestDistance)
            {
                closestDistance = distance;
                closestIndex = i;
            }
        }

        return closestIndex;
    }

    #region HPBar �Լ�
    private void LateUpdate()
    {
        if (_hpBar != null)
        {
            _hpBar.transform.LookAt(_hpBar.transform.position + mainCamera.transform.rotation * Vector3.forward,
                mainCamera.transform.rotation * Vector3.up);
        }
    }

    private void UpdateHPBarVisibility()
    {
        if (Vector3.Distance(target.position, transform.position) > hideDistance)
        {
            _hpBar.gameObject.SetActive(false);
        }
        else
        {
            _hpBar.gameObject.SetActive(true);
        }
    }
    #endregion


    private void OnDrawGizmos()
    {
        // target�� null�� �ƴ� ��쿡�� �׸����� �մϴ�.
        if (target != null)
        {
            // Ray ���� ��ġ�� ���� transform.position���� y���� 0.5�� ����
            Vector3 rayOrigin = transform.position;
            rayOrigin.y += 0.5f; // y���� 0.5�� ����

            // Ray ����
            Ray ray = new Ray(rayOrigin, transform.forward);

            Ray backRay = new Ray(rayOrigin, -transform.forward);

            // Ray�� � ������ �׷����� �����մϴ�.
            Gizmos.color = Color.red;

            // Ray�� �׸��ϴ�.
            Gizmos.DrawRay(ray.origin, ray.direction * attackRange);

            Gizmos.DrawRay(backRay.origin, backRay.direction * attackRange);

            // Ray�� ���� �������� ��ü�� �׷��ݴϴ�.
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, attackRange, (1 << 11)))
            {
                Gizmos.color = Color.green; // ���� ��� �ʷϻ����� ǥ��
                Gizmos.DrawSphere(hitInfo.point, 0.2f); // ���� ������ ���� ��ü�� �׸�
            }
        }
    }

}
