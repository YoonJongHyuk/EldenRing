using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    // 리스폰 포인트에 플레이어가 들어왔을 때 호출되는 메서드
    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerContorler player = other.GetComponent<PlayerContorler>();
            player.canSetRespawn = true;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            PlayerContorler player = other.GetComponent<PlayerContorler>();
            player.canSetRespawn = true;
        }
    }
}
