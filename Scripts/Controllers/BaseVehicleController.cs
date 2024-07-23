using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;

public class BaseVehicleController : MonoBehaviour
{
    public ArticulationBody RootArticulationBody;
    public ArticulationBody[] WheelsMotors;
    public float ForwardTorque;
    public float ReverseTorque;
    protected bool _toggleThrottle = false;

    public ArticulationBody[] WheelsSuspensions;

    public ArticulationBody[] WheelsSteering;
    public float MaxSteeringForce;
    public float SteeringForce;
    protected float _steeringForce;

    public ArticulationBody[] WheelsWithBrakes;
    public float BrakesTorque;
    protected float[] _originalFrictions;
    protected bool _toggleBrakesOrReverse = false;

    protected virtual void Start()
    {
        InitialiseSteering();

        InitialiseBrakes();
    }

    private void InitialiseBrakes()
    {
        _originalFrictions = new float[WheelsWithBrakes.Length];
        for (int brakesIndex = 0; brakesIndex < WheelsWithBrakes.Length; brakesIndex++)
        {
            _originalFrictions[brakesIndex] = WheelsWithBrakes[brakesIndex].jointFriction;
        }
    }

    private void InitialiseSteering()
    {
        for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
        {
            WheelsSteering[wheelsSteeringIndex].SetDriveForceLimit(ArticulationDriveAxis.Y, MaxSteeringForce);
        }
    }

    protected virtual void Update()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            _toggleThrottle = true;
        }
        else
        {
            _toggleThrottle = false;
        }

        if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            _toggleBrakesOrReverse = true;
        }
        else
        {
            _toggleBrakesOrReverse = false;
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

    }

    /// <summary>
    /// Calculating base vehicle behaviour
    /// </summary>
    protected virtual void FixedUpdate()
    {
        if (_toggleThrottle)
        {
            ThrottleCommand(WheelsMotors, ForwardTorque);
        }

        Debug.Log(WheelsWithBrakes[0].velocity);
        if (_toggleBrakesOrReverse)  // TODO: Improve braking system with smarter call to brakes command
        {
            for (int brakesIndex = 0; brakesIndex < WheelsWithBrakes.Length; brakesIndex++)
            {
                if (WheelsWithBrakes[brakesIndex].velocity.z > 0f)
                {
                    BrakesCommand(WheelsWithBrakes, BrakesTorque);
                }
                else
                {
                    BrakesCommand(WheelsWithBrakes, _originalFrictions);
                    ThrottleCommand(WheelsMotors, ReverseTorque*-1);
                }
            }
        }

        if (_steeringForce != 0f)
        {
            for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
            {
                WheelsSteering[wheelsSteeringIndex].AddRelativeTorque(new float3(0, 0, _steeringForce));
            }
        }
    }

    public void ThrottleCommand(ArticulationBody[] motors, float torque)
    {
        for (int motorsIndex = 0; motorsIndex < motors.Length; motorsIndex++)
        {
            motors[motorsIndex].AddRelativeTorque(new float3(1, 0, 0) * torque);
        }
    }

    public void BrakesCommand(ArticulationBody[] brakes, float strength)
    {
        for (int brakesIndex = 0; brakesIndex < brakes.Length; brakesIndex++)
        {
            brakes[brakesIndex].jointFriction = strength;
        }
    }

    public void BrakesCommand(ArticulationBody[] brakes, float[] strengths)
    {
        for (int brakesIndex = 0; brakesIndex < brakes.Length; brakesIndex++)
        {
            brakes[brakesIndex].jointFriction = strengths[brakesIndex];
        }
    }
}
