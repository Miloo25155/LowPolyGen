using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform target;

    public float mouseSensitivity = 3.5f;
    public float distFromTarget = 2;
    public Vector2 pitchMinMax = new Vector2(-40, 85);

    float pitch, yaw;

    public float rotationSmoothTime = 1.2f;
    Vector3 rotationSmoothVelocity;
    Vector3 currentRotation;

    void LateUpdate()
    {
        yaw += Input.GetAxis("Mouse X") * mouseSensitivity;

        pitch -= Input.GetAxis("Mouse Y") * mouseSensitivity;
        pitch = Mathf.Clamp(pitch, pitchMinMax.x, pitchMinMax.y);

        Vector3 targetRotation = new Vector3(pitch, yaw);
        currentRotation = Vector3.SmoothDamp(currentRotation, targetRotation, ref rotationSmoothVelocity, rotationSmoothTime);
        transform.localEulerAngles = currentRotation;

        transform.position = target.position - transform.forward * distFromTarget;
    }
}
