using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class MultiWheelVehicleController : BaseVehicleController
{
    public Light[] HeadLights;
    [Tooltip("Target intensity of headlights when turned on (In Lumens).")]
    public float[] HeadLightsIntensity;
    private bool _headLightsSwitch = false;
    private BaseLightActuator _headLightsController;

    public Light[] TailAndBrakeLights;
    [Tooltip("Target intensity of Tail lights when turned on (In Lumens). Intensity will be doubled under braking.")]
    public float[] TailAndBrakeLightsIntensity;
    private float[] _brakeLightsIntensity;
    private bool _tailAndBrakeLightsSwitch = false;
    private BaseLightActuator _tailAndBrakeLightsController;

    public Light[] ReverseLights;
    [Tooltip("Target intensity of reverse lights when turned on (In Lumens).")]
    public float[] ReverseLightsIntensity;
    private BaseLightActuator _reverseLightsController;

    protected override void Start()
    {
        base.Start();

        _headLightsController = gameObject.AddComponent<BaseLightActuator>();
        _headLightsController.InitialiseLights(HeadLights, false);

        _tailAndBrakeLightsController = gameObject.AddComponent<BaseLightActuator>();
        _tailAndBrakeLightsController.InitialiseLights(TailAndBrakeLights, false);
        _brakeLightsIntensity = new float[TailAndBrakeLightsIntensity.Length];
        for (uint lightsIndex = 0; lightsIndex < TailAndBrakeLightsIntensity.Length; lightsIndex++)
        {
            _brakeLightsIntensity[lightsIndex] = TailAndBrakeLightsIntensity[lightsIndex]*2f;
        }

        _reverseLightsController = gameObject.AddComponent<BaseLightActuator>();
        _reverseLightsController.InitialiseLights(ReverseLights, false);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_headLightsSwitch)
            {
                _headLightsController.Command(0f);
                _headLightsSwitch = false;
            }
            else
            {
                _headLightsController.Command(HeadLightsIntensity);
                _headLightsSwitch = true;
            }
            if (_tailAndBrakeLightsSwitch)
            {
                if (CurrentState != VehicleState.Braking)
                {
                    _tailAndBrakeLightsController.Command(0f);
                }
                _tailAndBrakeLightsSwitch = false;
            }
            else
            {
                _tailAndBrakeLightsController.Command(TailAndBrakeLightsIntensity);
                _tailAndBrakeLightsSwitch = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _tailAndBrakeLightsController.Command(_brakeLightsIntensity);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_tailAndBrakeLightsSwitch)
            {
                _tailAndBrakeLightsController.Command(TailAndBrakeLightsIntensity);
            }
            else
            {
                _tailAndBrakeLightsController.Command(0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _reverseLightsController.Command(ReverseLightsIntensity);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _reverseLightsController.Command(0f);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
