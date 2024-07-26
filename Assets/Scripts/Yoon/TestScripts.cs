using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;

public class TestScripts : MonoBehaviour, IState, IInput
{
    public bool hiding;
    public float hpCount = 20f;
    void Awake()
    {
        hiding = true;
        EnterState();
    }

    private void Update()
    {
        UpdateState();
    }

    

    void HidingChange()
    {
        hiding = !hiding;
    }

    public void EnterState()
    {

    }

    public void UpdateState()
    {
        InputFuc();
    }

    public void ExitState()
    {

    }

    private void OnDisable()
    {
        ExitState();
    }

    public void InputFuc()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            HidingChange();
        }
    }
}
