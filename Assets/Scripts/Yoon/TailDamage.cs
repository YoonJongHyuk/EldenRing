using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;

public class TailDamage : MonoBehaviour
{
    public int damageAmount = 35; // 플레이어에게 줄 데미지

    public Boss boss;

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player") && boss.isAttackTrue)
        {
            PlayerContorler player = other.GetComponent<PlayerContorler>();
            player.GetDamage(damageAmount);
            player.playerHit = true;
            boss.isAttackTrue = false;
            print("다리 맞음");
        }
    }
}
