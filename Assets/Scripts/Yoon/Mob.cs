using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;


namespace yoon
{
    public class Mob : Monster
    {
        public enum MoveType
        {
            Way_Point,
            FollowPlayer
        }

        public MoveType moveType = MoveType.Way_Point;
        public float moveSpeed = 1f;

        public TMP_Text hpText;

        public int thisHP;

        public bool isDead;
        public bool isAttackTrue;


        public bool drawGizmos = true;
        public float sightRange;
        public float sightDistance;

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


        #region 이동 함수
        IEnumerator MoveStop()
        {
            float originalSpeed = moveSpeed;
            moveSpeed = 0;
            anim.SetFloat("MoveSpeed", moveSpeed);
            yield return new WaitForSeconds(3.0f);
            moveSpeed = originalSpeed;
        }

        public override void Move()
        {
            if (moveSpeed > 0)
            {
                anim.SetFloat("MoveSpeed", moveSpeed);
            }

            switch (moveType)
            {
                case MoveType.Way_Point:
                    navMeshAgent.enabled = false; // NavMeshAgent 비활성화
                    if (points.Length == 0) return;

                    CheckSight(sightRange, sightDistance);

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
                            StartCoroutine(MoveStop());
                        }
                    }
                    break;

                case MoveType.FollowPlayer:
                    navMeshAgent.enabled = true; // NavMeshAgent 활성화
                    if (player != null)
                    {
                        navMeshAgent.SetDestination(player.position);

                        TaleAttack();
                    }
                    break;
            }

            anim.SetBool("Move", moveSpeed != 0);
        }
        #endregion


        #region Start용 함수
        public override void EnterState()
        {
            isDead = false;
            isAttackTrue = false;
            thisTransform = GetComponent<Transform>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();

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

            if (playerHideTrue)
            {
                if (moveType == MoveType.FollowPlayer)
                {
                    // moveType이 FollowPlayer에서 Way_Point로 변경될 때
                    nextIndex = GetClosestWayPointIndex();
                }
                moveType = MoveType.Way_Point;
            }
            else
            {
                moveType = MoveType.FollowPlayer;
            }

            if (!isDead)
            {
                Move();
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
        }
        #endregion


        #region Exit용 함수
        public override void ExitState()
        {
            // 구현 필요
        }
        #endregion

        // 플레이어와 충돌 시 공격
        private void OnCollisionStay(Collision collision)
        {
            if (isAttackTrue == false && !playerScript.isShieldActive)
            {
                //StartCoroutine(AttackDamage(collision));
            }
        }


        bool attackTrue = false;

        void TaleAttack()
        {
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;

            if (Physics.Raycast(ray, out hitInfo, 20, (1 << 11)) && !attackTrue)
            {
                attackTrue = true;

                if (playerScript != null)
                {
                    PlayerContorler player = hitInfo.transform.GetComponent<PlayerContorler>();
                    player.GetDamage(scorpionDamage);
                }
                else if (playerTestScript != null)
                {
                    TestScripts player = hitInfo.transform.GetComponent<TestScripts>();
                    print(player.name);
                }


                anim.SetTrigger("TaleAttack");
                StartCoroutine(MoveStop());
            }
        }



        private void OnCollisionExit(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                isAttackTrue = false;
                StopAllCoroutines();
            }
        }

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

        void CheckSight(float degree, float maxDistance)
        {
            // 시야 범위 안에 들어온 대상이 있다면 그 대상을 타겟으로 설정하고 싶다.
            // 시야 범위(시야각: 좌우 30도, 전방, 최대 시야 거리: 15미터)
            // 대상 선택을 위한 태그(Player) 설정
            target = null;

            // 1. 월드 안에 배치된 오브젝트 중에 Tag가 "Player"인 오브젝트를 모두 찾는다.
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

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
                    // 4-2. 만일, 사잇각의 값이 30보다 작으면(전방 좌우 30도 이내)...
                    if (cosTheta > 0 && theta < degree)
                    {
                        target = players[i].transform;
                        playerHideTrue = false;
                    }
                }
            }
        }


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

    }
}


