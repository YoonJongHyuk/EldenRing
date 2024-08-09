using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class KeyboardNavigation : MonoBehaviour
{
    public Button[] buttons; // ȭ��ǥ Ű�� �̵��� ��ư�� �迭
    private int selectedIndex = 0;
    public GameObject content;

    void Start()
    {
        // ������ �� ù ��° ��ư�� ����
        SelectButton(selectedIndex);
    }

    void Update()
    {
        // ȭ��ǥ Ű �Է� ����
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



        // E Ű �Է� ����
        if (Input.GetKeyDown(KeyCode.E))
        {
            // ���õ� ��ư Ŭ��
            buttons[selectedIndex].onClick.Invoke();
        }
    }

    void MoveButtonList()
    {
        
    }

    void SelectButton(int index)
    {
        // EventSystem�� ����Ͽ� ��ư�� ���� ���·� ����
        EventSystem.current.SetSelectedGameObject(buttons[index].gameObject);
    }
}
