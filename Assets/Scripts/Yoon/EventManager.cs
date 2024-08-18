using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EventManager : MonoBehaviour
{
    public AudioClip[] sounds;

    AudioSource PlayerSound;

    private void Update()
    {
        ExitButton();
    }

    void ExitButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }




    //void ReStartButton()
    //{
    //    if(Input.GetKeyDown(KeyCode.f5))
    //}

}
