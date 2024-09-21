using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraDevice : BaseSensor
{
    // Create camera component

    // initialise and configure camera component from given settings
    public override void InitialiseSensor()
    {
        throw new System.NotImplementedException();
    }

    // Command for custom render if needed
    public override T TakeReading<T>()
    {
        throw new System.NotImplementedException();
    }
    public override T TakeReading<T>(uint index)
    {
        throw new System.NotImplementedException();
    }
}
