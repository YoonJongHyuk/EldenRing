using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace yoon
{
    public class Boss : MonoBehaviour
    {

        public enum BossPattern
        {
            // 1페이즈 ============================================================
            Idle,                 // 기본 상태
            Cry,                  // 울부짖기
            LegSweepAttack,       // 앞발 휩쓸기 공격
            LegFoundAttack,       // 앞발 찍기 공격
            TaleAttack,           // 꼬리 휘두르기 공격
            DiagonalAvoidance,    // 대각선 회피
            DiagonalTackle,       // 대각선 몸통 박치기
            // 2페이즈 ============================================================
            FireCry2,             // 울부짖기 후 메테오
            FireBreath2,          // 직선 브레스
            Fly2,                 // 비행 시작
            Flying2,              // 비행 중
            Fall2,                // 추락
            FanShapedFireBreath2, // 부채꼴 브레스
            LegSweepAttack2,      // 앞발 휩쓸기 공격 2배속
            LegFoundAttack2,      // 앞발 찍기 공격 2배속
            DiagonalAvoidance2,   // 대각선 회피 2배속
            DiagonalTackle2,      // 대각선 몸통 박치기 2배속
            Death                 // 사망
        }

        BossPattern bossPattern = BossPattern.Idle;

        Animator anim;


        int bossAttackPower;
        

        


        private void Start()
        {
            
        }

        private void Update()
        {

        }

        void Setting()
        {
            anim = GetComponent<Animator>();
        }

        void AttackPower(int value)
        {
            bossAttackPower = value;
        }


        public void ChangePattern(BossPattern newPattern)
        {
            StopCoroutine(bossPattern.ToString());

            bossPattern = newPattern;

            StartCoroutine(bossPattern.ToString());
        }

        IEnumerator Idle()
        {
            anim.SetTrigger("Idle");
            yield return null;
        }

        IEnumerator Cry()
        {
            anim.SetTrigger("Cry");
            yield return null;
        }

    }
}
