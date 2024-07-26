using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;


namespace yoon
{
    public class Knight : Monster
    {
        Transform player;
        Vector3 dir;
        float moveSpeed = 5f;

        float knightDamage = 5f;
        float knightHpCount = 10f;

        TestScripts playerScript;

        Knight thisKnight;

        private void Start()
        {
            print("0");
            thisKnight = new Knight();
            knightDamage = thisKnight.knightDamage;
            knightHpCount = thisKnight.knightHpCount;

            player = GameObject.FindWithTag("Player").transform;
            
            playerScript = player.GetComponent<TestScripts>();
            print(playerScript.hpCount);
        }

        private void Update()
        {
            UpdateState();
        }

        public override void Attack()
        {
            playerScript.hpCount -= knightDamage;
            print(playerScript.hpCount);
        }
        public override void Die()
        {

        }
        public override void Move()
        {

        }
        public override void TargetPlayer()
        {
            if (player != null)
            {
                if (playerScript.hiding)
                {
                    dir = (player.transform.position - transform.position).normalized;
                    transform.position += dir * moveSpeed * Time.deltaTime;
                }
            }
        }
        public override void EnterState()
        {

        }
        public override void UpdateState()
        {
            TargetPlayer();
        }
        public override void ExitState()
        {

        }

        private void OnTriggerEnter(Collider other)
        {
            if (other.gameObject.CompareTag("Player"))
            {
                Attack();
            }
        }

    }
}

