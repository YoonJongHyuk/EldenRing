using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

namespace yoon
{
    public class Boss : MonoBehaviour
    {
        public enum BossPattern
        {
            // 보스 애니메이션 상태
            // 보스 기본 상태 ==============
            Move,
            TurnLeft,
            TurnRight,
            Idle,
            // 보스 1페이즈 ================
            Cry,
            LegSweepAttack,
            LegFoundAttack,
            TaleAttack,
            DiagonalAvoidance,
            DiagonalTackle,
            // 보스 2페이즈 ================
            // isFlyTrue == false ==========
            FireCry2,
            FireBreath2,
            Fly2,
            // =========================

            // isFlyTrue == true
            Flying2,
            Fall2,
            // =========================

            // isFlyTrue == false ==========
            FanShapedFireBreath2,
            LegSweepAttack2,
            LegFoundAttack2,
            DiagonalAvoidance2,
            DiagonalTackle2,
            Death
            // =============================
        }

        public enum BossPhase
        {
            Phase1,
            Phase2
        }

        public GameObject player;

        BossPattern bossPattern = BossPattern.Idle;
        BossPhase bossPhase = BossPhase.Phase1;

        Animator anim;

        NavMeshAgent navMeshAgent;

        int bossAttackPower;

        public int bossHP;
        public int bossMaxHP;

        [SerializeField]
        private Slider _hpBar;

        [SerializeField]
        private Slider _nextHpBar; // nextHP 용 슬라이더 추가

        float bossSpeed = 0.0f;
        float distance;

        public bool fristPlayerCheck = false;
        private bool BattleStartTrue = false;
        public Transform breathPos;

        public bool canPlay = false;
        bool isFlyTrue = false;
        bool isNearTrue = true;

        public int patternCount;

        /*
         패턴 설명
        0 = Idle 상태
        1 = 이동, 보통은 플레이어 접근
        2 = 근접 공격(LegSweepAttack,LegFoundAttack,TaleAttack)
        3 = 회피 후 몸통 박치기(DiagonalAvoidance)
        4 = Cry 후 1번으로 변경
        5 = 플레이어와의 거리 체크
        */


        private void Start()
        {
            Setting();
        }

        private void Update()
        {
            StartScene();
            LoopAnimationCheck();
            MoveTowardsPlayer();



            if(patternCount == 0)
            {
                ChangePattern(BossPattern.Idle);
            }
            else if(patternCount == 1)
            {
                PlayerChase();
            }
            else if(patternCount == 2)
            {
                RandomAttack();
            }
            else if (patternCount == 3)
            {
                return;
            }
            else if (patternCount == 4)
            {
                ChangePattern(BossPattern.Cry);
            }
            else if(patternCount == 5)
            {
                DistanceCheck();
            }


            #region 버튼 테스트
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangePattern(BossPattern.FireCry2);
            }
            


            if (Input.GetKeyDown(KeyCode.R))
            {
                ChangePattern(BossPattern.Fly2);
            }
            if (Input.GetKeyDown(KeyCode.S))
            {
                ChangePattern(BossPattern.Fall2);
            }
            #endregion

        }


        void Setting()
        {
            bossHP = bossMaxHP;
            navMeshAgent = GetComponent<NavMeshAgent>();
            player = GameObject.FindGameObjectWithTag("Player");
            anim = GetComponent<Animator>();
            anim.SetTrigger("Idle");
            anim.SetBool("isFlyTrue", false);
        }

        void StartScene()
        {
            if (fristPlayerCheck)
            {
                ChangePattern(BossPattern.Cry);
                fristPlayerCheck = false;
            }
        }

        void CryAfter()
        {
            patternCount = 1;
        }

        void MoveTowardsPlayer()
        {
            Vector3 currentVelocity = navMeshAgent.velocity;
            bossSpeed = currentVelocity.magnitude;
            
        }

        // 플레이어 쫒기
        void PlayerChase()
        {
            navMeshAgent.SetDestination(player.transform.position);
            distance = Vector3.Distance(transform.position, player.transform.position);
            if (distance <= 5)
            {
                navMeshAgent.ResetPath();
                patternCount++;
            }
        }

        void DistanceCheck()
        {
            distance = Vector3.Distance(transform.position, player.transform.position);
            TurnToPlayer();
            if (distance > 5)
            {
                patternCount = 1;
            }
            else
            {
                patternCount = 2;
            }
        }


        void TurnToPlayer()
        {
            transform.LookAt(player.transform.position);
        }

        // 1페이즈 근접공격
        void RandomAttack()
        {
            int num = Random.Range(0, 3);
            switch (num)
            {
                case 0:
                    ChangePattern(BossPattern.LegSweepAttack);
                    break;
                case 1:
                    ChangePattern(BossPattern.LegFoundAttack);
                    break;
                case 2:
                    ChangePattern(BossPattern.TaleAttack);
                    break;
            }
            patternCount = 5;
        }

        void RandomPattern()
        {
            int patternNum;
            if (bossPhase == BossPhase.Phase1)
            {
                patternNum = UnityEngine.Random.Range((int)BossPattern.Cry, (int)BossPattern.DiagonalTackle + 1);
            }
            else
            {
                patternNum = UnityEngine.Random.Range((int)BossPattern.FireCry2, (int)BossPattern.DiagonalTackle2 + 1);
            }

            ChangePattern((BossPattern)patternNum);
        }

        public void ChangePattern(BossPattern newPattern)
        {
            StartCoroutine(ChangePatternWhenReady(newPattern));
        }

        private IEnumerator ChangePatternWhenReady(BossPattern newPattern)
        {
            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime < 1.0f)
            {
                yield return null;
            }

            StopCoroutine(bossPattern.ToString());
            bossPattern = newPattern;
            StartCoroutine(HandleAnimation(bossPattern.ToString()));
        }

        void Flying()
        {
            isFlyTrue = !isFlyTrue;

            anim.SetBool("isFlyTrue", isFlyTrue);
            print(isFlyTrue);
        }

        void LoopAnimationCheck()
        {
            if (!isFlyTrue)
            {
                anim.SetFloat("Speed", bossSpeed);
            }
            else if (isFlyTrue)
            {
                anim.SetTrigger("Flying2");
            }
        }

        IEnumerator HandleAnimation(string animationName)
        {
            anim.SetTrigger(animationName);

            while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
            {
                yield return null;
            }
        }

        // Example of Boss Phase Transition
        public void TransitionToPhase2()
        {
            bossPhase = BossPhase.Phase2;
            ChangePattern(BossPattern.FireCry2);
        }

        //IEnumerator Move() { yield return StartCoroutine(HandleAnimation("Move")); }
        IEnumerator TurnLeft() { yield return StartCoroutine(HandleAnimation("TurnLeft")); }
        IEnumerator TurnRight() { yield return StartCoroutine(HandleAnimation("TurnRight")); }
        //IEnumerator Idle() { yield return StartCoroutine(HandleAnimation("Idle")); }
        IEnumerator Cry() { yield return StartCoroutine(HandleAnimation("Cry")); }
        IEnumerator LegSweepAttack() { yield return StartCoroutine(HandleAnimation("LegSweepAttack")); }
        IEnumerator LegFoundAttack() { yield return StartCoroutine(HandleAnimation("LegFoundAttack")); }
        IEnumerator TaleAttack() { yield return StartCoroutine(HandleAnimation("TaleAttack")); }
        IEnumerator DiagonalAvoidance() { yield return StartCoroutine(HandleAnimation("DiagonalAvoidance")); }
        //IEnumerator DiagonalTackle() { yield return StartCoroutine(HandleAnimation("DiagonalTackle")); }
        IEnumerator FireCry2() { yield return StartCoroutine(HandleAnimation("FireCry2")); }
        IEnumerator FireBreath2() { yield return StartCoroutine(HandleAnimation("FireBreath2")); }
        IEnumerator Fly2() { yield return StartCoroutine(HandleAnimation("Fly2")); }
        //IEnumerator Flying2() { yield return StartCoroutine(HandleAnimation("Flying2")); }
        IEnumerator Fall2() { yield return StartCoroutine(HandleAnimation("Fall2")); }
        IEnumerator FanShapedFireBreath2() { yield return StartCoroutine(HandleAnimation("FanShapedFireBreath2")); }
        IEnumerator LegSweepAttack2() { yield return StartCoroutine(HandleAnimation("LegSweepAttack2")); }
        IEnumerator LegFoundAttack2() { yield return StartCoroutine(HandleAnimation("LegFoundAttack2")); }
        IEnumerator DiagonalAvoidance2() { yield return StartCoroutine(HandleAnimation("DiagonalAvoidance2")); }
        //IEnumerator DiagonalTackle2() { yield return StartCoroutine(HandleAnimation("DiagonalTackle2")); }
        IEnumerator Death() { yield return StartCoroutine(HandleAnimation("Death")); }

    }
}
