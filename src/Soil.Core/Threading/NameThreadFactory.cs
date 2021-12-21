using System.Threading;

namespace Soil.Core.Threading;

internal class NameThreadFactory : IThreadFactory
{
    private static readonly NameThreadFactory _default = new();
    public static NameThreadFactory Default
    {
        get
        {
            return _default;
        }
    }

    private readonly ThreadPriority _priority;
    public ThreadPriority Priority
    {
        get
        {
            return _priority;
        }
    }

    private readonly string _name;
    public string Name
    {
        get
        {
            return _name;
        }
    }

    private NameThreadFactory() : this(ThreadPriority.Normal, "thread") { }

    internal NameThreadFactory(ThreadPriority priority_, string name_)
    {
        _priority = priority_;
        _name = name_;
    }

    public Thread Create(ThreadStart start_)
    {
        var thread = new Thread(start_)
        {
            Name = _name
        };
        return thread;
    }

    public Thread Create(ParameterizedThreadStart start_)
    {
        var thread = new Thread(start_)
        {
            Name = _name
        };
        return thread;
    }
}
