using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;

public class OpenCheck : MonoBehaviour
{
    public Boss boss;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            boss.fristPlayerCheck = true;
            Destroy(gameObject);
        }
    }
}
