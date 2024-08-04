using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using yoon;

namespace yoon
{
    public class Scorpion : Monster
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
        bool isAttackTrue;

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


        PlayerContorler playerScript;
        NavMeshAgent navMeshAgent;

        [SerializeField]
        private Slider _hpBar;

        [SerializeField]
        private Slider _nextHpBar; // nextHP 용 슬라이더 추가


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

        public override void Move()
        {
            switch (moveType)
            {
                case MoveType.Way_Point:
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
                    }
                    break;

                case MoveType.FollowPlayer:
                    navMeshAgent.enabled = true; // NavMeshAgent 활성화
                    if (player != null)
                    {
                        navMeshAgent.SetDestination(player.position);
                    }
                    break;
            }
        }

        public override void EnterState()
        {
            isDead = false;
            isAttackTrue = false;
            thisTransform = GetComponent<Transform>();
            navMeshAgent = GetComponent<NavMeshAgent>();
            anim = GetComponent<Animator>();

            if (wayPointGroup != null)
            {
                points = wayPointGroup.GetComponentsInChildren<Transform>();

                // 첫 번째 요소(부모)를 건너뛰고 배열을 초기화
                List<Transform> pointList = new List<Transform>(points);
                pointList.RemoveAt(0);
                points = pointList.ToArray();
            }

            player = GameObject.FindWithTag("Player").transform;
            playerScript = player.GetComponent<PlayerContorler>();
        }

        public override void UpdateState()
        {
            if (scorpionHP <= 0)
            {
                Die();
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


            if (playerScript.hiding)
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
        }

        public override void ExitState()
        {
            // 구현 필요
        }


        // 플레이어와 충돌 시 공격
        private void OnCollisionStay(Collision collision)
        {
            if (isAttackTrue == false)
            {
                StartCoroutine(AttackDamage(collision));
            }
        }

        

        IEnumerator AttackDamage(Collision collision)
        {
            print("테스트1");
            isAttackTrue = true;
            if (collision.gameObject.CompareTag("Player"))
            {
                PlayerContorler player = collision.transform.GetComponent<PlayerContorler>();
                player.GetDamage(scorpionDamage);
            }
            yield return new WaitForSeconds(2.0f);
            isAttackTrue = false;
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
    }
}
