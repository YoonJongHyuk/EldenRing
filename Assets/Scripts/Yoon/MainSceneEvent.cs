using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainSceneEvent : MonoBehaviour
{

    //¿Àµð¿À
    public AudioClip[] sounds;

    AudioSource PlayerSound;

    // Start is called before the first frame update
    void Start()
    {
        
    }


    private void Awake()
    {
        PlayerSound = gameObject.GetComponent<AudioSource>();

        if (PlayerSound != null)
        {
            PlayerSound.volume = 0.1f;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
