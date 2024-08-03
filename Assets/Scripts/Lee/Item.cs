using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Item : MonoBehaviour
{
    public enum Type { shield, Weapon, Arrow ,potion };
    public Type type;
    public int Value;

    void Update()
    {
        transform.Rotate(Vector3.up * 0* Time.deltaTime);
    }
    
}
