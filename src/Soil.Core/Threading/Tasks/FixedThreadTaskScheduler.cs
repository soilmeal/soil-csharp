using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Threading.Tasks;

public class FixedThreadTaskScheduler : AbstractTaskScheduler
{
    private readonly ILogger<FixedThreadTaskScheduler> _logger;

    private readonly int _maximumConcurrencyLevel;
    public override int MaximumConcurrencyLevel
    {
        get
        {
            return _maximumConcurrencyLevel;
        }
    }

    private readonly IThreadFactory _threadFactory;
    public override IThreadFactory ThreadFactory
    {
        get
        {
            return _threadFactory;
        }
    }

    private readonly BlockingCollection<Task> _tasks;

    private readonly Thread[] _threads;

    internal FixedThreadTaskScheduler(
        int maximumConcurrencyLevel,
        IThreadFactory threadFactory,
        ILoggerFactory loggerFactory) :
        this(maximumConcurrencyLevel, threadFactory, new(), loggerFactory)
    { }

    internal FixedThreadTaskScheduler(
        int maximumConcurrencyLevel,
        IThreadFactory threadFactory,
        BlockingCollection<Task> tasks,
        ILoggerFactory loggerFactory)
    {
        // call and discard to create unique id of scheduler
        _ = Id;

        _logger = loggerFactory.CreateLogger<FixedThreadTaskScheduler>();

        _maximumConcurrencyLevel = maximumConcurrencyLevel;
        _threadFactory = threadFactory;
        _tasks = tasks;

        _threads = Enumerable.Range(0, maximumConcurrencyLevel)
            .Select(_ => ThreadFactory.Create(Execute, true))
            .ToArray();
        foreach (var thread in _threads)
        {
            thread.Start();
        }
    }

    ~FixedThreadTaskScheduler()
    {
        Dispose(false);
    }

    protected override IEnumerable<Task>? GetScheduledTasks()
    {
        return _tasks.ToArray();
    }

    protected override void QueueTask(Task task_)
    {
        _tasks.Add(task_);
    }

    protected override bool TryExecuteTaskInline(Task task_, bool taskWasPreviouslyQueued_)
    {
        return false;
    }

    protected override bool TryDequeue(Task task_)
    {
        return false;
    }

    private void Execute()
    {
        foreach (var task in _tasks.GetConsumingEnumerable())
        {
            TryExecuteTask(task);
        }
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _tasks.CompleteAdding();
        _tasks.Dispose();
    }
}
