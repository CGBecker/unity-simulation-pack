using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public Transform TargetObject;  // Target to orbit around
    private Transform _camera;  // Camera transform reference
    public float mouseSensitivity = 2f;  // Expose mouse sensitivity to Inspector
    public float rotationSmoothing = 5f;  // Smoothing factor for rotation when not controlling
    private float _localRotation;  // Local rotation angle
    private float _rotationSpeed = 0f;  // Store the current rotation speed

    void Awake()
    {
        _camera = this.transform;
    }

    void Update()
    {
        HandleCameraRotation();
    }

    /// <summary>
    /// Handles the rotation of the camera around the target object.
    /// </summary>
    void HandleCameraRotation()
    {
        if (Input.GetMouseButton(1))  // If right mouse button is held
        {
            // Update the rotation based on mouse input
            float mouseX = Input.GetAxis("Mouse X");
            _rotationSpeed = mouseX * mouseSensitivity;
        }
        else
        {
            // Gradually reduce rotation speed to zero when the mouse isn't pressed
            _rotationSpeed = Mathf.Lerp(_rotationSpeed, 0f, Time.deltaTime * rotationSmoothing);
        }

        // Apply the rotation around the target object
        _localRotation += _rotationSpeed;
        _localRotation = Mathf.Clamp(_localRotation, -360f, 360f);  // Optionally clamp rotation

        // Rotate the camera around the target using Quaternion for smoothness
        _camera.RotateAround(TargetObject.position, Vector3.up, _rotationSpeed * Time.deltaTime * 100f);
    }
}