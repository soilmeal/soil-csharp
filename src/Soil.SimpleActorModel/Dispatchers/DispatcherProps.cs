namespace Soil.SimpleActorModel.Dispatchers;

public class DispatcherProps
{
    private readonly string _type;

    private readonly int _throughputPerActor;

    public string Type
    {
        get
        {
            return _type;
        }
    }

    public int ThroughputPerActor
    {
        get
        {
            return _throughputPerActor;
        }
    }

    public DispatcherProps(string type, int throughputPerActor)
    {
        _type = type;
        _throughputPerActor = throughputPerActor;
    }
}
