using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartSceneEvent : MonoBehaviour
{
    public GameObject StartUI;

    private void Awake()
    {
        StartCoroutine(IStartSceneEvent());
    }

    IEnumerator IStartSceneEvent()
    {
        StartUI.SetActive(true);
        yield return new WaitForSeconds(3);
        StartUI.SetActive(false);
    }
}
