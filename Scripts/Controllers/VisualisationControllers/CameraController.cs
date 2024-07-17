using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using Unity.Burst;

public class CameraController : MonoBehaviour
{
    public Transform TargetObject;
    private Transform _camera;
    private float _mouseSensitivity = 2f;
    private float _localRotation;
    void Awake()
    {
        _camera = this.transform;
    }

    /// <summary>
    /// Camera controller behaviour
    /// </summary>
    [BurstCompile]
    void FixedUpdate() 
    {
        if (Input.GetMouseButton(1))
        {
            _localRotation += Input.GetAxis("Mouse X") * _mouseSensitivity;
            _camera.RotateAround(new float3(TargetObject.position.x, _camera.position.y, TargetObject.position.z), 
                                    new float3(0, 1f, 0), Time.deltaTime * _localRotation);
        }
        else
        {
            if(_localRotation >= 1f)
            {
                _localRotation -= 1f;
            }
            else if(_localRotation <= -1f)
            {
                _localRotation += 1f;
            }
            else
            {
                _localRotation = 0f;
            }
            _camera.RotateAround(new float3(TargetObject.position.x, _camera.position.y, TargetObject.position.z), 
                                    new float3(0, 1f, 0), Time.deltaTime * _localRotation);
        }
    }
}
