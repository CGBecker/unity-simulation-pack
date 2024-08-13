using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Manages the flow of time, regulating simulation time in order not to compromise accuracy
/// </summary>
public class SimTimeManager : MonoBehaviour
{
    private static SimTimeManager _instance;
    public static SimTimeManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<SimTimeManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject("SimTimeManager");
                    _instance = singletonObject.AddComponent<SimTimeManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    /// <summary>
    /// NOT IMPLEMENTED: Initialisation of time management with settings of maxDelta and physicsStep in ms
    /// </summary>
    /// <param name="maxDelta"></param>
    /// <param name="physicsStep"></param>
    public void InitializeSettings(uint maxDelta, uint physicsStep)
    {
        Debug.Log("Setting Time Management for MaxDelta:" + maxDelta + " and physicsStep:" + physicsStep);
    }

    private void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else if (_instance != this)
        {
            Destroy(gameObject);
        }
    }

}
