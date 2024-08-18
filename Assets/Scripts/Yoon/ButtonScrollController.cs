using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ButtonScrollController : MonoBehaviour
{
    public RectTransform content;
    public RectTransform viewport;
    public Button[] buttons;
    private int selectedIndex = 0;
    private float contentYMin = 0f;
    private float contentYMax = 90f;
    private float step = 30f;


    void Awake()
    {
        SelectButton(selectedIndex);
    }

    void Update()
    {
        // 화살표 위 키 입력
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // 선택된 인덱스가 0보다 크고, 선택된 버튼이 뷰포트 내에서 가장 위에 있는 버튼이 아닐 경우
            if (selectedIndex > 0 && !IsTopVisibleButton(selectedIndex))
            {
                selectedIndex -= 1;
                SelectButton(selectedIndex);
            }
            else if (IsButtonVisible(buttons[selectedIndex]) && content.anchoredPosition.y > contentYMin)
            {
                // 가장 위에 있는 버튼이 선택되었을 경우 스크롤만 이동
                content.anchoredPosition += new Vector2(0, -step);
                SelectTopVisibleButton();
            }
        }

        // 화살표 아래 키 입력
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // 선택된 인덱스가 배열 길이보다 작고, 선택된 버튼이 뷰포트 내에서 가장 아래에 있는 버튼이 아닐 경우
            if (selectedIndex < buttons.Length - 1 && !IsBottomVisibleButton(selectedIndex))
            {
                selectedIndex += 1;
                SelectButton(selectedIndex);
            }
            else if (IsButtonVisible(buttons[selectedIndex]) && content.anchoredPosition.y < contentYMax)
            {
                // 가장 아래에 있는 버튼이 선택되었을 경우 스크롤만 이동
                content.anchoredPosition += new Vector2(0, step);
                SelectBottomVisibleButton();
            }
        }

        // E 키 입력으로 현재 선택된 버튼 클릭
        if (Input.GetKeyDown(KeyCode.E))
        {
            buttons[selectedIndex].onClick.Invoke();
        }
    }

    

    void SelectButton(int index)
    {
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }

    void SelectTopVisibleButton()
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (IsButtonVisible(buttons[i]))
            {
                selectedIndex = i;
                SelectButton(selectedIndex);
                break;
            }
        }
    }

    void SelectBottomVisibleButton()
    {
        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            if (IsButtonVisible(buttons[i]))
            {
                selectedIndex = i;
                SelectButton(selectedIndex);
                break;
            }
        }
    }

    bool IsButtonVisible(Button button)
    {
        RectTransform buttonRect = button.GetComponent<RectTransform>();
        Vector3[] buttonCorners = new Vector3[4];
        buttonRect.GetWorldCorners(buttonCorners);

        Vector3[] viewportCorners = new Vector3[4];
        viewport.GetWorldCorners(viewportCorners);

        return buttonCorners[0].y < viewportCorners[1].y && buttonCorners[1].y > viewportCorners[0].y;
    }

    // 현재 인덱스의 버튼이 뷰포트 내에서 가장 위에 있는 버튼인지 확인
    bool IsTopVisibleButton(int index)
    {
        for (int i = 0; i < buttons.Length; i++)
        {
            if (IsButtonVisible(buttons[i]))
            {
                return i == index;
            }
        }
        return false;
    }

    // 현재 인덱스의 버튼이 뷰포트 내에서 가장 아래에 있는 버튼인지 확인
    bool IsBottomVisibleButton(int index)
    {
        for (int i = buttons.Length - 1; i >= 0; i--)
        {
            if (IsButtonVisible(buttons[i]))
            {
                return i == index;
            }
        }
        return false;
    }
}
