using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StartButton : MonoBehaviour
{

    public GameObject startPressButton;
    public GameObject buttonListPanel;

    private void Update()
    {
        StartButtonClick();
    }

    public void StartButtonClick()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            startPressButton.SetActive(false);
            buttonListPanel.SetActive(true);
            Destroy(gameObject);
        }
    }
}
