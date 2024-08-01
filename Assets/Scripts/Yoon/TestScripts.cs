using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using yoon;

public class TestScripts : MonoBehaviour, IState, IInput
{
    public bool hiding;
    public int hpCount = 20;
    void Awake()
    {
        hiding = true;
        EnterState();
    }

    private void Update()
    {
        UpdateState();
    }

    private void FixedUpdate()
    {
        float h = Input.GetAxis("Horizontal");
        float v = Input.GetAxis("Vertical");

        Vector3 dir = new Vector3(h, 0, v);
        transform.position += dir * 5f * Time.deltaTime;
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
        if (Input.GetKeyDown(KeyCode.E))
        {
            Scorpion scorpion = GameObject.Find("Scorpion").GetComponent<Scorpion>();
            scorpion.GetDamage(20);
        }
    }
}
