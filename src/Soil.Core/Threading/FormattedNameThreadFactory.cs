using System.Threading;

namespace Soil.Core.Threading;

internal class FormattedNameThreadFactory : IThreadFactory
{
    private static readonly FormattedNameThreadFactory _default = new();
    public static FormattedNameThreadFactory Default
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

    private readonly ThreadNameFormatter _formatter;
    public ThreadNameFormatter Formatter
    {
        get
        {
            return _formatter;
        }
    }

    private FormattedNameThreadFactory()
        : this(ThreadPriority.Normal, ThreadNameFormatter.Default)
    { }

    internal FormattedNameThreadFactory(ThreadPriority priority, ThreadNameFormatter formatter)
    {
        _priority = priority;
        _formatter = formatter;
    }

    public Thread Create(ThreadStart start)
    {
        var thread = new Thread(start);
        thread.Name = _formatter.Format(thread.ManagedThreadId);
        return thread;
    }

    public Thread Create(ParameterizedThreadStart start)
    {
        var thread = new Thread(start);
        thread.Name = _formatter.Format(thread.ManagedThreadId);
        return thread;
    }
}
