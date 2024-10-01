using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.HighDefinition;
using System.Threading.Tasks;
using System;
using System.Threading;

/// <summary>
/// Base light controller class for generic light control
/// </summary>
public class BaseLightActuator : BaseActuator
{
    public Light[] Lights;
    private HDAdditionalLightData[] _lightsData;
    private float[] _previousLightsIntensity;

    public override void InitialiseDevice()
    {
        throw new System.NotImplementedException();
    }

    public override void ConfigureDevice()
    {
        _lightsData = new HDAdditionalLightData[Lights.Length];
        for (int lightsIndex = 0; lightsIndex < Lights.Length; lightsIndex++)
        {
            _lightsData[lightsIndex] = Lights[lightsIndex].GetComponent<HDAdditionalLightData>();
            _lightsData[lightsIndex].lightUnit = LightUnit.Lumen;
            _lightsData[lightsIndex].intensity = 0f;
            _lightsData[lightsIndex].gameObject.SetActive(false);
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
    /// Instant light command for one or all Lights at once
    /// </summary>
    /// <param name="intensity"></param>
    public override bool Command<T>(T intensity)
    {
        if (intensity is float intensityInFloat)
        {
            for (uint lightsIndex = 0; lightsIndex < _lightsData.Length; ++lightsIndex)
            {
                InstantLightSet(_lightsData[lightsIndex], intensityInFloat);
            }
        }
        else if (intensity is float[] intensitiesInFloatArray)
        {
            if (intensitiesInFloatArray.Length == _lightsData.Length)
            {
                for (uint lightsIndex = 0; lightsIndex < intensitiesInFloatArray.Length; ++lightsIndex)
                {
                    InstantLightSet(_lightsData[lightsIndex], intensitiesInFloatArray[lightsIndex]);
                }
            }
            else
            {
                Debug.LogError("Number of intensity targets provided do not match number of light devices!");
            }
        }
        else
        {
            Debug.LogError("ERROR: Unexpected type for light command. Expected: float or float[]. Received: " + typeof(T) );
        }
        return true;
    }

    public override float[] TakeReading()
    {
        throw new NotImplementedException();
    }

    public override float TakeReading(uint index)
    {
        throw new NotImplementedException();
    }

}