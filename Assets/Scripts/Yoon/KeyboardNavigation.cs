using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardNavigation : MonoBehaviour
{
    public Button[] buttons; // 화살표 키로 이동할 버튼들 배열
    private int selectedIndex = 0;
    public GameObject content;

    void Start()
    {
        // 시작할 때 첫 번째 버튼을 선택
        SelectButton(selectedIndex);
    }

    void Update()
    {
        // 화살표 키 입력 감지
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            selectedIndex = Mathf.Clamp(selectedIndex - 1, 0, buttons.Length - 1);
            SelectButton(selectedIndex);

        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            selectedIndex = Mathf.Clamp(selectedIndex + 1, 0, buttons.Length - 1);
            SelectButton(selectedIndex);
        }



        // E 키 입력 감지
        if (Input.GetKeyDown(KeyCode.E))
        {
            // 선택된 버튼 클릭
            buttons[selectedIndex].onClick.Invoke();
        }
    }

    void MoveButtonList()
    {
        
    }

    void SelectButton(int index)
    {
        // EventSystem을 사용하여 버튼을 선택 상태로 만듦
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }
}
