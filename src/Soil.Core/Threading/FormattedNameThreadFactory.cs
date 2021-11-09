using System.Threading;

namespace Soil.Core.Threading;

internal class FormattedNameThreadFactory : IThreadFactory
{
    private readonly ThreadPriority _priority;
    public ThreadPriority Priority => _priority;

    private readonly ThreadNameFormatter _formatter;
    public ThreadNameFormatter Formatter => _formatter;

    public FormattedNameThreadFactory(ThreadPriority priority_, ThreadNameFormatter formatter_)
    {
        _priority = priority_;
        _formatter = formatter_;
    }

    public Thread Create(ThreadStart start_)
    {
        var thread = new Thread(start_);
        thread.Name = _formatter.Format(thread.ManagedThreadId);
        return thread;
    }

    public Thread Create(ParameterizedThreadStart start_)
    {
        var thread = new Thread(start_);
        thread.Name = _formatter.Format(thread.ManagedThreadId);
        return thread;
    }
}
