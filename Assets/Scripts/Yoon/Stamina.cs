using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Stamina : MonoBehaviour
{
    PlayerContorler player;

    float currentTime;
    float nextStamina;
    private float lerpDuration = 1.5f; // t스태미나가 천천히 빠질 시간 (1.5초)
    private float delayDuration = 1.5f; // 딜레이



    [SerializeField]
    private Slider _staminaBar;

    [SerializeField]
    private Slider _nextStaminaBar; // nextHP 용 슬라이더 추가
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerContorler>();
    }

    void Update()
    {

        nextStamina = player.currentStamina;
        _staminaBar.value = player.currentStamina;
        StaminaBar();
    }

    void StaminaBar()
    {
        if (_nextStaminaBar == null) return; // nextHP 슬라이더가 할당되지 않았으면 리턴

        if (_nextStaminaBar.value != nextStamina)
        {
            currentTime += Time.deltaTime;

            if (currentTime >= delayDuration)
            {
                float t = (currentTime - delayDuration) / lerpDuration;
                _nextStaminaBar.value = Mathf.Lerp(_nextStaminaBar.value, nextStamina, t);

                if (Mathf.Abs(_nextStaminaBar.value - nextStamina) < 0.01f)
                {
                    _nextStaminaBar.value = nextStamina;
                    currentTime = 0.0f;
                }
            }
        }
    }
}
