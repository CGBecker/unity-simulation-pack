using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class Gyroscope : BaseSensor
{
    public Rigidbody[] rigidbodies;
    public ArticulationBody[] articulationBodies;
    private Transform[] transforms;
    private DeviceData<Vector3[]>[] deviceDatas;

    public override void ConfigureDevice()
    {
        throw new System.NotImplementedException();
    }

    /// <summary>
    /// Initialises Gyroscope sensor if either articulation bodies or rigidbodies have been assigned
    /// </summary>
    public override void InitialiseSensor()
    {
        if (rigidbodies != null)
        {
            deviceDatas = new DeviceData<Vector3[]>[rigidbodies.Length];
            transforms = new Transform[rigidbodies.Length];
            for (int i = 0; i < transforms.Length; i++)
            {
                transforms[i] = rigidbodies[i].transform;
            }
        }        
        else if (articulationBodies != null)
        {
            deviceDatas = new DeviceData<Vector3[]>[articulationBodies.Length];
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
    /// <returns>DeviceData[] of Gyroscope as a Time string and an array of two Vector3, orientation[1] and angular velocity[0], per DeviceData</returns>
    public override T TakeReading<T>()
    {
        if (rigidbodies != null)
        {
            for (int i = 0; i < deviceDatas.Length; i++)
            {
                Vector3[] tempData = new Vector3[2];
                tempData[0] = rigidbodies[i].angularVelocity;
                tempData[1] = transforms[i].eulerAngles;
                deviceDatas[i].Time = "XXXXXXXX";
                deviceDatas[i].Data = tempData;
            }
        }
        else if (articulationBodies != null)
        {
            for (int i = 0; i < deviceDatas.Length; i++)
            {
                Vector3[] tempData = new Vector3[2];
                tempData[0] = articulationBodies[i].angularVelocity;
                tempData[1] = transforms[i].eulerAngles;
                deviceDatas[i].Time = "XXXXXXXX";
                deviceDatas[i].Data = tempData;
            }
        }
        return (T)(object)deviceDatas;
    }

    /// <summary>
    /// Returns the data of the Gyroscope
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="index"></param>
    /// <returns>DeviceData of Gyroscope as a Time string and an array of two Vector3, orientation[1] and angular velocity[0], at the index</returns>
    public override T TakeReading<T>(uint index)
    {
        if (rigidbodies != null)
        {
            Vector3[] tempData = new Vector3[2];
            tempData[0] = rigidbodies[index].angularVelocity;
            tempData[1] = transforms[index].eulerAngles;
            deviceDatas[index].Time = "XXXXXXXX";
            deviceDatas[index].Data = tempData;
            
        }
        else if (articulationBodies != null)
        {
            Vector3[] tempData = new Vector3[2];
            tempData[0] = articulationBodies[index].angularVelocity;
            tempData[1] = transforms[index].eulerAngles;
            deviceDatas[index].Time = "XXXXXXXX";
            deviceDatas[index].Data = tempData;
        }
        return (T)(object)deviceDatas[index];
    }
}
