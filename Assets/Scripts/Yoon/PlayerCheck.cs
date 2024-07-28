using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCheck : MonoBehaviour
{



    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            TestScripts player = other.gameObject.GetComponent<TestScripts>();
            player.hiding = false;
        }
    }

}
