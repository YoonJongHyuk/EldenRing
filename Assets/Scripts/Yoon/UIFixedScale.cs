using UnityEngine;

public class UIFixedScale : MonoBehaviour
{
    public Camera mainCamera;
    public float scaleFactor = 1.0f;

    void Start()
    {
        if (mainCamera == null)
        {
            mainCamera = Camera.main; // 메인 카메라가 설정되지 않았다면 메인 카메라를 가져옴
        }
    }

    void Update()
    {
        float distance = Vector3.Distance(transform.position, mainCamera.transform.position);
        transform.localScale = Vector3.one * distance * scaleFactor;
    }
}
