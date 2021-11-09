using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Soil.Core.Threading.Tasks;

public class FixedThreadTaskScheduler : TaskScheduler
{
    private readonly int _maximumConcurrencyLevel;
    public override int MaximumConcurrencyLevel => _maximumConcurrencyLevel;

    private readonly IThreadFactory _threadFactory;
    public override IThreadFactory ThreadFactory => _threadFactory;

    private readonly BlockingCollection<Task> _tasks;

    private readonly List<Thread> _threads;

    internal FixedThreadTaskScheduler(int maximumConcurrencyLevel_, IThreadFactory threadFactory_) :
        this(maximumConcurrencyLevel_, threadFactory_, new())
    { }

    internal FixedThreadTaskScheduler(int maximumConcurrencyLevel_, IThreadFactory threadFactory_, BlockingCollection<Task> tasks_)
    {
        // call and discard to create unique id of scheduler
        _ = Id;

        _maximumConcurrencyLevel = maximumConcurrencyLevel_;
        _threadFactory = threadFactory_;
        _tasks = tasks_;

        _threads = Enumerable.Range(0, maximumConcurrencyLevel_)
            .Select(i => ThreadFactory.Create(Execute))
            .ToList();
        _threads.ForEach(thread => thread.Start());
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
