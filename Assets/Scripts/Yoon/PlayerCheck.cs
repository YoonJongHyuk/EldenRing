using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheck : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            PlayerContorler player = other.gameObject.GetComponent<PlayerContorler>();
            player.hiding = false;
        }
    }

}
