using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.Assertions.Must;
using UnityEngine.UI;
using static yoon.Mob;

public class FinalMob : MonoBehaviour
{

    public float distanceToPlayer; // 플레이어와의 거리

    public float moveSpeed = 1f;
    float currentSpeed;

    public TMP_Text hpText;

    //public int thisHP;

    public bool isDead = false;
    public bool isAttackTrue; // 공격 여부 체크
    public bool isDelayTrue = false;

    public bool monsterHit = false;

    public int thisHP;

    public float attackRange; // 공격 범위
    public float checkRange; // 시야 범위

    public bool isFrontTrue = false;
    // 백스텝용 함수
    public bool isBackPlayer = false;
    public bool isJumpTrue = false;
    public float curveTime = 5.0f;
    public float interval = 0.1f;
    public float jumpPower = 10f;
    public float mass = 5;
    float maxJumpDistance = 10;

    public Animator anim;


    float damping = 3f;
    float rotationSpeedMultiplier = 1f; // 이동 속도에 따른 회전 속도 증가 비율
    private int nextIndex = 0;

    Transform player;

    public Transform wayPointGroup; // 인스펙터 창에서 할당할 수 있도록 public으로 설정
    private Transform[] points;

    public int scorpionDamage = 5;
    public int scorpionHP;
    int nextHP;
    float currentTime = 0;
    private float lerpDuration = 1.5f; // 체력이 천천히 빠질 시간 (1.5초)
    private float delayDuration = 1.5f; // 딜레이
    private int totalDamageTaken = 0; // 누적 데미지
    int wayPointNum = 0;


    PlayerContorler playerScript;
    TestScripts playerTestScript;
    public bool playerHideTrue;

    NavMeshAgent navMeshAgent;

    [SerializeField]
    private Slider _hpBar;

    [SerializeField]
    private Slider _nextHpBar; // nextHP 용 슬라이더 추가

    [SerializeField]
    Transform target;



    private Camera mainCamera; // 메인 카메라 참조
    public float hideDistance = 10f; // 체력바를 숨길 거리

    private void Start()
    {
        playerHideTrue = true;
        playerScript = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContorler>();
        mainCamera = Camera.main; // 메인 카메라 초기화
        navMeshAgent = GetComponent<NavMeshAgent>();
        anim = GetComponent<Animator>();
        currentSpeed = moveSpeed;

        if (wayPointGroup != null)
        {
            points = wayPointGroup.GetComponentsInChildren<Transform>();

            // 첫 번째 요소(부모)를 건너뛰고 배열을 초기화
            List<Transform> pointList = new List<Transform>(points);
            pointList.RemoveAt(0);
            points = pointList.ToArray();
        }

    }

    private void Awake()
    {
        scorpionHP = thisHP;
        nextHP = scorpionHP;
        _hpBar.maxValue = scorpionHP;
        _hpBar.value = scorpionHP;
        _nextHpBar.maxValue = scorpionHP; // nextHP 슬라이더 초기화
        _nextHpBar.value = scorpionHP;
    }

    private void Update()
    {
        if (!monsterHit && !isDead)
        {
            if (!isAttackTrue)
            {
                RangeCheck();
            }

            if (playerHideTrue)
            {
                WayPointMove();
            }
            else if (!playerHideTrue)
            {
                NavMeshPlayer();
            }

            if (isJumpTrue && !isAttackTrue && isDead != true)
            {
                print("이게 문제다");
                anim.SetTrigger("JumpAttack");
                AttackDelay();

                navMeshAgent.isStopped = true;
            }

            else if (isJumpTrue && isAttackTrue)
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

            if (Physics.Raycast(transform.position, transform.forward, out RaycastHit hitFront, 6f, (1 << 10)))
            {
                // Y축 회전을 유지하면서 X축 회전을 0도로 설정
                transform.rotation = Quaternion.Euler(0, transform.eulerAngles.y, transform.eulerAngles.z);
            }
        }
        else
        {

        }

        if (_nextHpBar.value != nextHP)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= delayDuration)
            {
                float t = (currentTime - delayDuration) / lerpDuration;
                _nextHpBar.value = Mathf.Lerp(_nextHpBar.value, nextHP, t);

                if (Mathf.Abs(_nextHpBar.value - nextHP) < 0.01f)
                {
                    _nextHpBar.value = nextHP;
                    currentTime = 0.0f;
                }
            }
        }

        UpdateHPBarVisibility();
    }

    void Die()
    {
        anim.SetTrigger("Death");
        navMeshAgent.isStopped = true;
        moveSpeed = 0;
        StartCoroutine(DestroyMonster());
    }

    IEnumerator DestroyMonster()
    {
        yield return new WaitForSeconds(3f);
        Destroy(gameObject);
    }


    void FrontJump()
    {
        if (!isJumpTrue)
        {
            navMeshAgent.isStopped = true;
            //print("BackJump");

            Vector3 dir = transform.forward + transform.up;
            dir.Normalize();
            Rigidbody rb = GetComponent<Rigidbody>();

            rb.AddForce(dir * jumpPower, ForceMode.Impulse);

            StartCoroutine(CheckLanding()); // 착지 체크를 코루틴으로 실행
        }
    }

    IEnumerator CheckLanding()
    {
        isJumpTrue = true; // 점프 상태로 설정
        yield return new WaitForSeconds(0.1f); // 점프 후 0.1초 대기
        while (isJumpTrue)
        {
            // 아래 방향으로 Raycast를 쏘아 바닥 충돌 확인
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            {
                if (hit.distance < 0.1f) // 바닥에 거의 닿았을 경우
                {
                    // 바닥에 닿았을 경우 착지 처리
                    transform.position = hit.point; // 바닥에 정확히 착지
                    isBackPlayer = false; // 백점프 종료
                    isJumpTrue = false; // 점프 상태 해제
                    moveSpeed = currentSpeed; // 이동 속도 복원
                    //print("안쪽 isAttackTrue " + isAttackTrue);
                    navMeshAgent.isStopped = false; // 네비게이션 재활성화
                }
            }
            yield return new WaitForSeconds(0.1f); // 착지 여부를 반복적으로 확인
        }
    }

    public void GetDamage(int damage)
    {
        if (scorpionHP > 0 && !isDead)
        {
            scorpionHP -= damage;
            _hpBar.value = scorpionHP;
            nextHP = scorpionHP;
            currentTime = 0.0f; // 새 데미지를 받을 때마다 currentTime을 초기화
            totalDamageTaken += damage; // 누적 데미지 업데이트
            UpdateDamageText(); // 데미지 텍스트 업데이트

            if (scorpionHP <= 0)
            {
                isDead = true;
                Die();
            }
            else
            {
                anim.SetTrigger("Hit");
                AttackDelay();
            }
        }
    }




    private void UpdateDamageText()
    {
        hpText.text = totalDamageTaken.ToString();
    }


    void NavMeshPlayer()
    {
        
        //print("NavMeshPlayer");
        navMeshAgent.isStopped = false;
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
        //print("RangeCheck");
        target = GameObject.FindGameObjectWithTag("Player").transform;

        // Ray 시작 위치를 현재 transform.position에서 y값을 0.5로 변경
        Vector3 rayOrigin = transform.position;
        rayOrigin.y += 0.5f; // y값을 0.5로 설정

        Ray ray = new Ray(rayOrigin, transform.forward);
        Ray backRay = new Ray(rayOrigin, -transform.forward);


        distanceToPlayer = Vector3.Distance(target.position, transform.position);



        // 플레이어를 발견했지만 공격 범위에 있지 않은 경우
        if (checkRange >= distanceToPlayer && attackRange < distanceToPlayer && !isBackPlayer)
        {
            playerHideTrue = false;
            isJumpTrue = false;
            transform.LookAt(target.transform.position);
            navMeshAgent.isStopped = false;
        }
        // 공격 범위에 들어온 경우
        else if (attackRange >= distanceToPlayer && !isBackPlayer)
        {
            isJumpTrue = true;
            // 추가 동작이 필요한 경우 여기에 작성
        }
        // 플레이어가 발견 범위를 넘어선 경우
        else if (checkRange < distanceToPlayer)
        {
            playerHideTrue = true;
            isJumpTrue = false;
        }
        
    }


    public void Attack()
    {
        if ((!isDead))
        {
            print("Attack");
            isAttackTrue = true;
        }
    }

    public void AttackAfter()
    {
        if ((!isDead))
        {
            print("AttackAfter");
            isAttackTrue = false;
            isJumpTrue = true;
        }
    }

    void AttackDelay()
    {
        currentTime += Time.deltaTime;
        moveSpeed = 0;
        transform.LookAt(playerScript.transform.position);
        if (currentTime > 4f)
        {
            currentTime = 0;
            anim.SetTrigger("delayFinish");
            navMeshAgent.isStopped = false;
            isDelayTrue = false;
            moveSpeed = currentSpeed;
        }
    }

    void HitAfter()
    {
        monsterHit = false;
        if (isAttackTrue == true)
        {
            isAttackTrue = false;
        }
        isDelayTrue = false;
        NavMeshPlayer();
    }

    private void OnTriggerEnter(Collider other)
    {
        
        if (other.gameObject.CompareTag("Melee") && playerScript.hit && !isDead)
        {
            Sward sward = other.GetComponent<Sward>();
            print("hiyMonster");
            playerScript.hit = false;
            monsterHit = true;
            GetDamage(sward.attackPower);
            return;
        }

        if (other.gameObject.CompareTag("Player"))
        {
            //PlayerContorler player = other.GetComponent<PlayerContorler>();
            print("플레이어 피격");

            //player.GetDamage(scorpionDamage);
            //isAttackTrue = false;
        }
    }

    private void OnCollisionStay(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isAttackTrue && !isDead)
        {
            PlayerContorler player = collision.gameObject.GetComponent<PlayerContorler>();
            print("플레이어 피격");

            playerScript.playerHit = true;
            player.GetDamage(scorpionDamage);
            isAttackTrue = false;
        }
    }



    #region 웨이포인트 함수
    void WayPointMove()
    {
        moveSpeed = currentSpeed;
        
        navMeshAgent.isStopped = true; // NavMeshAgent 비활성화
        if (points.Length == 0) return;

        Vector3 direction = points[nextIndex].position - transform.position;
        Quaternion rot = Quaternion.LookRotation(direction);
        float adjustedDamping = damping * (moveSpeed * rotationSpeedMultiplier); // 이동 속도에 따른 회전 속도 조정
        transform.rotation = Quaternion.Slerp(transform.rotation, rot, Time.deltaTime * adjustedDamping);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;

        // 다음 웨이포인트로 이동할 조건 확인
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

    // 가장 가까운 웨이포인트를 다음 웨이포인트로 설정
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

    #region HPBar 함수
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
        // target이 null이 아닌 경우에만 그리도록 합니다.
        if (target != null)
        {
            // Ray 시작 위치를 현재 transform.position에서 y값을 0.5로 변경
            Vector3 rayOrigin = transform.position;
            rayOrigin.y += 0.5f; // y값을 0.5로 설정

            // Ray 생성
            Ray ray = new Ray(rayOrigin, transform.forward);

            Ray backRay = new Ray(rayOrigin, -transform.forward);

            // Ray가 어떤 색으로 그려질지 설정합니다.
            Gizmos.color = Color.red;

            // Ray를 그립니다.
            Gizmos.DrawRay(ray.origin, ray.direction * attackRange);

            Gizmos.DrawRay(backRay.origin, backRay.direction * attackRange);

            // Ray가 맞춘 지점까지 구체를 그려줍니다.
            RaycastHit hitInfo;
            if (Physics.Raycast(ray, out hitInfo, attackRange, (1 << 11)))
            {
                Gizmos.color = Color.green; // 맞춘 경우 초록색으로 표시
                Gizmos.DrawSphere(hitInfo.point, 0.2f); // 맞춘 지점에 작은 구체를 그림
            }
        }
    }

}
