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
    private BaseLightController _headLightsController;

    public Light[] TailAndBrakeLights;
    [Tooltip("Target intensity of Tail lights when turned on (In Lumens). Intensity will be doubled under braking.")]
    public float[] TailAndBrakeLightsIntensity;
    private float[] _brakeLightsIntensity;
    private bool _tailAndBrakeLightsSwitch = false;
    private BaseLightController _tailAndBrakeLightsController;

    public Light[] ReverseLights;
    [Tooltip("Target intensity of reverse lights when turned on (In Lumens).")]
    public float[] ReverseLightsIntensity;
    private BaseLightController _reverseLightsController;

    protected override void Start()
    {
        base.Start();

        _headLightsController = gameObject.AddComponent<BaseLightController>();
        _headLightsController.InitialiseLights(HeadLights, false);

        _tailAndBrakeLightsController = gameObject.AddComponent<BaseLightController>();
        _tailAndBrakeLightsController.InitialiseLights(TailAndBrakeLights, false);
        _brakeLightsIntensity = new float[TailAndBrakeLightsIntensity.Length];
        for (uint lightsIndex = 0; lightsIndex < TailAndBrakeLightsIntensity.Length; lightsIndex++)
        {
            _brakeLightsIntensity[lightsIndex] = TailAndBrakeLightsIntensity[lightsIndex]*2f;
        }

        _reverseLightsController = gameObject.AddComponent<BaseLightController>();
        _reverseLightsController.InitialiseLights(ReverseLights, false);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.L))
        {
            if (_headLightsSwitch)
            {
                _headLightsController.LightCommand(0f);
                _headLightsSwitch = false;
            }
            else
            {
                _headLightsController.LightCommand(HeadLightsIntensity);
                _headLightsSwitch = true;
            }
            if (_tailAndBrakeLightsSwitch)
            {
                if (!_toggleBrakes)
                {
                    _tailAndBrakeLightsController.LightCommand(0f);
                }
                _tailAndBrakeLightsSwitch = false;
            }
            else
            {
                _tailAndBrakeLightsController.LightCommand(TailAndBrakeLightsIntensity);
                _tailAndBrakeLightsSwitch = true;
            }
        }

        if (Input.GetKeyDown(KeyCode.Space))
        {
            _tailAndBrakeLightsController.LightCommand(_brakeLightsIntensity);
        }
        if (Input.GetKeyUp(KeyCode.Space))
        {
            if (_tailAndBrakeLightsSwitch)
            {
                _tailAndBrakeLightsController.LightCommand(TailAndBrakeLightsIntensity);
            }
            else
            {
                _tailAndBrakeLightsController.LightCommand(0f);
            }
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            _reverseLightsController.LightCommand(ReverseLightsIntensity);
        }
        if (Input.GetKeyUp(KeyCode.S))
        {
            _reverseLightsController.LightCommand(0f);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
