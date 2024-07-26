using System.Collections;
using System.Collections.Generic;
using UnityEngine;


namespace yoon
{
    public abstract class Monster : MonoBehaviour, IState, IMonsterState
    {
        public virtual void Attack()
        {
        }

        public virtual void Die()
        {

        }
        public virtual void Move()
        {

        }

        public virtual void TargetPlayer()
        {

        }

        public virtual void EnterState()
        {
        }

        public virtual void ExitState()
        {
        }
        public virtual void UpdateState()
        {

        }




    }
}

