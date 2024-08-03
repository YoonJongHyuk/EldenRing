using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int damage;

    private void OnCollisionEnter(Collision collision)
    {
        
        if(collision.gameObject.tag == "Ground")
        {
            Destroy(gameObject, 3);
        }
        else if (collision.gameObject.tag == "Wall")
        {
            Destroy(gameObject, 3);
        }




    }
}
