using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class BaseActuator : MonoBehaviour
{
    public abstract bool Command<T>(T command);

    public abstract float TakeReading(uint index);
    public abstract float[] TakeReading();
}
