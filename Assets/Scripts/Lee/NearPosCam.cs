using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NearPosCam : MonoBehaviour
{
    public Transform target;
    
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        transform.position = target.position;   
    }
}