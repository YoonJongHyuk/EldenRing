//using System.Collections;
//using System.Collections.Generic;
//using UnityEngine;

//public class PlayerCheck : MonoBehaviour
//{

//    enum PlayerScript
//    {
//        TestScript,
//        PlayerController
//    }

//    PlayerScript playerScript = PlayerScript.TestScript;

//    private void OnTriggerEnter(Collider other)
//    {
//        if (other.gameObject.CompareTag("Player"))
//        {
//            switch(playerScript)
//            {
//                case PlayerScript.TestScript:
//                    TestScripts playered = other.gameObject.GetComponent<TestScripts>();
//                    playered.hiding = false;
//                    break;
//                case PlayerScript.PlayerController:
//                    PlayerContorler player = other.gameObject.GetComponent<PlayerContorler>();
//                    player.hiding = false;
//                    break;
//            }
            
//        }
//    }

//}
