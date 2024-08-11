using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Respawn : MonoBehaviour
{

    // ������ ����Ʈ�� �÷��̾ ������ �� ȣ��Ǵ� �޼���
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
