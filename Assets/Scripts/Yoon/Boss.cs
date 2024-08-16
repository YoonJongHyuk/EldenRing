using KINEMATION.KAnimationCore.Runtime.Attributes;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
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
        public GameObject fireParticle;

        public BoxCollider tailBox;
        public BoxCollider legBox;
        public SphereCollider bossBodyBox;


        BossPattern bossPattern = BossPattern.Idle;
        BossPhase bossPhase = BossPhase.Phase1;

        Animator anim;

        NavMeshAgent navMeshAgent;

        Vector3 bossYRot;

        int bossAttackPower;

        public int bossHP;
        public int bossMaxHP;

        [SerializeField]
        private Slider _hpBar;

        [SerializeField]
        private Slider _nextHpBar; // nextHP 용 슬라이더 추가

        float bossSpeed = 0.0f;
        float distance;
        float bossHPHalf;

        public bool fristPlayerCheck = false;
        private bool BattleStartTrue = false;
        public Transform breathPos;

        public bool canPlay = false;
        bool isFlyTrue = false;
        bool isNearTrue = true;
        bool onePaseStart = true;
        bool twoPaseStart = false;
        public bool isAttackTrue = false;
        public bool playerHit = false;

        public int patternCount;

        /*
         패턴 설명
        0 = Idle 상태
        1 = 이동, 보통은 플레이어 접근
        2 = 근접 공격(LegSweepAttack,LegFoundAttack,TaleAttack) / 회피 후 몸통 박치기(DiagonalAvoidance)
        
        2 페이즈

        3 = 
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


            if (patternCount == 0)
            {
                ChangePattern(BossPattern.Idle);
            }
            else if (patternCount == 1)
            {
                PlayerChase();
            }

            if (bossHP > bossHPHalf)
            {
                if (patternCount == 2)
                {
                    if(OnePaseAttack < 3)
                    {
                        patternCount = 99;
                        RandomAttack();
                    }
                    else if(OnePaseAttack >= 3)
                    {
                        OnePaseAttack = 0;
                        patternCount = 99;
                        ChangePattern(BossPattern.DiagonalAvoidance);
                    }
                }
            }
            else if(bossHP <= bossHPHalf)
            {
                
                print("체력 절반");
                if (patternCount == 3 && !twoPaseStart)
                {
                    print("패턴3");
                    twoPaseStart = true;
                    navMeshAgent.ResetPath();
                    ChangePattern(BossPattern.FireCry2);
                }
                else if(patternCount == 4)
                {
                    
                }
            }
            else if(bossHP <= 0)
            {
                bossHP = 0;
                ChangePattern(BossPattern.Death);
            }
            
            

            if (onePaseStart && bossHP <= bossHPHalf)
            {
                onePaseStart = false;
                patternCount = 3;
            }


            if (patternCount == 99)
            {
                // 패턴 딜레이
            }




            #region 버튼 테스트
            if (Input.GetKeyDown(KeyCode.H))
            {
                ChangePattern(BossPattern.FireBreath2);
            }

            if (Input.GetKeyDown(KeyCode.J))
            {
                ChangePattern(BossPattern.FanShapedFireBreath2);
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
            bossHPHalf = bossMaxHP / 2;
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


        void OnePaseAttackAfter()
        {
            patternCount = 1;
        }

        void HitStart()
        {
            isAttackTrue = true;
        }

        void HitEnd()
        {
            isAttackTrue = false;
        }

        void BreathStart()
        {
            fireParticle.SetActive(true);
        }

        void BreathEnd()
        {
            fireParticle.SetActive(false);
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
            

            Vector3 dir = new Vector3(transform.position.x, transform.position.y + 1, transform.position.z);
            Ray ray = new Ray(transform.position, transform.forward);
            RaycastHit hitInfo;

            
            if (Physics.Raycast(ray, out hitInfo, 5, (1 << 11)))
            {
                navMeshAgent.ResetPath();
                if (onePaseStart)
                {
                    patternCount = 2;
                }
                else if (twoPaseStart)
                {
                    patternCount = 4;
                }
            }
            else
            {
                TurnToPlayer();
            }
        }

        float rotationSpeed = 2.0f;

        void TurnToPlayer()
        {
            // 목표 회전 방향 계산
            Vector3 direction = player.transform.position - transform.position;
            direction.y = 0; // y축 회전을 무시하여 보스가 수평면에서만 회전하도록 함
            Quaternion targetRotation = Quaternion.LookRotation(direction);

            // 현재 회전에서 목표 회전으로 부드럽게 회전
            float step = rotationSpeed * Time.deltaTime; // 회전 속도 조절
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRotation, step);
        }

        int OnePaseAttack = 0;

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

            OnePaseAttack++;
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
