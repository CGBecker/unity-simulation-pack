using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;

public class BaseVehicleController : MonoBehaviour
{
    public ArticulationBody RootArticulationBody;
    public ArticulationBody[] WheelsMotors;
    public float ForwardTorque;
    public float ReverseTorque;
    private bool _toggleThrottle = false;

    public ArticulationBody[] WheelsSuspensions;

    public ArticulationBody[] WheelsSteering;
    public float MaxSteeringForce;
    public float SteeringForce;
    private float _steeringForce;

    public ArticulationBody[] WheelsWithBrakes;
    public float BrakesTorque;
    private float[] _originalFrictions;
    private bool _toggleBrakesOrReverse = false;

    public Light[] HeadLights;
    public float HeadLightsTargetIntensityOn;
    private float _previousHeadLightsIntensity;
    private float _headLightsTargetIntensity;
    public Light[] TailAndBrakeLights;
    public float TailAndBrakeLightsTargetIntensityOn;
    private float _previousTailAndBrakeLightsIntensity;
    private float _tailAndBrakeLightsIntensity;
    public Light[] ReverseLights;
    public float ReverseLightsTargetIntensityOn;
    private float _previousReverseLightsIntensity;
    private float _reverseLightsIntensity;
    private int _lightsProgressionDelta = 20;
    private bool _lightSwitch = false;




    void Start()
    {
        InitialiseSteering();

        InitialiseBrakes();

        InitialiseLights(HeadLights);

        InitialiseLights(TailAndBrakeLights);

        InitialiseLights(ReverseLights);
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

    private void InitialiseLights(Light[] lights)
    {
        for (int lightsIndex = 0; lightsIndex < lights.Length; lightsIndex++)
        {
            lights[lightsIndex].intensity = 0f;
            lights[lightsIndex].enabled = false;
        }
    }

    void Update()
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

        if (Input.GetKeyDown(KeyCode.L))
        {
            _previousHeadLightsIntensity = _previousHeadLightsIntensity == HeadLightsTargetIntensityOn ? 0f : HeadLightsTargetIntensityOn;
            _previousReverseLightsIntensity = _previousReverseLightsIntensity == ReverseLightsTargetIntensityOn ? 0f : ReverseLightsTargetIntensityOn;
            _previousTailAndBrakeLightsIntensity = _previousTailAndBrakeLightsIntensity == TailAndBrakeLightsTargetIntensityOn ? 0f : TailAndBrakeLightsTargetIntensityOn;
           _lightSwitch = !_lightSwitch;
        }

    }

    /// <summary>
    /// Calculating base vehicle behaviour
    /// </summary>
    void FixedUpdate()
    {
        if (_toggleThrottle)
        {
            ThrottleCommand(WheelsMotors, ForwardTorque);
        }

        if (_toggleBrakesOrReverse)
        {
            for (int brakesIndex = 0; brakesIndex < WheelsWithBrakes.Length; brakesIndex++)
            {
                if (WheelsWithBrakes[brakesIndex].angularVelocity.x > 0f && WheelsWithBrakes[brakesIndex].angularVelocity.y > 0f && WheelsWithBrakes[brakesIndex].angularVelocity.z > 0f)
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

        if (_lightSwitch)
        {
            _headLightsTargetIntensity = HeadLightsTargetIntensityOn;
            _tailAndBrakeLightsIntensity = TailAndBrakeLightsTargetIntensityOn;
            _reverseLightsIntensity = ReverseLightsTargetIntensityOn;
        }
        else
        {
            _headLightsTargetIntensity = 0f;
            _tailAndBrakeLightsIntensity = 0f;
            _reverseLightsIntensity = 0f;
        }

        LightProgressiveControl(HeadLights, _headLightsTargetIntensity, _previousHeadLightsIntensity);

        LightProgressiveControl(TailAndBrakeLights, _tailAndBrakeLightsIntensity, _previousTailAndBrakeLightsIntensity);

        LightProgressiveControl(ReverseLights, _reverseLightsIntensity, _previousReverseLightsIntensity);
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

    private void LightProgressiveControl(Light[] lights, float targetIntesity, float previousIntesity)
    {
        for (int lightIndex = 0; lightIndex < lights.Length; lightIndex++)
        {
            if (lights[lightIndex].intensity > 1f && !lights[lightIndex].enabled)
            {
                lights[lightIndex].enabled = true;
            }
            else if (lights[lightIndex].intensity <= 1f && lights[lightIndex].enabled)
            {
                lights[lightIndex].enabled = false;
            }
            if (lights[lightIndex].enabled && lights[lightIndex].intensity != targetIntesity)
            {
                lights[lightIndex].intensity -= (previousIntesity-targetIntesity)/_lightsProgressionDelta;
            }
        }
    }
}
