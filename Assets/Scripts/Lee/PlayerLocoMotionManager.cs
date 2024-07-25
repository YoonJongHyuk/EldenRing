using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace SG
{

    public class PlayerLocoMotionManager : MonoBehaviour
    {
        PlayerManger player;

        public float verticalMovement;
        public float horizontalMovement;
        public float moveAmount;

        private Vector3 moveDirection;

        public void HandleAllMovement()
        {
            // 기본움직임

            // 공중 움직임 

        }
        void HandleGroundedMovement()
        {
            // 플레이어 따라가는 카메라 움직임 
            moveDirection = PlayerCamera.instance.transform.forward * verticalMovement;
            moveDirection = moveDirection + PlayerCamera.instance.transform.right * horizontalMovement;
            moveDirection.Normalize();
            moveDirection.y = 0;

            //if()
            //{

            //}
        }

    }


}