public class DeviceData<T>
{
    private string _time;
    public string Time
    {
        get
        {
            return _time;
        }
        set
        {
            _time = value;
        }
    }

    private T _data;

    public T Data
    {
        get
        {
            return _data;
        }
        set
        {
            _data = value;
        }
    }
}