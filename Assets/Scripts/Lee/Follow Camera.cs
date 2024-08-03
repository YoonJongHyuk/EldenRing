using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering;

public class FollowCamera: MonoBehaviour
{
    public Transform target;
    public float followSpeed = 10.0f;
    public float Sensitivity = 100.0f;
    public float ClampAngle = 70.0f;

    float rotX;
    float rotY;

    public Transform realCamera;
    public Vector3 dirNormalized;
    public Vector3 finalDir;
    public float minDistance;
    public float maxDistance;
    public float finalDistance = 10f;
    public float smoothness = 10f;
    private void Start()
    {
        rotX = transform.localRotation.eulerAngles.x;
        rotY = transform.localRotation.eulerAngles.y;

        dirNormalized = realCamera.localPosition.normalized;
        finalDistance = realCamera.localPosition.magnitude;

       // Cursor.lockState = CursorLockMode.Locked;
      //  Cursor.visible = false;
    }
    private void Update()
    {
        rotX += Input.GetAxis("Mouse X") * Sensitivity * Time.deltaTime;
        rotY += Input.GetAxis("Mouse Y") * Sensitivity * Time.deltaTime;

        rotX = Mathf.Clamp(rotX, -ClampAngle, ClampAngle);
        Quaternion rot = Quaternion.Euler(rotY , rotX, 0);
        transform.rotation = rot;
    }
    private void LateUpdate()
    {
        transform.position = Vector3.MoveTowards(transform.position, target.position, followSpeed * Time.deltaTime);

        finalDir = transform.TransformPoint(dirNormalized * maxDistance);

        RaycastHit hit;

        if(Physics.Linecast(transform.position, finalDir, out hit))
        {
            finalDistance = Mathf.Clamp(hit.distance, minDistance, maxDistance);
            
        }
        else
        {
            finalDistance = maxDistance;
        }
        realCamera.localPosition = Vector3.Lerp(realCamera.localPosition, dirNormalized * finalDistance, Time.deltaTime * smoothness);
    }
}
