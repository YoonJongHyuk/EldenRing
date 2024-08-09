using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackCheck : MonoBehaviour
{
    FinalMob mob;

    private void Start()
    {
        mob = transform.parent.GetComponent<FinalMob>();
    }


    //private void OnTriggerEnter(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        print("내 뒤에 플레이어가 " + mob.isBackPlayer);
    //        mob.isBackPlayer = true;
    //    }
    //}

    //private void OnTriggerExit(Collider other)
    //{
    //    if (other.gameObject.CompareTag("Player"))
    //    {
    //        print("내 뒤에 플레이어가 " + mob.isBackPlayer);
    //        mob.isBackPlayer = false;
    //    }
    //}
}
