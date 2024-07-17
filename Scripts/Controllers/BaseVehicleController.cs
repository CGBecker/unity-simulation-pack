using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using System.Linq;
using UnityEngine.Rendering.HighDefinition;

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

    public HDAdditionalLightData[] HeadLights;
    public float HeadLightsTargetIntensityOn;
    private float _previousHeadLightsIntensity;
    private float _headLightsTargetIntensity;
    public HDAdditionalLightData[] TailAndBrakeLights;
    public float TailAndBrakeLightsTargetIntensityOn;
    private float _previousTailAndBrakeLightsIntensity;
    private float _tailAndBrakeLightsTargetIntensity;
    public HDAdditionalLightData[] ReverseLights;
    public float ReverseLightsTargetIntensityOn;
    private float _previousReverseLightsIntensity;
    private float _reverseLightsTargetIntensity;
    private int _lightsProgressionDelta = 40;
    private bool _lightSwitch;




    void Start()
    {
        InitialiseSteering();

        InitialiseBrakes();

        _previousHeadLightsIntensity = HeadLights[0].intensity;
        _headLightsTargetIntensity = 0f;
        _previousReverseLightsIntensity = ReverseLights[0].intensity;
        _tailAndBrakeLightsTargetIntensity = 0f;
        _previousTailAndBrakeLightsIntensity = TailAndBrakeLights[0].intensity;
        _reverseLightsTargetIntensity = 0f;
        
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

    private void InitialiseLights(HDAdditionalLightData[] lights)
    {
        for (int lightsIndex = 0; lightsIndex < lights.Length; lightsIndex++)
        {
            lights[lightsIndex].lightUnit = LightUnit.Lumen;
            lights[lightsIndex].intensity = 0f;
            lights[lightsIndex].gameObject.SetActive(false);
            _lightSwitch = false;
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
            _previousHeadLightsIntensity = _previousHeadLightsIntensity == 0f ? HeadLights[0].intensity : 0f;
            _previousReverseLightsIntensity = _previousReverseLightsIntensity == 0f ? ReverseLights[0].intensity : 0f;
            _previousTailAndBrakeLightsIntensity = _previousTailAndBrakeLightsIntensity == 0f ? TailAndBrakeLights[0].intensity : 0f;
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

        if (_toggleBrakesOrReverse)  // TODO: Improve braking system with smarter call to brakes command
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
            _tailAndBrakeLightsTargetIntensity = TailAndBrakeLightsTargetIntensityOn;
            _reverseLightsTargetIntensity = ReverseLightsTargetIntensityOn;
        }
        else
        {
            _headLightsTargetIntensity = 0f;
            _tailAndBrakeLightsTargetIntensity = 0f;
            _reverseLightsTargetIntensity = 0f;
        }

        LightProgressiveControl(HeadLights, _headLightsTargetIntensity, _previousHeadLightsIntensity);  // TODO: Call to light progressive control too frequent, consider AsyncTask

        LightProgressiveControl(TailAndBrakeLights, _tailAndBrakeLightsTargetIntensity, _previousTailAndBrakeLightsIntensity);

        LightProgressiveControl(ReverseLights, _reverseLightsTargetIntensity, _previousReverseLightsIntensity);
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

    private void LightProgressiveControl(HDAdditionalLightData[] lights, float targetIntesity, float previousIntesity)  // TODO: Consider improvement or AsyncTask
    {
        for (int lightsIndex = 0; lightsIndex < lights.Length; lightsIndex++)
        {
            if (lights[lightsIndex].intensity > 1f && !lights[lightsIndex].gameObject.activeSelf)
            {
                lights[lightsIndex].gameObject.SetActive(true);
            }
            else if (lights[lightsIndex].intensity <= 1f && lights[lightsIndex].enabled)
            {
                lights[lightsIndex].gameObject.SetActive(false);
            }
            if (lights[lightsIndex].intensity != targetIntesity)
            {
                lights[lightsIndex].intensity -= (previousIntesity-targetIntesity)/_lightsProgressionDelta;
            }
        }
    }
}
