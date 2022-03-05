using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Message;
using Soil.Threading;
using Soil.Threading.Atomic;
using Soil.Threading.Tasks;

namespace Soil.SimpleActorModel.Dispatcher;

public class TaskSchedulerDispatcher : IDispatcher
{
    private readonly string _id;

    private readonly int _throughputPerActor;

    private readonly AbstractTaskScheduler _taskScheduler;

    private readonly TaskFactory _taskFactory;

    private readonly AtomicBool _disposed = false;

    public string Id
    {
        get
        {
            return _id;
        }
    }

    public int ThroughputPerActor
    {
        get
        {
            return _throughputPerActor;
        }
    }

    public TaskSchedulerDispatcher(
        string id,
        int throughputPerActor)
    {
        _id = id;
        _throughputPerActor = throughputPerActor;

        IThreadFactory threadFactory = new IThreadFactory.Builder()
            .SetPriority(ThreadPriority.Normal)
            .Build(new ThreadNameFormatter($"{id}-{{0}}"));

        _taskScheduler = new AbstractTaskScheduler.Builder()
            .SetThreadFactory(threadFactory)
            .SetQueue(new ConcurrentQueue<Task>())
            .SetMaximumConcurrencyLevel(1)
            .Build();
        _taskFactory = new TaskFactory(_taskScheduler);
    }

    ~TaskSchedulerDispatcher()
    {
        Dispose(false);
    }

    public void Dispatch(ActorCell actorCell, Envelope envelope)
    {
        if (actorCell == null)
        {
            throw new ArgumentNullException(nameof(actorCell));
        }

        if (!actorCell.Mailbox.TryAdd(envelope))
        {
            return;
        }

        TryExecuteMailbox(actorCell.Mailbox);
    }

    public bool TryExecuteMailbox(Mailbox mailbox)
    {
        if (!mailbox.HasAnyMessage())
        {
            return false;
        }

        if (!mailbox.TrySetScheduled())
        {
            return false;
        }

        Execute(mailbox.Process);
        return true;
    }

    public Task Execute(Action action)
    {
        return _taskFactory.StartNew(action);
    }

    public Task<T> Execute<T>(Func<T> func)
    {
        return _taskFactory.StartNew(func);
    }

    public void JoinAll()
    {
        Dispose();

        _taskScheduler.JoinAll();
    }

    public void JoinAll(TimeSpan timeout)
    {
        Dispose();

        _taskScheduler.JoinAll(timeout);
    }

    public void JoinAll(int millisecondsTimeout)
    {
        Dispose();

        _taskScheduler.JoinAll(millisecondsTimeout);
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        if (_disposed.Exchange(true))
        {
            return;
        }

        _taskScheduler.Dispose();
    }
}
