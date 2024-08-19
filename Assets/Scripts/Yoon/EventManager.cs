using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class EventManager : MonoBehaviour
{
    public AudioClip[] sounds;

    AudioSource PlayerSound;

    private void Update()
    {
        ExitButton();
        ReStartButton();
    }

    void ExitButton()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            Application.Quit();
        }
    }




    void ReStartButton()
    {
        if (Input.GetKeyDown(KeyCode.F5))
        {
            SceneManager.LoadScene(1);
        }
    }

}
