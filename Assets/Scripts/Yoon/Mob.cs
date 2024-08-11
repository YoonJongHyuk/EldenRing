using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace yoon
{
    public class Mob : Monster
    {
        public enum MobType
        {
            Way_Point,
            FollowPlayer,
            JumpAttack,
            Death,
            ReturnHome,
            AttackDelay,
            Idle,
            BackJump
        }

        public MobType mobType = MobType.Way_Point;
        public float moveSpeed = 1f;
        float currentSpeed;

        public TMP_Text hpText;

        public int thisHP;

        public bool isDead;
        public bool isAttackTrue;
        private bool jumpAttackPerformed = false;

        public bool drawGizmos = true;
        public float sightRange;
        public float sightDistance;
        public float attackRange;


        // 백스텝용 함수
        public float curveTime = 5.0f;
        public float interval = 0.1f;
        public float jumpPower = 10f;
        public float mass = 5;
        public bool isJumpTrue = false;
        float maxJumpDistance = 10;

        public Animator anim;


        float damping = 3f;
        float rotationSpeedMultiplier = 1f; // 이동 속도에 따른 회전 속도 증가 비율
        private int nextIndex = 0;

        Transform player;
        Transform thisTransform;

        public Transform wayPointGroup; // 인스펙터 창에서 할당할 수 있도록 public으로 설정
        private Transform[] points;

        public int scorpionDamage = 5;
        int scorpionHP;
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


        public int ScorpionHP
        {
            get => scorpionHP;
            private set => scorpionHP = Mathf.Clamp(value, 0, scorpionHP);
        }

        private Camera mainCamera; // 메인 카메라 참조
        public float hideDistance = 10f; // 체력바를 숨길 거리

        private void Start()
        {
            EnterState();
        }

        private void Update()
        {
            UpdateState();
            UpdateHPBarVisibility(); // 체력바 가시성 업데이트
        }

        private void Awake()
        {
            scorpionHP = thisHP;
            nextHP = scorpionHP;
            _hpBar.maxValue = scorpionHP;
            _hpBar.value = scorpionHP;
            _nextHpBar.maxValue = scorpionHP; // nextHP 슬라이더 초기화
            _nextHpBar.value = scorpionHP;
            mainCamera = Camera.main; // 메인 카메라 초기화
        }


        public void GetDamage(int damage)
        {
            if (scorpionHP > 0)
            {
                scorpionHP -= damage;
                _hpBar.value = scorpionHP;
                nextHP = scorpionHP;
                currentTime = 0.0f; // 새 데미지를 받을 때마다 currentTime을 초기화
                totalDamageTaken += damage; // 누적 데미지 업데이트
                UpdateDamageText(); // 데미지 텍스트 업데이트
            }
        }

        private void UpdateDamageText()
        {
            hpText.text = totalDamageTaken.ToString();
        }


        public override void Die()
        {
            if (!isDead)
            {
                isDead = true;
                Destroy(gameObject);
            }
        }



        #region Start용 함수
        public override void EnterState()
        {
            isDead = false;
            isAttackTrue = false;
            thisTransform = GetComponent<Transform>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();
            currentSpeed = moveSpeed;
            bool hiding = false;

            if (playerScript != null)
            {
                hiding = playerScript.hiding;
            }
            else if (playerTestScript != null)
            {
                hiding = playerTestScript.hiding;
            }

            playerHideTrue = hiding;

            playerHideTrue = true;

            sightRange = Mathf.Clamp(sightRange, 0, 90.0f);

            if (wayPointGroup != null)
            {
                points = wayPointGroup.GetComponentsInChildren<Transform>();

                // 첫 번째 요소(부모)를 건너뛰고 배열을 초기화
                List<Transform> pointList = new List<Transform>(points);
                pointList.RemoveAt(0);
                points = pointList.ToArray();
            }

            GameObject playerObject = GameObject.FindWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerScript = player.GetComponent<PlayerContorler>();
                playerTestScript = player.GetComponent<TestScripts>();
            }
            else
            {
                Debug.LogError("Player object not found!");
            }
        }
        #endregion


        #region Update용 함수
        public override void UpdateState()
        {
            if (scorpionHP <= 0)
            {
                Die();
                return; // Die() 이후에는 다른 작업을 하지 않도록 리턴
            }

            if (navMeshAgent.enabled)
            {
                moveSpeed = currentSpeed;
            }
            else
            {
                moveSpeed = 0;
            }

            if (!isDead)
            {
                Move();
            }

            CurrentState();

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
        }
        #endregion


        #region Exit용 함수
        public override void ExitState()
        {
            // 구현 필요
        }
        #endregion

        




        IEnumerator HandlePostAttack(MobType nextState)
        {
            moveSpeed = 0;
            anim.SetFloat("MoveSpeed", moveSpeed);
            anim.SetBool("Move", false);
            yield return new WaitForSeconds(0.01f);
            float animTime = anim.GetCurrentAnimatorStateInfo(0).length * 3;
            yield return new WaitForSeconds(animTime); // 애니메이션 길이만큼 대기
            isAttackTrue = false; // 공격 완료 상태로 설정
            mobType = nextState; // 다음 상태로 전환
        }

        void JumpAttack()
        {
            if (!isAttackTrue) // 점프 어택을 한 번만 수행
            {
                isAttackTrue = true; // 공격 중 상태로 설정
                anim.SetTrigger("JumpAttack");
                StartCoroutine(HandlePostAttack(MobType.AttackDelay)); // 공격 후 딜레이 상태로 전환
                return;
            }

            // JumpAttack 후에는 거리에 따라 행동을 결정
            float dist = Vector3.Distance(transform.position, target.position);
            HandleDistanceBasedBehavior(dist); // 거리 기반 행동 처리
        }


        void BackJump()
        {
            if (isJumpTrue)
            {
                // JumpAttack 후에는 거리에 따라 행동을 결정
                float dist = Vector3.Distance(transform.position, target.position);
                HandleDistanceBasedBehavior(dist); // 거리 기반 행동 처리
            }

            Vector3 startPos = transform.position;
            Vector3 dir = transform.forward * -1 + transform.up;
            dir.Normalize();

            Vector3 gravity = Physics.gravity;

            // 위치 계산
            Vector3 result = startPos + dir * jumpPower * currentTime + 0.5f * gravity * currentTime * currentTime;
            transform.position = result;

            // 아래 방향으로 Raycast를 쏘아 바닥 충돌 확인
            if (Physics.Raycast(transform.position, Vector3.down, out RaycastHit hit, 1f))
            {
                // 바닥에 닿았을 경우 착지 처리
                transform.position = hit.point; // 바닥에 정확히 착지
                currentTime = 0; // currentTime 초기화
                isJumpTrue = true;
            }

            // currentTime을 interval만큼 증가
            currentTime += interval;
        }

        void HandleDistanceBasedBehavior(float dist)
        {
            if (dist <= 2f)
            {
                mobType = MobType.BackJump; // 거리 2 이하일 때 백점프
            }
            else if (dist > 2f && dist <= 5f)
            {
                mobType = MobType.JumpAttack; // 거리 2 이상 5 이하일 때 점프 공격
            }
            else if (dist > 5f && dist <= 10f)
            {
                RotateTowardsPlayer(); // 거리 5 초과 10 이하일 때 플레이어를 바라보도록 회전
                mobType = MobType.FollowPlayer;
            }
            else if (dist > 10f)
            {
                mobType = MobType.Way_Point; // 거리 10 이상일 때 웨이포인트로 전환
            }
        }

        void RotateTowardsPlayer()
        {
            // 플레이어를 바라보도록 회전
            Vector3 directionToPlayer = player.position - transform.position;
            directionToPlayer.y = 0; // y축 회전을 위해 y값을 0으로 설정

            Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);
            transform.rotation = Quaternion.Slerp(transform.rotation, rotationToPlayer, Time.deltaTime * rotationSpeedMultiplier);
        }

        void FollowPlayer()
        {
            navMeshAgent.enabled = true;
            moveSpeed = currentSpeed;
            anim.SetFloat("MoveSpeed", moveSpeed);
            anim.SetBool("Move", true);

            if (player != null)
            {
                navMeshAgent.SetDestination(player.position);
                float distanceToPlayer = Vector3.Distance(player.position, thisTransform.position);

                if (distanceToPlayer <= attackRange) // 공격 가능한 범위 내에 있을 때
                {
                    mobType = MobType.AttackDelay; // 전투 상태로 전환
                    navMeshAgent.enabled = false;
                }
                else
                {
                    HandleDistanceBasedBehavior(distanceToPlayer); // 거리 기반 행동 처리
                }
            }

            if (playerHideTrue)
            {
                nextIndex = GetClosestWayPointIndex();
                mobType = MobType.Way_Point;
            }
        }


        void AttackDelay()
        {
            navMeshAgent.enabled = false;

            // 타겟이 설정되지 않았다면 종료
            if (target == null)
            {
                return;
            }

            float dist = Vector3.Distance(transform.position, target.position);

            // 시야 범위와 거리 조건을 검사
            CheckSight(sightRange, sightDistance);

            // 거리 기반 행동 처리
            HandleDistanceBasedBehavior(dist);

            // 추가 로직이 필요한 경우 여기에 작성할 수 있음
        }


        #region 쓸모없는 함수 모음(삭제 보류중인 코드 모음)
        //void FollowStart()
        //{
        //    // 다시 추격 상태로 전환한다.
        //    mobType = MobType.FollowPlayer;
        //    print("My State: AttackDelay -> FollowPlayer");
        //    currentTime = 0;
        //    moveSpeed = currentSpeed;
        //    anim.SetFloat("MoveSpeed", moveSpeed);
        //    anim.SetBool("Move", true);
        //    jumpAttackPerformed = false; // 추격 상태로 돌아갈 때 초기화
        //    return;
        //}
        #endregion

        void CurrentState()
        {
            switch (mobType)
            {
                case MobType.Way_Point:
                    WayPointMove();
                    break;

                case MobType.FollowPlayer:
                    FollowPlayer();
                    break;

                case MobType.AttackDelay:
                    AttackDelay();
                    break;

                case MobType.JumpAttack:
                    JumpAttack();
                    break;
                case MobType.Idle:
                    Idle();
                    break;
                case MobType.BackJump:
                    BackJump();
                    break;
            }
        }

        void Idle()
        {
            navMeshAgent.enabled = false;
            moveSpeed = 0;
            anim.SetFloat("MoveSpeed", moveSpeed);
            anim.SetBool("Move", false);
        }


        #region 웨이포인트 이동 함수
        void WayPointMove()
        {
            CheckSight(sightRange, sightDistance);
            moveSpeed = currentSpeed;
            anim.SetFloat("MoveSpeed", moveSpeed);
            anim.SetBool("Move", true);
            navMeshAgent.enabled = false; // NavMeshAgent 비활성화
            if (points.Length == 0) return;

            Vector3 direction = points[nextIndex].position - thisTransform.position;
            Quaternion rot = Quaternion.LookRotation(direction);
            float adjustedDamping = damping * (moveSpeed * rotationSpeedMultiplier); // 이동 속도에 따른 회전 속도 조정
            thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, rot, Time.deltaTime * adjustedDamping);

            thisTransform.position += thisTransform.forward * moveSpeed * Time.deltaTime;

            // 다음 웨이포인트로 이동할 조건 확인
            if (Vector3.Distance(thisTransform.position, points[nextIndex].position) < 0.1f)
            {
                nextIndex = (++nextIndex >= points.Length) ? 0 : nextIndex;
                wayPointNum++;
                if (wayPointNum % 3 == 0)
                {
                    wayPointNum = 0;
                    //StartCoroutine(MoveStop());
                }
            }
            if(!playerHideTrue)
            {
                mobType = MobType.FollowPlayer;
            }
        }
        #endregion

        #region 플레이어 접근 함수
        //void FollowPlayer()
        //{
        //    navMeshAgent.enabled = true;
        //    moveSpeed = currentSpeed;
        //    anim.SetFloat("MoveSpeed", moveSpeed);
        //    print("moveSpeed" + moveSpeed);
        //    print("currentSpeed" + currentSpeed);
        //    anim.SetBool("Move", true);
        //    if (player != null)
        //    {
        //        navMeshAgent.SetDestination(player.position);
        //        float distanceToPlayer = Vector3.Distance(player.position, thisTransform.position);
        //        if (distanceToPlayer <= attackRange) // Define attackRange
        //        {
        //            mobType = MobType.AttackDelay; // 전투 상태로 전환
        //            navMeshAgent.enabled = false;
        //        }
        //    }
        //    if (playerHideTrue)
        //    {
        //        nextIndex = GetClosestWayPointIndex();
        //        mobType = MobType.Way_Point;
        //    }
        //}
        #endregion






        // 가장 가까운 웨이포인트를 다음 웨이포인트로 설정
        private int GetClosestWayPointIndex()
        {
            int closestIndex = 0;
            float closestDistance = float.MaxValue;

            for (int i = 0; i < points.Length; i++)
            {
                float distance = Vector3.Distance(thisTransform.position, points[i].position);
                if (distance < closestDistance)
                {
                    closestDistance = distance;
                    closestIndex = i;
                }
            }

            return closestIndex;
        }



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
            if (Vector3.Distance(player.position, thisTransform.position) > hideDistance)
            {
                _hpBar.gameObject.SetActive(false);
            }
            else
            {
                _hpBar.gameObject.SetActive(true);
            }
        }


        #region 몬스터 시야각 함수
        void CheckSight(float degree, float maxDistance)
        {
            // 시야 범위 안에 들어온 대상이 있다면 그 대상을 타겟으로 설정하고 싶다.
            // 시야 범위(시야각: 좌우 30도, 전방, 최대 시야 거리: 15미터)
            // 대상 선택을 위한 태그(Player) 설정
            target = null;

            // 1. 월드 안에 배치된 오브젝트 중에 Tag가 "Player"인 오브젝트를 모두 찾는다.
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");
            //Transform targetPlayer;

            // 2. 찾은 오브젝트들 중에서 거리가 maxDistance 이내인 오브젝트만 찾는다.
            for (int i = 0; i < players.Length; i++)
            {
                float distance = Vector3.Distance(players[i].transform.position, transform.position);

                if (distance <= maxDistance)
                {
                    // 3. 찾은 오브젝트를 바라보는 벡터와 나의 전방 벡터를 내적한다.
                    Vector3 lookVector = players[i].transform.position - transform.position;
                    lookVector.Normalize();

                    float cosTheta = Vector3.Dot(transform.forward, lookVector);
                    float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

                    // 4-1. 만일, 내적의 결과 값이 0보다 크면(나보다 앞쪽에 있다)...
                    // 4-2. 만일, 사잇각의 값이 degree보다 작으면(전방 좌우 degree도 이내)...
                    if (cosTheta > 0 && theta < degree)
                    {
                        target = players[i].transform;
                        playerHideTrue = false;
                        if (distance > 6)
                        {
                            mobType = MobType.AttackDelay;
                        }
                    }
                    // 5. 이미 공격한 상태라면 플레이어가 뒤에 있을 경우 뒤를 돌아보는 기능 추가
                    else if (cosTheta < 0 && theta > degree)
                    {
                        // 플레이어를 바라보도록 회전
                        Vector3 directionToPlayer = players[i].transform.position - transform.position;
                        directionToPlayer.y = 0; // y축 회전을 위해 y값을 0으로 설정

                        Quaternion rotationToPlayer = Quaternion.LookRotation(directionToPlayer);
                        transform.rotation = Quaternion.Euler(0, rotationToPlayer.eulerAngles.y, 0);
                    }
                }
            }

            // target이 null인 경우 mobType을 Way_Point로 설정
            if (target == null)
            {
                mobType = MobType.Way_Point;
            }
        }

        #endregion


        #region  시야각 그리기 함수
        // 원 그리기
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
            {
                return;
            }

            Gizmos.color = new Color32(154, 14, 235, 255);

            #region 원 그리기
            //List<Vector3> points = new List<Vector3>();
            //for (int i = 0; i < 360; i += 5)
            //{
            //    Vector3 point = new Vector3(Mathf.Cos(i * Mathf.Deg2Rad), 0, Mathf.Sin(i * Mathf.Deg2Rad)) * 5;
            //    points.Add(transform.position + point);
            //}

            //for (int i = 0; i < points.Count - 1; i++)
            //{
            //    Gizmos.DrawLine(points[i], points[i + 1]);
            //}
            #endregion

            // 시야각 그리기
            float rightDegree = 90 - sightRange;
            float leftDegree = 90 + sightRange;

            Vector3 rightPos = (transform.right * Mathf.Cos(rightDegree * Mathf.Deg2Rad) +
                               transform.forward * Mathf.Sin(rightDegree * Mathf.Deg2Rad)) * sightDistance
                               + transform.position;

            Vector3 leftPos = (transform.right * Mathf.Cos(leftDegree * Mathf.Deg2Rad) +
                               transform.forward * Mathf.Sin(leftDegree * Mathf.Deg2Rad)) * sightDistance
                              + transform.position;

            Gizmos.DrawLine(transform.position, rightPos);
            Gizmos.DrawLine(transform.position, leftPos);

        }
        #endregion
    }
}


