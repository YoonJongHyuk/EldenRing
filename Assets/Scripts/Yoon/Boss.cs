using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace yoon
{
    public class Boss : MonoBehaviour
    {

        public enum BossPattern
        {
            // 기본상태 ============================================================
            Move,                 // 이동
            TurnLeft,             // 왼쪽 이동
            TurnRight,            // 오른쪽 이동
            Idle,                 // 기본 상태
            // 1페이즈  ============================================================
            Cry,                  // 울부짖기
            LegSweepAttack,       // 앞발 휩쓸기 공격
            LegFoundAttack,       // 앞발 찍기 공격
            TaleAttack,           // 꼬리 휘두르기 공격
            DiagonalAvoidance,    // 대각선 회피
            DiagonalTackle,       // 대각선 몸통 박치기
            // 2페이즈  ============================================================
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

        public bool fristPlayerCheck = false;

        private void Start()
        {
            Setting();
        }

        private void Update()
        {
            StartScene(); 
            // 플레이어 입장 완료 시 Cry =========================
        }

        void StartScene()
        {
            if (fristPlayerCheck)
            {
                ChangePattern(BossPattern.Cry);
                fristPlayerCheck = false;
            }
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
        IEnumerator Move()
        {
            anim.SetTrigger("Move");
            Debug.Log("Move");
            yield return null;
        }
        IEnumerator TurnLeft()
        {
            anim.SetTrigger("TurnLeft");
            Debug.Log("TurnLeft");
            yield return null;
        }
        IEnumerator TurnRight()
        {
            anim.SetTrigger("TurnRight");
            Debug.Log("TurnRight");
            yield return null;
        }

        IEnumerator Idle()
        {
            anim.SetTrigger("Idle");
            yield return null;
        }

        IEnumerator Cry()
        {
            anim.SetTrigger("Cry");
            Debug.Log("Cry");
            yield return null;
        }

        IEnumerator LegSweepAttack()
        {
            anim.SetTrigger("LegSweepAttack");
            Debug.Log("LegSweepAttack");
            yield return null;
        }

        IEnumerator LegFoundAttack()
        {
            anim.SetTrigger("LegFoundAttack");
            Debug.Log("LegFoundAttack");
            yield return null;
        }

        IEnumerator TaleAttack()
        {
            anim.SetTrigger("TaleAttack");
            Debug.Log("TaleAttack");
            yield return null;
        }

        IEnumerator DiagonalAvoidance()
        {
            anim.SetTrigger("DiagonalAvoidance");
            Debug.Log("DiagonalAvoidance");
            yield return null;
        }

        IEnumerator DiagonalTackle()
        {
            anim.SetTrigger("DiagonalTackle");
            Debug.Log("DiagonalTackle");
            yield return null;
        }
        IEnumerator FireCry2()
        {
            anim.SetTrigger("FireCry2");
            Debug.Log("FireCry2");
            yield return null;
        }
        IEnumerator FireBreath2()
        {
            anim.SetTrigger("FireBreath2");
            Debug.Log("FireBreath2");
            yield return null;
        }
        IEnumerator Fly2()
        {
            anim.SetTrigger("Fly2");
            Debug.Log("Fly2");
            yield return null;
        }
        IEnumerator Flying2()
        {
            anim.SetTrigger("Flying2");
            Debug.Log("Flying2");
            yield return null;
        }
        IEnumerator Fall2()
        {
            anim.SetTrigger("Fall2");
            Debug.Log("Fall2");
            yield return null;
        }
        IEnumerator FanShapedFireBreath2()
        {
            anim.SetTrigger("FanShapedFireBreath2");
            Debug.Log("FanShapedFireBreath2");
            yield return null;
        }
        IEnumerator LegSweepAttack2()
        {
            anim.SetTrigger("LegSweepAttack2");
            Debug.Log("LegSweepAttack2");
            yield return null;
        }
        IEnumerator LegFoundAttack2()
        {
            anim.SetTrigger("LegFoundAttack2");
            Debug.Log("LegFoundAttack2");
            yield return null;
        }
        IEnumerator DiagonalAvoidance2()
        {
            anim.SetTrigger("DiagonalAvoidance2");
            Debug.Log("DiagonalAvoidance2");
            yield return null;
        }
        IEnumerator DiagonalTackle2()
        {
            anim.SetTrigger("DiagonalTackle2");
            Debug.Log("DiagonalTackle2");
            yield return null;
        }
        IEnumerator Death()
        {
            anim.SetTrigger("Death");
            Debug.Log("Death");
            yield return null;
        }
    }
}
