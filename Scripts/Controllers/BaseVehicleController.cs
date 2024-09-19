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

    public ArticulationBody[] WheelsSuspensions;

    public ArticulationBody[] WheelsSteering;
    public float MaxSteeringForce;
    public float SteeringForce;
    protected float _steeringForce;

    public ArticulationBody[] WheelsWithBrakes;
    public float BrakesTorque;
    protected float[] _originalFrictions;

    public VehicleState CurrentState;

    public enum VehicleState
    {
        Idle,
        Accelerating,
        Reversing,
        Braking
    }

    /// <summary>
    /// Start() runs basic initialisation of base devices
    /// </summary>
    protected virtual void Start()
    {
        InitialiseSteering();

        InitialiseBrakes();
    }

    /// <summary>
    /// Initialises brakes by storing original friction of the articulation bodies for toggle release
    /// </summary>
    private void InitialiseBrakes()
    {
        _originalFrictions = new float[WheelsWithBrakes.Length];
        for (int brakesIndex = 0; brakesIndex < WheelsWithBrakes.Length; brakesIndex++)
        {
            _originalFrictions[brakesIndex] = WheelsWithBrakes[brakesIndex].jointFriction;
        }
    }

    /// <summary>
    /// Initialises steering by setting maximum drive force limit
    /// </summary>
    private void InitialiseSteering()
    {
        for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
        {
            WheelsSteering[wheelsSteeringIndex].SetDriveForceLimit(ArticulationDriveAxis.Y, MaxSteeringForce);
        }
    }

    /// <summary>
    /// Keyboard input read (must be done in Update due to Unity's limitation)
    /// </summary>
    protected virtual void Update()
    {
        if (Input.GetKey(KeyCode.W) && !Input.GetKey(KeyCode.S))
        {
            CurrentState = VehicleState.Accelerating;
        }
        else if (Input.GetKey(KeyCode.S) && !Input.GetKey(KeyCode.W))
        {
            CurrentState = VehicleState.Reversing;
        }
        else if (Input.GetKey(KeyCode.Space))
        {
            CurrentState = VehicleState.Braking;
        }
        else
        {
            CurrentState = VehicleState.Idle;
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
        if (CurrentState == VehicleState.Accelerating)
        {
            ThrottleCommand(WheelsMotors, ForwardTorque);
        }
        if (CurrentState == VehicleState.Reversing)
        {
            ThrottleCommand(WheelsMotors, ReverseTorque*-1);
        }
        if (CurrentState == VehicleState.Braking)
        {
            BrakesCommand(WheelsWithBrakes, BrakesTorque);
        }
        else
        {
            BrakesCommand(WheelsWithBrakes, _originalFrictions);                    
        }

        if (_steeringForce != 0f)
        {
            for (int wheelsSteeringIndex = 0; wheelsSteeringIndex < WheelsSteering.Length; wheelsSteeringIndex++)
            {
                WheelsSteering[wheelsSteeringIndex].AddRelativeTorque(new float3(0, 0, _steeringForce));
            }
        }
    }

    /// <summary>
    /// Accelerator command for all motors with single global torque
    /// </summary>
    /// <param name="motors">Articulation bodies being used as motors/actuators</param>
    /// <param name="torque">Torque input (global)</param>
    public void ThrottleCommand(ArticulationBody[] motors, float torque)
    {
        for (int motorsIndex = 0; motorsIndex < motors.Length; motorsIndex++)
        {
            motors[motorsIndex].AddRelativeTorque(new float3(1, 0, 0) * torque);
        }
    }

    /// <summary>
    /// Accelerator command for each motor with individual torque inputs
    /// </summary>
    /// <param name="motors">Articulation bodies being used as motors/actuators</param>
    /// <param name="torque">Torque input (individual for each motor per index)</param>
    public void ThrottleCommand(ArticulationBody[] motors, float[] torque)
    {
        for (int motorsIndex = 0; motorsIndex < motors.Length; motorsIndex++)
        {
            motors[motorsIndex].AddRelativeTorque(new float3(1, 0, 0) * torque[motorsIndex]);
        }
    }

    /// <summary>
    /// Brakes command with global strength for all articulation bodies with brakes
    /// </summary>
    /// <param name="brakes">Articulation bodies to be used as brakes</param>
    /// <param name="strength">Strength of brakes as friction</param>
    public void BrakesCommand(ArticulationBody[] brakes, float strength)
    {
        for (int brakesIndex = 0; brakesIndex < brakes.Length; brakesIndex++)
        {
            brakes[brakesIndex].jointFriction = strength;
        }
    }

    /// <summary>
    /// Brakes command with individual strengths per articulation body
    /// </summary>
    /// <param name="brakes">Articulation bodies to be used as brakes</param>
    /// <param name="strengths">Strength of brakes as friction</param>
    public void BrakesCommand(ArticulationBody[] brakes, float[] strengths)
    {
        for (int brakesIndex = 0; brakesIndex < brakes.Length; brakesIndex++)
        {
            brakes[brakesIndex].jointFriction = strengths[brakesIndex];
        }
    }
}
