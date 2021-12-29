using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Threading;

internal class NameThreadFactory : IThreadFactory
{
    private readonly ILogger<NameThreadFactory> _logger;

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

    internal NameThreadFactory(ILoggerFactory loggerFactory)
        : this(ThreadPriority.Normal, "thread", loggerFactory) { }

    internal NameThreadFactory(ThreadPriority priority, string name, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<NameThreadFactory>();

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
