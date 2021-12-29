using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Threading;

internal class FormattedNameThreadFactory : IThreadFactory
{
    private readonly ILogger<FormattedNameThreadFactory> _logger;

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

    internal FormattedNameThreadFactory(ILoggerFactory loggerFactory)
        : this(ThreadPriority.Normal, new ThreadNameFormatter(loggerFactory), loggerFactory)
    { }

    internal FormattedNameThreadFactory(
        ThreadPriority priority,
        ThreadNameFormatter formatter,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<FormattedNameThreadFactory>();

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
