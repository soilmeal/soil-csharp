using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Soil.Core.Threading.Tasks;

public class TaskSchedulerBuilder
{
    private int _maximumConcurrencyLevel = 0;
    public int MaximumConcurrencyLevel => _maximumConcurrencyLevel;

    public IThreadFactory? _threadFactory;
    public IThreadFactory? ThreadFactory => _threadFactory;

    private BlockingCollection<Task>? _queue;
    public BlockingCollection<Task>? Queue => _queue;

    public TaskSchedulerBuilder() { }

    public TaskSchedulerBuilder SetMaximumConcurrencyLevel(int maximumConcurrencyLevel_)
    {
        _maximumConcurrencyLevel = maximumConcurrencyLevel_;
        return this;
    }

    public TaskSchedulerBuilder SetThreadFactory(IThreadFactory threadFactory_)
    {
        _threadFactory = threadFactory_;
        return this;
    }

    public TaskSchedulerBuilder SetQueue(BlockingCollection<Task> queue_)
    {
        _queue = queue_;
        return this;
    }

    public TaskScheduler Build()
    {
        BlockingCollection<Task>? queue = Queue;
        if (queue == null)
        {
            throw new NullReferenceException("Queue is null");
        }

        IThreadFactory threadFactory = ThreadFactory ?? ThreadFactoryBuilder.BuildDefault();

        int maximumConcurrencyLevel = CoerceMaximumConcurrencyLevel();
        return new TaskScheduler(maximumConcurrencyLevel, threadFactory, queue);
    }

    public static TaskScheduler BuildSingleThread()
    {
        var builder = new TaskSchedulerBuilder();
        return builder.SetMaximumConcurrencyLevel(1)
            .SetThreadFactory(ThreadFactoryBuilder.BuildDefault())
            .SetQueue(new BlockingCollection<Task>())
            .Build();
    }

    public static TaskScheduler BuildWorkStealing(int maximumConcurrencyLevel_)
    {
        if (maximumConcurrencyLevel_ == 1)
        {
            throw new ArgumentOutOfRangeException(nameof(maximumConcurrencyLevel_), maximumConcurrencyLevel_, "maximumConcurrencyLevel of BuildWorkStealing() is must be greater than or less than 1");
        }

        var builder = new TaskSchedulerBuilder();
        return builder.SetMaximumConcurrencyLevel(maximumConcurrencyLevel_)
            .SetThreadFactory(ThreadFactoryBuilder.BuildDefault())
            .SetQueue(new BlockingCollection<Task>(new ConcurrentBag<Task>()))
            .Build();
    }

    private int CoerceMaximumConcurrencyLevel()
    {
        int maximumConcurrencyLevel = MaximumConcurrencyLevel;
        return maximumConcurrencyLevel > 0 ? maximumConcurrencyLevel : Environment.ProcessorCount;
    }
}
