using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displacement : BaseSensor
{
    private Vector3[] previousPositions;
    public Transform[] DisplacementObjects;

    public override void ConfigureDevice()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Initialise Displacement Sensor
    /// WARNING: DisplacementObjects variable must be assigned before initialisation
    /// </summary>
    public override void InitialiseSensor()
    {
        previousPositions = new Vector3[DisplacementObjects.Length];
        for (int i = 0; i < DisplacementObjects.Length; i++)
        {
            previousPositions[i] = DisplacementObjects[i].position;
        }
    }

    /// <summary>
    /// Take reading of displacement sensor array
    /// </summary>
    /// <returns>Array of floats with distances of displacement per sensor in meters</returns>
    public override T TakeReading<T>()
    {
        float[] displacementsInMeters = new float[DisplacementObjects.Length];
        Vector3 tempPose;
        for (int i = 0; i < DisplacementObjects.Length; i++)
        {
            tempPose = DisplacementObjects[i].position - previousPositions[i];
            displacementsInMeters[i] = tempPose.magnitude;
            previousPositions[i] = DisplacementObjects[i].position;
        }

        return (T)(object)displacementsInMeters;
    }

    /// <summary>
    /// Take reading of single displacement sensor at <paramref name="index"/>
    /// </summary>
    /// <param name="index"></param>
    /// <returns>A float with the distance of displacement in meters</returns>
    public override T TakeReading<T>(uint index)
    {
        Vector3 tempPose = DisplacementObjects[index].position - previousPositions[index];
        float displacementsInMeters = tempPose.magnitude;
        previousPositions[index] = DisplacementObjects[index].position;

        return (T)(object)displacementsInMeters;
    }
}
