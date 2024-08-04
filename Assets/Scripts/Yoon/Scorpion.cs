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
        float rotationSpeedMultiplier = 1f; // �̵� �ӵ��� ���� ȸ�� �ӵ� ���� ����
        private int nextIndex = 0;

        Transform player;
        Transform thisTransform;

        public Transform wayPointGroup; // �ν����� â���� �Ҵ��� �� �ֵ��� public���� ����
        private Transform[] points;

        public int scorpionDamage = 5;
        int scorpionHP;
        int nextHP;
        float currentTime = 0;
        private float lerpDuration = 1.5f; // ü���� õõ�� ���� �ð� (1.5��)
        private float delayDuration = 1.5f; // ������
        private int totalDamageTaken = 0; // ���� ������


        PlayerContorler playerScript;
        NavMeshAgent navMeshAgent;

        [SerializeField]
        private Slider _hpBar;

        [SerializeField]
        private Slider _nextHpBar; // nextHP �� �����̴� �߰�


        public int ScorpionHP
        {
            get => scorpionHP;
            private set => scorpionHP = Mathf.Clamp(value, 0, scorpionHP);
        }

        private Camera mainCamera; // ���� ī�޶� ����
        public float hideDistance = 10f; // ü�¹ٸ� ���� �Ÿ�

        private void Start()
        {
            EnterState();
        }

        private void Update()
        {
            UpdateState();
            UpdateHPBarVisibility(); // ü�¹� ���ü� ������Ʈ
        }

        private void Awake()
        {
            scorpionHP = thisHP;
            nextHP = scorpionHP;
            _hpBar.maxValue = scorpionHP;
            _hpBar.value = scorpionHP;
            _nextHpBar.maxValue = scorpionHP; // nextHP �����̴� �ʱ�ȭ
            _nextHpBar.value = scorpionHP;
            mainCamera = Camera.main; // ���� ī�޶� �ʱ�ȭ
        }


        public void GetDamage(int damage)
        {
            if (scorpionHP > 0)
            {
                scorpionHP -= damage;
                _hpBar.value = scorpionHP;
                nextHP = scorpionHP;
                currentTime = 0.0f; // �� �������� ���� ������ currentTime�� �ʱ�ȭ
                totalDamageTaken += damage; // ���� ������ ������Ʈ
                UpdateDamageText(); // ������ �ؽ�Ʈ ������Ʈ
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
                    navMeshAgent.enabled = false; // NavMeshAgent ��Ȱ��ȭ
                    if (points.Length == 0) return;

                    Vector3 direction = points[nextIndex].position - thisTransform.position;
                    Quaternion rot = Quaternion.LookRotation(direction);
                    float adjustedDamping = damping * (moveSpeed * rotationSpeedMultiplier); // �̵� �ӵ��� ���� ȸ�� �ӵ� ����
                    thisTransform.rotation = Quaternion.Slerp(thisTransform.rotation, rot, Time.deltaTime * adjustedDamping);

                    thisTransform.position += thisTransform.forward * moveSpeed * Time.deltaTime;

                    // ���� ��������Ʈ�� �̵��� ���� Ȯ��
                    if (Vector3.Distance(thisTransform.position, points[nextIndex].position) < 0.1f)
                    {
                        nextIndex = (++nextIndex >= points.Length) ? 0 : nextIndex;
                    }
                    break;

                case MoveType.FollowPlayer:
                    navMeshAgent.enabled = true; // NavMeshAgent Ȱ��ȭ
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

                // ù ��° ���(�θ�)�� �ǳʶٰ� �迭�� �ʱ�ȭ
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
                    // moveType�� FollowPlayer���� Way_Point�� ����� ��
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
            // ���� �ʿ�
        }


        // �÷��̾�� �浹 �� ����
        private void OnCollisionStay(Collision collision)
        {
            if (isAttackTrue == false)
            {
                StartCoroutine(AttackDamage(collision));
            }
        }

        

        IEnumerator AttackDamage(Collision collision)
        {
            print("�׽�Ʈ1");
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

        // ���� ����� ��������Ʈ�� ���� ��������Ʈ�� ����
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
