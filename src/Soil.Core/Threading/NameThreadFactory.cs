using System.Threading;

namespace Soil.Core.Threading;

internal class NameThreadFactory : IThreadFactory
{
    private readonly ThreadPriority _priority;
    public ThreadPriority Priority => _priority;

    private readonly string _name;
    public string Name => _name;

    public NameThreadFactory(ThreadPriority priority_, string name_)
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
