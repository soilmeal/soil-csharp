using System.Threading;

namespace Soil.Threading;

internal class FormattedNameThreadFactory : IThreadFactory
{
    private readonly ThreadPriority _priority;

    private readonly ThreadNameFormatter _formatter;

    public ThreadPriority Priority
    {
        get
        {
            return _priority;
        }
    }

    public ThreadNameFormatter Formatter
    {
        get
        {
            return _formatter;
        }
    }

    internal FormattedNameThreadFactory()
        : this(ThreadPriority.Normal, new ThreadNameFormatter())
    { }

    internal FormattedNameThreadFactory(
        ThreadPriority priority,
        ThreadNameFormatter formatter)
    {
        _priority = priority;
        _formatter = formatter;
    }

    public Thread Create(ThreadStart start)
    {
        return Create(start, false);
    }

    public Thread Create(ThreadStart start, bool backgound)
    {
        var thread = new Thread(start)
        {
            IsBackground = backgound,
        };
        thread.Name = _formatter.Format(thread.ManagedThreadId);
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
            IsBackground = backgound,
        };
        thread.Name = _formatter.Format(thread.ManagedThreadId);
        return thread;
    }
}
