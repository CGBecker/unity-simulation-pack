using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System.Threading.Tasks;
using Unity.Mathematics;
using Unity.Burst;

/// <summary>
/// Base light controller class for generic light control
/// </summary>
public class BaseLightController : MonoBehaviour
{
    /// <summary>
    /// Lights to be controlled
    /// OBS.: HDRP only due to use of HDAdditionalLightData and Lumens
    /// </summary>
    public Light[] Lights;
    private HDAdditionalLightData[] _lightsData;
    public float LightsTargetIntensityOn;
    private float _previousLightsIntensity;
    private float _lightsTargetIntensity;
    private int _lightsProgressionDelta = 40;  // Default set to 40. Higher value equals slower transition
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
    public void InitialiseLights(Light[] lights, int lightProgresssiveShiftDelta)
    {
        _lightsProgressionDelta = lightProgresssiveShiftDelta;
        if (Lights.Length == 0)
        {
            Lights = lights;
        }
        _lightsData = new HDAdditionalLightData[Lights.Length];
        _tasks = new Task[Lights.Length];
        for (int lightsIndex = 0; lightsIndex < lights.Length; lightsIndex++)
        {
            _lightsData[lightsIndex] = Lights[lightsIndex].GetComponent<HDAdditionalLightData>();
            _lightsData[lightsIndex].lightUnit = LightUnit.Lumen;
            _lightsData[lightsIndex].intensity = 0f;
            _lightsData[lightsIndex].gameObject.SetActive(false);
        }
    }

    /// <summary>
    /// Command method for setting light intensity target for all lights
    /// </summary>
    /// <param name="intensityInLumens"></param>
    public void LightCommand(float intensityInLumens)
    {

    }

    /// <summary>
    /// Command method for setting light intensity target for one light
    /// </summary>
    /// <param name="light"></param>
    /// <param name="intensityInLumens"></param>
    public void LightCommand(Light light, float intensityInLumens)
    {

    }

}

/// <summary>
/// Light control static class for progressive behaviour of light device
/// </summary>
public static class ProgressiveLightControl
{
   public static Task ProgressiveLightControlTask(float intensityInLumens, float previousIntesityInLumens)
   {
       return Task.Run(() =>
       {
           // Do task           
       });
   }
}
