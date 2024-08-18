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
        // ȭ��ǥ �� Ű �Է�
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            // ���õ� �ε����� 0���� ũ��, ���õ� ��ư�� ����Ʈ ������ ���� ���� �ִ� ��ư�� �ƴ� ���
            if (selectedIndex > 0 && !IsTopVisibleButton(selectedIndex))
            {
                selectedIndex -= 1;
                SelectButton(selectedIndex);
            }
            else if (IsButtonVisible(buttons[selectedIndex]) && content.anchoredPosition.y > contentYMin)
            {
                // ���� ���� �ִ� ��ư�� ���õǾ��� ��� ��ũ�Ѹ� �̵�
                content.anchoredPosition += new Vector2(0, -step);
                SelectTopVisibleButton();
            }
        }

        // ȭ��ǥ �Ʒ� Ű �Է�
        if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            // ���õ� �ε����� �迭 ���̺��� �۰�, ���õ� ��ư�� ����Ʈ ������ ���� �Ʒ��� �ִ� ��ư�� �ƴ� ���
            if (selectedIndex < buttons.Length - 1 && !IsBottomVisibleButton(selectedIndex))
            {
                selectedIndex += 1;
                SelectButton(selectedIndex);
            }
            else if (IsButtonVisible(buttons[selectedIndex]) && content.anchoredPosition.y < contentYMax)
            {
                // ���� �Ʒ��� �ִ� ��ư�� ���õǾ��� ��� ��ũ�Ѹ� �̵�
                content.anchoredPosition += new Vector2(0, step);
                SelectBottomVisibleButton();
            }
        }

        // E Ű �Է����� ���� ���õ� ��ư Ŭ��
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

    // ���� �ε����� ��ư�� ����Ʈ ������ ���� ���� �ִ� ��ư���� Ȯ��
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

    // ���� �ε����� ��ư�� ����Ʈ ������ ���� �Ʒ��� �ִ� ��ư���� Ȯ��
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
