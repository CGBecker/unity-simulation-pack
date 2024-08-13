using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseActuator : MonoBehaviour
{
    public abstract void Command(float command);
    public abstract void Command(float[] command);

    public abstract float TakeReading(uint index);
    public abstract float[] TakeReading();
}
