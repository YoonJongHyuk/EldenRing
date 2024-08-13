using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace yoon
{
    public class Boss : MonoBehaviour
    {

        public enum BossPattern
        {
            // 1������ ============================================================
            Idle,                 // �⺻ ����
            Cry,                  // ���¢��
            LegSweepAttack,       // �չ� �۾��� ����
            LegFoundAttack,       // �չ� ��� ����
            TaleAttack,           // ���� �ֵθ��� ����
            DiagonalAvoidance,    // �밢�� ȸ��
            DiagonalTackle,       // �밢�� ���� ��ġ��
            // 2������ ============================================================
            FireCry2,             // ���¢�� �� ���׿�
            FireBreath2,          // ���� �극��
            Fly2,                 // ���� ����
            Flying2,              // ���� ��
            Fall2,                // �߶�
            FanShapedFireBreath2, // ��ä�� �극��
            LegSweepAttack2,      // �չ� �۾��� ���� 2���
            LegFoundAttack2,      // �չ� ��� ���� 2���
            DiagonalAvoidance2,   // �밢�� ȸ�� 2���
            DiagonalTackle2,      // �밢�� ���� ��ġ�� 2���
            Death                 // ���
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
