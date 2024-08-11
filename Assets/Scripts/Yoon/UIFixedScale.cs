using UnityEngine;

public class UIFixedScale : MonoBehaviour
{
    public Camera mainCamera;
    public float scaleFactor = 1.0f;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // ���� ī�޶� �������� �ʾҴٸ� ���� ī�޶� ������
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        transform.localScale = Vector3.one * distance * scaleFactor;
    }
}
