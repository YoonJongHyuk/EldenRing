using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using yoon;

namespace yoon
{
    public class Knight : Monster
    {
        public enum MoveType
        {
            Way_Point,
            FollowPlayer
        }

        public MoveType moveType = MoveType.Way_Point;
        public float moveSpeed = 1f;
        float damping = 3f;
        float rotationSpeedMultiplier = 1f; // �̵� �ӵ��� ���� ȸ�� �ӵ� ���� ����
        private int nextIndex = 0;

        Transform player;
        Transform thisTransform;
        Vector3 dir;

        public Transform wayPointGroup; // �ν����� â���� �Ҵ��� �� �ֵ��� public���� ����
        private Transform[] points;

        float knightDamage = 5f;
        float knightHpCount = 10f;

        TestScripts playerScript;
        NavMeshAgent navMeshAgent;

        private void Start()
        {
            EnterState();
        }

        private void Update()
        {
            UpdateState();
        }

        public override void Attack()
        {
            playerScript.hpCount -= knightDamage;
        }

        public override void Die()
        {
            // ���� �ʿ�
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
            thisTransform = GetComponent<Transform>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            if (wayPointGroup != null)
            {
                points = wayPointGroup.GetComponentsInChildren<Transform>();

                // ù ��° ���(�θ�)�� �ǳʶٰ� �迭�� �ʱ�ȭ
                List<Transform> pointList = new List<Transform>(points);
                pointList.RemoveAt(0);
                points = pointList.ToArray();
            }

            player = GameObject.FindWithTag("Player").transform;
            playerScript = player.GetComponent<TestScripts>();
        }

        public override void UpdateState()
        {
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
            Move();
        }

        public override void ExitState()
        {
            // ���� �ʿ�
        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Attack();
            }
        }

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
    }
}
