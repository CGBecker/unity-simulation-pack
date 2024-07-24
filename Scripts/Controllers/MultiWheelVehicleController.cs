using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiWheelVehicleController : BaseVehicleController
{
    public Light[] HeadLights;
    [Tooltip("Target intensity of headlights when turned on (In Lumens).")]
    public float HeadLightsIntensity;
    private bool _headLightsSwitch = false;
    private BaseLightController _headLightsController;

    public Light[] TailAndBrakeLights;
    [Tooltip("Target intensity of Tail lights when turned on (In Lumens). Intensity will be doubled under braking.")]
    public float TailAndBrakeLightsIntensity;
    private bool _tailAndBrakeLightsSwitch = false;
    private BaseLightController _tailAndBrakeLightsController;

    public Light[] ReverseLights;
    [Tooltip("Target intensity of reverse lights when turned on (In Lumens).")]
    public float ReverseLightsIntensity;
    private BaseLightController _reverseLightsController;

    protected override void Start()
    {
        base.Start();

        _headLightsController = gameObject.AddComponent<BaseLightController>();
        _headLightsController.LightsTargetIntensityOn = HeadLightsIntensity;
        _headLightsController.InitialiseLights(HeadLights);

        _tailAndBrakeLightsController = gameObject.AddComponent<BaseLightController>();
        _tailAndBrakeLightsController.LightsTargetIntensityOn = TailAndBrakeLightsIntensity;
        _tailAndBrakeLightsController.InitialiseLights(TailAndBrakeLights);

        _reverseLightsController = gameObject.AddComponent<BaseLightController>();
        _reverseLightsController.LightsTargetIntensityOn = ReverseLightsIntensity;
        _reverseLightsController.InitialiseLights(ReverseLights);
    }

    protected override void Update()
    {
        base.Update();

        if (Input.GetKeyDown(KeyCode.L))
        {
            Debug.Log("Pressed L");
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
                _tailAndBrakeLightsController.LightCommand(0f);
                _tailAndBrakeLightsSwitch = false;
            }
            else
            {
                _tailAndBrakeLightsController.LightCommand(TailAndBrakeLightsIntensity);
                _tailAndBrakeLightsSwitch = true;
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
