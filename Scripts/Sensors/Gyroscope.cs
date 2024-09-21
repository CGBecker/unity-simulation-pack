using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

/// <summary>
/// Struct of the DeviceData for Gyroscope in orientation (Vector3) and angularVelocity (Vector3)
/// Data is separated per each Gyroscope sensor (array of DeviceData for all gyro sensors)
/// </summary>
public struct DeviceData
{
    public Vector3 orientationData;
    public Vector3 angularVelocityData;
}
public class Gyroscope : BaseSensor
{
    public Rigidbody[] rigidbodies;
    public ArticulationBody[] articulationBodies;
    private Transform[] transforms;
    private DeviceData[] deviceDatas;

    /// <summary>
    /// Initialises Gyroscope sensor if either articulation bodies or rigidbodies have been assigned
    /// </summary>
    public override void InitialiseSensor()
    {
        if (rigidbodies != null)
        {
            deviceDatas = new DeviceData[rigidbodies.Length];
            transforms = new Transform[rigidbodies.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i] = rigidbodies[i].transform;
            }
        }        
        else if (articulationBodies != null)
        {
            deviceDatas = new DeviceData[articulationBodies.Length];
            transforms = new Transform[articulationBodies.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i] = articulationBodies[i].transform;
            }
        }
        else
        {
            Debug.LogError("Found no Rigidbody nor ArticulationBody to measure Gyroscope data");
        }
    }

    /// <summary>
    /// Returns the data of the Gyroscope
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <returns>DeviceData[] of Gyroscope as two Vector3, orientation and angular velocity, per DeviceData</returns>
    public override T TakeReading<T>()
    {
        if (rigidbodies != null)
        {
            for (int i = 0; i < deviceDatas.Length; i++)
            {
                deviceDatas[i].angularVelocityData = rigidbodies[i].angularVelocity;
                deviceDatas[i].orientationData = transforms[i].eulerAngles;
            }
        }
        else if (articulationBodies != null)
        {
            for (int i = 0; i < deviceDatas.Length; i++)
            {
                deviceDatas[i].angularVelocityData = articulationBodies[i].angularVelocity;
                deviceDatas[i].orientationData = transforms[i].eulerAngles;
            }
        }
        return (T)(object)deviceDatas;
    }

    /// <summary>
    /// Returns the data of the Gyroscope
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns>DeviceData of Gyroscope as two Vector3, orientation and angular velocity, at the index</returns>
    public override T TakeReading<T>(uint index)
    {
        if (rigidbodies != null)
        {
            deviceDatas[index].angularVelocityData = rigidbodies[index].angularVelocity;
            deviceDatas[index].orientationData = transforms[index].eulerAngles;
        }
        else if (articulationBodies != null)
        {
            deviceDatas[index].angularVelocityData = articulationBodies[index].angularVelocity;
            deviceDatas[index].orientationData = transforms[index].eulerAngles;
        }
        return (T)(object)deviceDatas[index];
    }
}
