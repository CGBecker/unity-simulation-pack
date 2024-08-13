using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Displacement : BaseSensor
{
    // command if on-demand take reading
    public override void InitialiseSensor()
    {
        throw new System.NotImplementedException();
    }

    // take reading, on-demand or always running
    public override float[] TakeReading()
    {
        throw new System.NotImplementedException();
    }
    public override float TakeReading(uint index)
    {
        throw new System.NotImplementedException();
    }
}
