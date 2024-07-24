using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.Burst;
using System.Linq;
using System;
using System.Threading;

/// <summary>
/// Base light controller class for generic light control
/// </summary>
public class BaseLightController : MonoBehaviour
{
    /// <summary>
    /// Lights to be controlled
    /// OBS.: HDRP only due to use of HDAdditionalLightData and Lumens
    /// </summary>
    public Light[] Lights;  // Is this needed if we are going to spawn and configure at runtime?
    private HDAdditionalLightData[] _lightsData;
    public float LightsTargetIntensityOn;
    private float _previousLightsIntensity;
    private uint _lightsProgressionDelta = 40;  // Default set to 40. Higher value equals slower transition
    private Task[] _tasks;
    private CancellationTokenSource[] _sources;
    private CancellationToken[] _tokens;

    /// <summary>
    ///  Initialise lights with given config
    /// </summary>
    /// <param name="lights"></param>
    public void InitialiseLights(Light[] lights)
    {
        Debug.Log("Init Lights");
        if (Lights == null)
        {
            Lights = lights;
        }
        _lightsData = new HDAdditionalLightData[Lights.Length];
        for (int lightsIndex = 0; lightsIndex < lights.Length; lightsIndex++)
        {
            _lightsData[lightsIndex] = Lights[lightsIndex].GetComponent<HDAdditionalLightData>();
            _lightsData[lightsIndex].lightUnit = LightUnit.Lumen;
            _lightsData[lightsIndex].intensity = 0f;
            _lightsData[lightsIndex].gameObject.SetActive(false);
        }
        _tasks = new Task[lights.Length];
        _sources = new CancellationTokenSource[lights.Length];
        _tokens = new CancellationToken[lights.Length];
    }

    /// <summary>
    /// Initialise lights with given config and delta for transition speed (Higher value equals slower transition)
    /// </summary>
    /// <param name="lights"></param>
    /// <param name="lightProgresssiveShiftDelta"></param>
    public void InitialiseLights(Light[] lights, uint lightProgresssiveShiftDelta)
    {
        Debug.Log("Init Lights with Delta");
        _lightsProgressionDelta = lightProgresssiveShiftDelta;
        InitialiseLights(lights);
    }

    /// <summary>
    /// Command method for setting light intensity target for all lights
    /// </summary>
    /// <param name="intensityInLumens"></param>
    public void LightCommand(float intensityInLumens)
    {
        Debug.Log("Light Command");

        _previousLightsIntensity = _lightsData[0].intensity;
        for (int lightsIndex = 0; lightsIndex < _lightsData.Length; lightsIndex++)
        {
            if (_tasks[lightsIndex] != null)
            {
                if(_tasks[lightsIndex].Status.Equals(TaskStatus.Running))
                {
                    _sources[lightsIndex].Cancel();
                    Debug.LogWarning("Cancelling light task " + lightsIndex);
                }
            }
            _sources[lightsIndex] = new CancellationTokenSource();
            _tokens[lightsIndex] = _sources[lightsIndex].Token;
            _tasks[lightsIndex] = Task.Factory.StartNew(() => ProgressiveLightControl.ProgressiveLightControlTask(_lightsData[lightsIndex], intensityInLumens, _previousLightsIntensity, _lightsProgressionDelta), _tokens[lightsIndex]);
        }
    }

    /// <summary>
    /// Command method for setting light intensity target for one light
    /// </summary>
    /// <param name="lightData"></param>
    /// <param name="intensityInLumens"></param>
    public void LightCommand(HDAdditionalLightData lightData, float intensityInLumens)
    {
        Debug.Log("Light command individual light");
        _previousLightsIntensity = lightData.intensity;  // Consider array of cached previous intensities
        if(_lightsData.Contains(lightData))
        {
            Debug.Log("LightData exists in array of lightDatas");
            int index = Array.FindIndex(_lightsData, row => _lightsData.Contains(lightData));
            if (_tasks[index] != null)
            {
                if(_tasks[index].Status.Equals(TaskStatus.Running))
                {
                    // _tasks[index].Wait();
                    _tasks[index].Dispose();
                }
            }
            Debug.Log("Run task for individual light");
            _tasks[index] = ProgressiveLightControl.ProgressiveLightControlTask(_lightsData[index], intensityInLumens, _previousLightsIntensity, _lightsProgressionDelta);
        }
    }

}

/// <summary>
/// Light control static class for progressive behaviour of light device
/// </summary>
public static class ProgressiveLightControl
{
    /// <summary>
    /// Light control Task to progressively reach target light intensity from any non-negative value
    /// </summary>
    /// <param name="lightData"></param>
    /// <param name="intensityInLumens"></param>
    /// <param name="previousIntesityInLumens"></param>
    /// <param name="lightProgressionDelta"></param>
    /// <returns></returns>
    [BurstCompile]
    public static async Task ProgressiveLightControlTask(HDAdditionalLightData lightData, float intensityInLumens, float previousIntesityInLumens, uint lightProgressionDelta)
    {
        Debug.Log("Task started");
        for (uint elapsedDelta = lightProgressionDelta; elapsedDelta > 0; --elapsedDelta)
        {
            Debug.Log("Task loop " + previousIntesityInLumens  + "-" + intensityInLumens+")/" + lightProgressionDelta + "-" + lightData.intensity);
            lightData.intensity = math.abs(((previousIntesityInLumens-intensityInLumens)/lightProgressionDelta)-lightData.intensity);
            if (lightData.intensity >= 1f)
            {
                lightData.gameObject.SetActive(true);
            }
            else
            {
                lightData.gameObject.SetActive(false);
            }
            await Task.Yield();
        }
    }
}
