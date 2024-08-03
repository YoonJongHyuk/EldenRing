using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Test : MonoBehaviour
{
    public int maxHP;
    public int currentHP;

    Rigidbody rigid;
    BoxCollider boxCollider;

    private void Awake()
    {
        rigid = GetComponent<Rigidbody>();
        boxCollider = GetComponent<BoxCollider>();

    }
    private void OnTriggerEnter(Collider other)
    {
        if(other.tag  == "Melee")
        {
            Sward Sward = other.GetComponent<Sward>();
            currentHP -= Sward.damage;

            print("Melee : " + currentHP);
        }
        else if (other.tag == "Weapon")
        {
            Arrow arrow = other.GetComponent<Arrow>();
         //
         //
         //
         //
         //
         //currentHP -= Sward.damage;
            
            print("Range" + currentHP);
        }
    }
}
