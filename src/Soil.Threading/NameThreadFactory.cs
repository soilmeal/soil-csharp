using System.Threading;

namespace Soil.Threading;

internal class NameThreadFactory : IThreadFactory
{
    private readonly ThreadPriority _priority;

    private readonly string _name;

    public ThreadPriority Priority
    {
        get
        {
            return _priority;
        }


    }

    public string Name
    {
        get
        {
            return _name;
        }
    }

    internal NameThreadFactory()
        : this(ThreadPriority.Normal, "thread") { }

    internal NameThreadFactory(ThreadPriority priority, string name)
    {
        _priority = priority;
        _name = name;
    }

    public Thread Create(ThreadStart start)
    {
        return Create(start, false);
    }

    public Thread Create(ThreadStart start, bool backgound)
    {
        var thread = new Thread(start)
        {
            Name = _name,
            IsBackground = backgound,
        };
        return thread;
    }

    public Thread Create(ParameterizedThreadStart start)
    {
        return Create(start, false);
    }

    public Thread Create(ParameterizedThreadStart start, bool backgound)
    {
        var thread = new Thread(start)
        {
            Name = _name,
            IsBackground = backgound,
        };
        return thread;
    }
}
