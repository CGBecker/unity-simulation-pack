using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MultiWheelVehicleController : BaseVehicleController
{
    public Light[] HeadLights;
    [Tooltip("Target intensity of headlights when turned on (In Lumens).")]
    public float HeadLightsIntensity;
    private BaseLightController _headLightsController;

    public Light[] TailAndBrakeLights;
    [Tooltip("Target intensity of Tail lights when turned on (In Lumens). Intensity will be doubled under braking.")]
    public float TailAndBrakeLightsIntensity;
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
            _headLightsController.LightCommand(HeadLightsIntensity);
            _tailAndBrakeLightsController.LightCommand(TailAndBrakeLightsIntensity);
        }
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }
}
