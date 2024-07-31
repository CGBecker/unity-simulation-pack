using UnityEngine;

public class LoggingSingleton : MonoBehaviour
{
    private LoggingSingleton() { }
    private static LoggingSingleton _logging;

    private static readonly object _lock = new object();

    public static LoggingSingleton Logging
    {
        get
        {
            lock (_lock)
            {
                if (_logging == null)
                {
                    _logging = FindObjectOfType<LoggingSingleton>();

                    if (_logging == null)
                    {
                        GameObject singletonObject = new GameObject();
                        _logging = singletonObject.AddComponent<LoggingSingleton>();
                        singletonObject.name = typeof(LoggingSingleton).ToString() + " (Singleton)";

                        DontDestroyOnLoad(singletonObject);
                    }
                }
                return _logging;
            }
        }
    }

    private void Awake()
    {
        if (_logging == null)
        {
            _logging = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    public void Log(string logMessage)
    {
        Debug.Log(logMessage);
    }

    public void LogWarning(string logMessage)
    {
        Debug.LogWarning(logMessage);
    }

    public void LogError(string logMessage)
    {
        Debug.LogError(logMessage);
    }
}