using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneEvents : MonoBehaviour
{
    public void MainSceneStart()
    {
        SceneManager.LoadScene(1);
    }
}
