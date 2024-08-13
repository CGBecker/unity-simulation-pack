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
public class BaseLightActuator : BaseActuator
{
    private HDAdditionalLightData[] _lightsData;
    private float[] _previousLightsIntensity;
    private Task[] _tasks;
    private CancellationTokenSource[] _sources;
    private CancellationToken[] _tokens;

    /// <summary>
    /// 
    /// </summary>
    /// <param name="lights"></param>
    /// <param name="enableProgressiveDeltaSupport">Enables progressive light control support. (OBS.: Experimental, may cause overhead and instability)</param>
    public void InitialiseLights(Light[] lights, bool enableProgressiveDeltaSupport)
    {
        _lightsData = new HDAdditionalLightData[lights.Length];
        for (int lightsIndex = 0; lightsIndex < lights.Length; lightsIndex++)
        {
            _lightsData[lightsIndex] = lights[lightsIndex].GetComponent<HDAdditionalLightData>();
            _lightsData[lightsIndex].lightUnit = LightUnit.Lumen;
            _lightsData[lightsIndex].intensity = 0f;
            _lightsData[lightsIndex].gameObject.SetActive(false);
        }
        if (enableProgressiveDeltaSupport)
        {
            _tasks = new Task[lights.Length];
            _sources = new CancellationTokenSource[lights.Length];
            _tokens = new CancellationToken[lights.Length];
        }
    }

    private void InstantLightSet(HDAdditionalLightData lightData, float intensityInLumens)
    {
        lightData.intensity = intensityInLumens;
        if (intensityInLumens < 1f)
        {
            lightData.gameObject.SetActive(false);
        }
        else
        {
            lightData.gameObject.SetActive(true);
        }
    }

    /// <summary>
    /// Instant light shift command for individual lights
    /// </summary>
    /// <param name="intensities"></param>
    public override void Command(float[] intensities)
    {
        if (intensities.Length == _lightsData.Length)
        {
            for (uint lightsIndex = 0; lightsIndex < intensities.Length; ++lightsIndex)
            {
                InstantLightSet(_lightsData[lightsIndex], intensities[lightsIndex]);
            }
        }
        else
        {
            Debug.LogError("Number of intensity targets provided do not match number of light devices!");
        }
    }

    /// <summary>
    /// Instant light command for all lights at once
    /// </summary>
    /// <param name="intensity"></param>
    public override void Command(float intensity)
    {
        for (uint lightsIndex = 0; lightsIndex < _lightsData.Length; ++lightsIndex)
        {
            InstantLightSet(_lightsData[lightsIndex], intensity);
        }
    }

    public override float[] TakeReading()
    {
        throw new NotImplementedException();
    }

    public override float TakeReading(uint index)
    {
        throw new NotImplementedException();
    }

    /// <summary>
    /// Command method for setting light intensity target for all lights progressively
    /// Indexes must be tied for all arrays (e.g. Light[1].intensity = intensityInLumens[1])
    /// OBS.: Will take a certain time to reach target (lightsProgressionDelta*Time.fixedDeltaTime)
    /// </summary>
    /// <param name="intensityInLumens"></param>
    /// <param name="lightsProgressionDelta"></param>
    public void LightCommand(float[] intensityInLumens, uint[] lightsProgressionDelta)
    {
        Debug.Log("Light Command");
        for (int lightsIndex = 0; lightsIndex < _lightsData.Length; lightsIndex++)
        {
            _previousLightsIntensity[lightsIndex] = _lightsData[lightsIndex].intensity;
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
            _tasks[lightsIndex] = Task.Factory.StartNew(() => ProgressiveLightControl.ProgressiveLightControlTask(
                                                                                _lightsData[lightsIndex],
                                                                                intensityInLumens[lightsIndex],
                                                                                _previousLightsIntensity[lightsIndex],
                                                                                lightsProgressionDelta[lightsIndex]),
                                                                                _tokens[lightsIndex]);
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
