using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThirdPersonCamera : MonoBehaviour
{
    public Transform target;           
    public float distance = 5.0f;       
    public float minDistance = 2.0f;      
    public float maxDistance = 10.0f;     

    public float mouseSensitivity = 3.0f; 
    public float scrollSensitivity = 2.0f; 
    public float heightOffset = 1.5f;      

    private float rotationX = 0f;
    private float rotationY = 0f;

    void Start()
    {
        Vector3 angles = transform.eulerAngles;
        rotationX = angles.y;
        rotationY = angles.x;

        // Cursor.lockState = CursorLockMode.Locked;
        // Cursor.visible = false;
    }

    void LateUpdate()
    {
        if (!target) return;
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity;
        rotationY -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        rotationY = Mathf.Clamp(rotationY, -40f, 85f);
        distance -= Input.GetAxis("Mouse ScrollWheel") * scrollSensitivity;
        distance = Mathf.Clamp(distance, minDistance, maxDistance);
        Quaternion rotation = Quaternion.Euler(rotationY, rotationX, 0);
        Vector3 offset = rotation * new Vector3(0, 0, -distance);

        Vector3 cameraPos = target.position + Vector3.up * heightOffset + offset;
        transform.position = cameraPos;
        transform.LookAt(target.position + Vector3.up * heightOffset);
    }
}
