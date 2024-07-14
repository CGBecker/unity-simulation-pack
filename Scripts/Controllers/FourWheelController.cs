using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class FourWheelController : MonoBehaviour
{
    public ArticulationBody RootArticulationBody;
    public ArticulationBody[] WheelsMotors;
    public float motorsTorque;

    public ArticulationBody[] WheelsSuspensions;

    public ArticulationBody[] WheelsSteering;
    public float MaxSteeringForce;
    public float SteeringForce;
    private float _steeringForce;

    public ArticulationBody[] WheelsWithBrakes;
    public float brakesAndReverseTorque;



    void Start() 
    {
        for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
            {
                WheelsSteering[wheelsSteeringIndex].SetDriveForceLimit(ArticulationDriveAxis.Y, MaxSteeringForce);
            }
    }
    
    /// <summary>
    /// Calculating basic vehicle behaviour  // TODO: Add correct braking system, disconnecting from drive system
    /// </summary>
    void FixedUpdate()
    {
        if (Input.GetKey(KeyCode.W))
        {
            for (int motorsIndex = 0; motorsIndex < WheelsMotors.Length; motorsIndex++)
            {
                WheelsMotors[motorsIndex].AddRelativeTorque(new float3(1, 0, 0) * motorsTorque);
            }
        }

        if (Input.GetKey(KeyCode.S))
        {
            for (int brakesIndex = 0; brakesIndex < WheelsWithBrakes.Length; brakesIndex++)
            {
                WheelsWithBrakes[brakesIndex].AddRelativeTorque(new float3(-1, 0, 0) * brakesAndReverseTorque);
            }
        }

        if (Input.GetKey(KeyCode.A) && !Input.GetKey(KeyCode.D))
        {
            _steeringForce = -SteeringForce;
        }
        else if (Input.GetKey(KeyCode.D) && !Input.GetKey(KeyCode.A))
        {
            _steeringForce = SteeringForce;
        }
        else
        {
            _steeringForce = 0f;
        }

        if (_steeringForce != 0f)
        {
            for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
            {
                WheelsSteering[wheelsSteeringIndex].AddRelativeTorque(new float3(0, 0, _steeringForce));
            }
        }
    }
}
