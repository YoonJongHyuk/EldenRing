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
            TaleAttack,
            LongAttack,
            JumpAttack,
            Death,
            ReturnHome,
            AttackDelay
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


        #endregion


        #region Start�� �Լ�
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

                // ù ��° ���(�θ�)�� �ǳʶٰ� �迭�� �ʱ�ȭ
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


        #region Update�� �Լ�
        public override void UpdateState()
        {
            if (scorpionHP <= 0)
            {
                Die();
                return; // Die() ���Ŀ��� �ٸ� �۾��� ���� �ʵ��� ����
            }

            if (navMeshAgent.enabled)
            {
                moveSpeed = currentSpeed;
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


        #region Exit�� �Լ�
        public override void ExitState()
        {
            // ���� �ʿ�
        }
        #endregion

        // �÷��̾�� �浹 �� ����
        private void OnCollisionStay(Collision collision)
        {
            if (collision.gameObject.CompareTag("Player"))
            {
                StartCoroutine(HandlePostAttack(MobType.AttackDelay)); // ���� �� ������ ���·� ��ȯ
                anim.SetTrigger("TaleAttack");
            }
        }



        void TaleAttack()
        {

            StartCoroutine(HandlePostAttack(MobType.AttackDelay)); // ���� �� ������ ���·� ��ȯ
            anim.SetTrigger("TaleAttack");
        }

        IEnumerator HandlePostAttack(MobType nextState)
        {
            isAttackTrue = true; // ���� �� ���·� ����
            currentSpeed = moveSpeed;
            moveSpeed = 0;
            anim.SetFloat("MoveSpeed", moveSpeed);
            anim.SetBool("Move", false);
            yield return new WaitForSeconds(0.01f);
            float animTime = anim.GetCurrentAnimatorStateInfo(0).length * 3;
            yield return new WaitForSeconds(animTime); // �ִϸ��̼� ���̸�ŭ ���
            isAttackTrue = false; // ���� �Ϸ� ���·� ����
            mobType = nextState; // ���� ���·� ��ȯ
        }

        void LongAttack()
        {
            StartCoroutine(HandlePostAttack(MobType.AttackDelay)); // ���� �� ������ ���·� ��ȯ
            anim.SetTrigger("LongAttack");
            anim.SetBool("Move", false);
        }

        void JumpAttack()
        {
            StartCoroutine(HandlePostAttack(MobType.AttackDelay)); // ���� �� ������ ���·� ��ȯ
            anim.SetTrigger("JumpAttack");
        }

        void AttackDelay()
        {
            navMeshAgent.enabled = false;

            

            // ����, Ÿ�ٰ� �Ÿ��� ���� ������ ������ ����ٸ�...
            float dist = Vector3.Distance(transform.position, target.position);

            if (dist > 7)
            {
                // �ٽ� �߰� ���·� ��ȯ�Ѵ�.
                mobType = MobType.FollowPlayer;
                print("My State: AttackDelay -> FollowPlayer");
                currentTime = 0;
                moveSpeed = currentSpeed;
                anim.SetFloat("MoveSpeed", moveSpeed);
                anim.SetBool("Move", true);
                jumpAttackPerformed = false; // �߰� ���·� ���ư� �� �ʱ�ȭ
                return;
            }
            else if (dist > 4)
            {
                // �����ð� ����Ѵ�.
                currentTime += Time.deltaTime;
                // ���� �ð��� �����ٸ� ���¸� ���Ÿ� ���� ���·� ��ȯ�Ѵ�.
                if (currentTime > 1.5f)
                {
                    currentTime = 0;
                    mobType = MobType.LongAttack;
                    print("My State: AttackDelay -> LongAttack");
                    anim.SetTrigger("LongAttack");
                }
            }
            else if (dist <= 4 && !jumpAttackPerformed)
            {
                // �����ð� ����Ѵ�.
                currentTime += Time.deltaTime;
                // ���� �ð��� �����ٸ� ���¸� �ٰŸ� ���� ���·� ��ȯ�Ѵ�.
                if (currentTime > 1.5f)
                {
                    currentTime = 0;
                    mobType = MobType.JumpAttack;
                    print("My State: AttackDelay -> JumpAttack");
                    anim.SetTrigger("JumpAttack");
                    jumpAttackPerformed = true; // ���� ���� ���� �� true�� ����
                }
            }
            else if (dist <= 4 && jumpAttackPerformed)
            {
                // ���� ���� �� TailAttack���� ��ȯ
                currentTime += Time.deltaTime;
                if (currentTime > 1.5f)
                {
                    currentTime = 0;
                    mobType = MobType.TaleAttack;
                    print("My State: AttackDelay -> TaleAttack");
                    anim.SetTrigger("TaleAttack");
                    anim.SetBool("Move", true);
                }
            }
        }


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

                case MobType.TaleAttack:
                    TaleAttack();
                    break;

                case MobType.LongAttack:
                    LongAttack();
                    break;

                case MobType.JumpAttack:
                    JumpAttack();
                    break;
            }
        }


        #region ��������Ʈ �̵� �Լ�
        void WayPointMove()
        {
            CheckSight(sightRange, sightDistance);
            anim.SetFloat("MoveSpeed", moveSpeed);
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
                wayPointNum++;
                if (wayPointNum % 3 == 0)
                {
                    wayPointNum = 0;
                    StartCoroutine(MoveStop());
                }
            }
            if(!playerHideTrue)
            {
                mobType = MobType.FollowPlayer;
            }
        }
        #endregion

        #region �÷��̾� ���� �Լ�
        void FollowPlayer()
        {
            navMeshAgent.enabled = true;
            moveSpeed = currentSpeed;
            anim.SetFloat("MoveSpeed", moveSpeed);
            if (player != null)
            {
                navMeshAgent.SetDestination(player.position);
                float distanceToPlayer = Vector3.Distance(player.position, thisTransform.position);
                if (distanceToPlayer <= attackRange) // Define attackRange
                {
                    mobType = MobType.AttackDelay; // ���� ���·� ��ȯ
                }
            }
            if (playerHideTrue)
            {
                nextIndex = GetClosestWayPointIndex();
                mobType = MobType.Way_Point;
            }
        }
        #endregion

        void OnAttackComplete()
        {
            isAttackTrue = false; // ���� �Ϸ� ���·� ����
            mobType = MobType.AttackDelay; // ���� ���·� ��ȯ
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


        #region ���� �þ߰� �Լ�
        void CheckSight(float degree, float maxDistance)
        {
            // �þ� ���� �ȿ� ���� ����� �ִٸ� �� ����� Ÿ������ �����ϰ� �ʹ�.
            // �þ� ����(�þ߰�: �¿� 30��, ����, �ִ� �þ� �Ÿ�: 15����)
            // ��� ������ ���� �±�(Player) ����
            target = null;

            // 1. ���� �ȿ� ��ġ�� ������Ʈ �߿� Tag�� "Player"�� ������Ʈ�� ��� ã�´�.
            GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

            // 2. ã�� ������Ʈ�� �߿��� �Ÿ��� maxDistance �̳��� ������Ʈ�� ã�´�.
            for (int i = 0; i < players.Length; i++)
            {
                float distance = Vector3.Distance(players[i].transform.position, transform.position);

                if (distance <= maxDistance)
                {
                    // 3. ã�� ������Ʈ�� �ٶ󺸴� ���Ϳ� ���� ���� ���͸� �����Ѵ�.
                    Vector3 lookVector = players[i].transform.position - transform.position;
                    lookVector.Normalize();

                    float cosTheta = Vector3.Dot(transform.forward, lookVector);
                    float theta = Mathf.Acos(cosTheta) * Mathf.Rad2Deg;

                    // 4-1. ����, ������ ��� ���� 0���� ũ��(������ ���ʿ� �ִ�)...
                    // 4-2. ����, ���հ��� ���� 30���� ������(���� �¿� 30�� �̳�)...
                    if (cosTheta > 0 && theta < degree)
                    {
                        target = players[i].transform;
                        playerHideTrue = false;
                        if (distance > 6)
                        {
                            mobType = MobType.AttackDelay;
                        }
                    }
                }
            }
        }
        #endregion


        #region  �þ߰� �׸��� �Լ�
        // �� �׸���
        private void OnDrawGizmos()
        {
            if (!drawGizmos)
            {
                return;
            }

            Gizmos.color = new Color32(154, 14, 235, 255);

            #region �� �׸���
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

            // �þ߰� �׸���
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

