using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Lidar : BaseSensor
{
    // Array of lidars

    // initialise lider using raycast command for parallel with settings received
    public override void InitialiseSensor()
    {
        throw new System.NotImplementedException();
    }

    // command method for receiving commands 

    // take reading method to send data back
    public override float[] TakeReading()
    {
        throw new System.NotImplementedException();
    }
    public override float TakeReading(uint index)
    {
        throw new System.NotImplementedException();
    }
}
