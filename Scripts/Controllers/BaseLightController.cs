using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.Burst;
using System.Linq;
using System;

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

    /// <summary>
    ///  Initialise lights with given config
    /// </summary>
    /// <param name="lights"></param>
    public void InitialiseLights(Light[] lights)
    {
        if (Lights.Length == 0)
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
    }

    /// <summary>
    /// Initialise lights with given config and delta for transition speed (Higher value equals slower transition)
    /// </summary>
    /// <param name="lights"></param>
    /// <param name="lightProgresssiveShiftDelta"></param>
    public void InitialiseLights(Light[] lights, uint lightProgresssiveShiftDelta)
    {
        _lightsProgressionDelta = lightProgresssiveShiftDelta;
        InitialiseLights(lights);
    }

    /// <summary>
    /// Command method for setting light intensity target for all lights
    /// </summary>
    /// <param name="intensityInLumens"></param>
    public void LightCommand(float intensityInLumens)
    {
        _previousLightsIntensity = _lightsData[0].intensity;  // Consider array of cached previous intensities
        for (int lightsIndex = 0; lightsIndex < _lightsData.Length; lightsIndex++)
        {
            if(_tasks[lightsIndex].Status.Equals(TaskStatus.Running))
            {
                _tasks[lightsIndex].Wait();
            }
            _tasks[lightsIndex] = ProgressiveLightControl.ProgressiveLightControlTask(_lightsData[lightsIndex], intensityInLumens, _previousLightsIntensity, _lightsProgressionDelta);
        }
    }

    /// <summary>
    /// Command method for setting light intensity target for one light
    /// </summary>
    /// <param name="lightData"></param>
    /// <param name="intensityInLumens"></param>
    public void LightCommand(HDAdditionalLightData lightData, float intensityInLumens)
    {
        _previousLightsIntensity = lightData.intensity;  // Consider array of cached previous intensities
        if(_lightsData.Contains(lightData))
        {
            int index = Array.FindIndex(_lightsData, row => _lightsData.Contains(lightData));
            if(_tasks[index].Status.Equals(TaskStatus.Running))
            {
                _tasks[index].Wait();
            }
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
   public static async Task ProgressiveLightControlTask(HDAdditionalLightData lightData, float intensityInLumens, float previousIntesityInLumens, uint lightProgressionDelta)
   {
       await Task.Run(() =>
       {
            for (uint elapsedDelta = lightProgressionDelta; elapsedDelta > 0; --elapsedDelta)
            {
                lightData.intensity = math.abs(((previousIntesityInLumens-intensityInLumens)/lightProgressionDelta)-lightData.intensity);
            }
       });
   }
}
