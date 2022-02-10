using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soil.Threading.Atomic;

namespace Soil.Threading.Tasks;

public class FixedThreadTaskScheduler : AbstractTaskScheduler
{
    private readonly int _maximumConcurrencyLevel;

    private readonly IThreadFactory _threadFactory;

    private readonly BlockingCollection<Task> _tasks;

    private readonly Thread[] _threads;

    private readonly HashSet<int> _managedThreadIds;

    private readonly AtomicBool _disposed = false;

    public override int MaximumConcurrencyLevel
    {
        get
        {
            return _maximumConcurrencyLevel;
        }
    }

    public override IThreadFactory ThreadFactory
    {
        get
        {
            return _threadFactory;
        }
    }

    internal FixedThreadTaskScheduler(
        int maximumConcurrencyLevel,
        IThreadFactory threadFactory) :
        this(maximumConcurrencyLevel, threadFactory, new())
    { }

    internal FixedThreadTaskScheduler(
        int maximumConcurrencyLevel,
        IThreadFactory threadFactory,
        BlockingCollection<Task> tasks)
    {
        // call and discard to create unique id of scheduler
        _ = Id;

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

        _managedThreadIds = _threads.Select(th => th.ManagedThreadId)
            .ToHashSet();
    }

    ~FixedThreadTaskScheduler()
    {
        Dispose(false);
    }

    public override bool HasThread(int managedThreadId)
    {
        return _managedThreadIds.Contains(managedThreadId);
    }

    public override void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    public override void JoinAll()
    {
        Dispose();

        Parallel.ForEach(_threads, thread => thread.Join());
    }

    public override void JoinAll(TimeSpan timeout)
    {
        Dispose();

        Parallel.ForEach(_threads, thread => thread.Join(timeout));
    }

    public override void JoinAll(int millisecondsTimeout)
    {
        Dispose();

        Parallel.ForEach(_threads, thread => thread.Join(millisecondsTimeout));
    }

    protected override void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (_disposed.Exchange(true))
        {
            return;
        }

        _tasks.CompleteAdding();
        _tasks.Dispose();
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
}
