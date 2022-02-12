using System;

namespace Soil.SimpleActorModel.Dispatcher;

public class DispatcherProps
{
    private string _id;

    private string _type;

    private int _throughputPerActor;

    public string Id
    {
        get
        {
            return _id;
        }
    }

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

    public DispatcherProps()
    {
        _id = Dispatchers.DefaulId;
        _type = Dispatchers.DefaultType;
        _throughputPerActor = Dispatchers.DefaultThroughput;
    }

    public DispatcherProps SetId(string id)
    {
        if (string.IsNullOrEmpty(id))
        {
            throw new ArgumentException($"{nameof(id)} is null or empty", id);
        }

        _id = id;

        return this;
    }

    public DispatcherProps SetType(string type)
    {
        if (string.IsNullOrEmpty(type))
        {
            throw new ArgumentException($"{nameof(type)} is null or empty", type);
        }

        _type = type;

        return this;
    }

    public DispatcherProps SetThroughputPerActor(int throughputPerActor)
    {
        if (throughputPerActor <= 0)
        {
            throw new ArgumentOutOfRangeException(
                nameof(throughputPerActor),
                throughputPerActor,
                null);
        }

        _throughputPerActor = throughputPerActor;

        return this;
    }
}
