using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace yoon
{
    public class Boss : MonoBehaviour
    {
        public enum BossPattern
        {
            Move,
            TurnLeft,
            TurnRight,
            Idle,
            Cry,
            LegSweepAttack,
            LegFoundAttack,
            TaleAttack,
            DiagonalAvoidance,
            DiagonalTackle,
            FireCry2,
            FireBreath2,
            Fly2,
            Flying2,
            Fall2,
            FanShapedFireBreath2,
            LegSweepAttack2,
            LegFoundAttack2,
            DiagonalAvoidance2,
            DiagonalTackle2,
            Death
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

        int bossAttackPower;
        int patternCount;

        float bossSpeed = 1.0f;

        public bool fristPlayerCheck = false;
        private bool BattleStartTrue = false;
        public Transform breathPos;

        public bool canPlay = false;
        bool isFlyTrue = false;

        private void Start()
        {
            Setting();
        }

        private void Update()
        {
            StartScene();

            if (canPlay)
            {
                RandomPattern();
                canPlay = false; // 한 번 실행 후 멈추도록
            }
        }

        void Setting()
        {
            anim = GetComponent<Animator>();
            patternCount = Enum.GetValues(typeof(BossPattern)).Length;
        }

        void StartScene()
        {
            if (fristPlayerCheck)
            {
                canPlay = true;
                //ChangePattern(BossPattern.Cry);
                fristPlayerCheck = false;
                //BattleStartTrue = true;
            }
        }

        void MoveTowardsPlayer()
        {
            //Vector3 dir = 
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
            StartCoroutine(HandleAnimation(bossPattern.ToString(), IsLoopingPattern(bossPattern), bossSpeed, isFlyTrue));
        }

        private bool IsLoopingPattern(BossPattern pattern)
        {
            switch (pattern)
            {
                case BossPattern.Idle:
                case BossPattern.Move:
                case BossPattern.Flying2:
                    return true;
                default:
                    return false;
            }
        }

        IEnumerator HandleAnimation(string animationName, bool isLooping, float speed, bool isFlyTrue)
        {
            anim.SetTrigger(animationName);

            if (isLooping)
            {
                while (true)
                {
                    if (animationName == "Idle" && speed > 0)
                    {
                        anim.SetTrigger("Move");
                        break;
                    }
                    else if (animationName == "Move" && speed == 0)
                    {
                        anim.SetTrigger("Idle");
                        break;
                    }
                    else if (animationName == "Flying2" && !isFlyTrue)
                    {
                        anim.SetTrigger("Idle");
                        break;
                    }

                    yield return null;
                }
            }
            else
            {
                while (anim.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1.0f)
                {
                    canPlay = true;
                    yield return null;
                }
            }
        }

        // Example of Boss Phase Transition
        public void TransitionToPhase2()
        {
            bossPhase = BossPhase.Phase2;
            ChangePattern(BossPattern.FireCry2);
        }

        IEnumerator Move() { yield return StartCoroutine(HandleAnimation("Move", true, bossSpeed, false)); }
        IEnumerator TurnLeft() { yield return StartCoroutine(HandleAnimation("TurnLeft", false, bossSpeed, false)); }
        IEnumerator TurnRight() { yield return StartCoroutine(HandleAnimation("TurnRight", false, bossSpeed, false)); }
        IEnumerator Idle() { yield return StartCoroutine(HandleAnimation("Idle", true, bossSpeed, false)); }
        IEnumerator Cry() { yield return StartCoroutine(HandleAnimation("Cry", false, bossSpeed, false)); }
        IEnumerator LegSweepAttack() { yield return StartCoroutine(HandleAnimation("LegSweepAttack", false, bossSpeed, false)); }
        IEnumerator LegFoundAttack() { yield return StartCoroutine(HandleAnimation("LegFoundAttack", false, bossSpeed, false)); }
        IEnumerator TaleAttack() { yield return StartCoroutine(HandleAnimation("TaleAttack", false, bossSpeed, false)); }
        IEnumerator DiagonalAvoidance() { yield return StartCoroutine(HandleAnimation("DiagonalAvoidance", false, bossSpeed, false)); }
        IEnumerator DiagonalTackle() { yield return StartCoroutine(HandleAnimation("DiagonalTackle", false, bossSpeed, false)); }
        IEnumerator FireCry2() { yield return StartCoroutine(HandleAnimation("FireCry2", false, bossSpeed, false)); }
        IEnumerator FireBreath2() { yield return StartCoroutine(HandleAnimation("FireBreath2", false, bossSpeed, false)); }
        IEnumerator Fly2() { yield return StartCoroutine(HandleAnimation("Fly2", false, bossSpeed, false)); }
        IEnumerator Flying2() { yield return StartCoroutine(HandleAnimation("Flying2", true, bossSpeed, true)); }
        IEnumerator Fall2() { yield return StartCoroutine(HandleAnimation("Fall2", false, bossSpeed, false)); }
        IEnumerator FanShapedFireBreath2() { yield return StartCoroutine(HandleAnimation("FanShapedFireBreath2", false, bossSpeed, false)); }
        IEnumerator LegSweepAttack2() { yield return StartCoroutine(HandleAnimation("LegSweepAttack2", false, bossSpeed, false)); }
        IEnumerator LegFoundAttack2() { yield return StartCoroutine(HandleAnimation("LegFoundAttack2", false, bossSpeed, false)); }
        IEnumerator DiagonalAvoidance2() { yield return StartCoroutine(HandleAnimation("DiagonalAvoidance2", false, bossSpeed, false)); }
        IEnumerator DiagonalTackle2() { yield return StartCoroutine(HandleAnimation("DiagonalTackle2", false, bossSpeed, false)); }
        IEnumerator Death() { yield return StartCoroutine(HandleAnimation("Death", false, bossSpeed, false)); }
    }
}
