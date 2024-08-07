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


        #region �̵� �Լ�
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
                    navMeshAgent.enabled = false; // NavMeshAgent ��Ȱ��ȭ
                    if (points.Length == 0) return;

                    CheckSight(sightRange, sightDistance);

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
                    break;

                case MoveType.FollowPlayer:
                    navMeshAgent.enabled = true; // NavMeshAgent Ȱ��ȭ
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

            if (playerHideTrue)
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
                    }
                }
            }
        }


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

    }
}


