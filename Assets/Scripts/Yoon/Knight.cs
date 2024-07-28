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
        float rotationSpeedMultiplier = 1f; // 이동 속도에 따른 회전 속도 증가 비율
        private int nextIndex = 0;

        Transform player;
        Transform thisTransform;
        Vector3 dir;

        public Transform wayPointGroup; // 인스펙터 창에서 할당할 수 있도록 public으로 설정
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
            // 구현 필요
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
            thisTransform = GetComponent<Transform>();
            navMeshAgent = GetComponent<NavMeshAgent>();

            if (wayPointGroup != null)
            {
                points = wayPointGroup.GetComponentsInChildren<Transform>();

                // 첫 번째 요소(부모)를 건너뛰고 배열을 초기화
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
                    // moveType이 FollowPlayer에서 Way_Point로 변경될 때
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
            // 구현 필요
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
