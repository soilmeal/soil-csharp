using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Soil.Core.Threading.Tasks;

public abstract class TaskScheduler : System.Threading.Tasks.TaskScheduler
{
    public abstract IThreadFactory ThreadFactory { get; }

    public class Builder
    {
        private int _maximumConcurrencyLevel = 0;
        public int MaximumConcurrencyLevel
        {
            get
            {
                return _maximumConcurrencyLevel;
            }
        }

        private IThreadFactory? _threadFactory;
        public IThreadFactory? ThreadFactory
        {
            get
            {
                return _threadFactory;
            }
        }

        private BlockingCollection<Task>? _queue;
        public BlockingCollection<Task>? Queue
        {
            get
            {
                return _queue;
            }
        }

        public Builder() { }

        public Builder SetMaximumConcurrencyLevel(int maximumConcurrencyLevel)
        {
            _maximumConcurrencyLevel = maximumConcurrencyLevel;
            return this;
        }

        public Builder SetThreadFactory(IThreadFactory threadFactory)
        {
            _threadFactory = threadFactory ?? throw new ArgumentNullException(nameof(threadFactory));
            return this;
        }

        public Builder SetQueue(BlockingCollection<Task> queue)
        {
            _queue = queue ?? throw new ArgumentNullException(nameof(queue));
            return this;
        }

        public Builder SetQueue(IProducerConsumerCollection<Task> queue)
        {
            BlockingCollection<Task> blockingCollection = queue != null
                ? new BlockingCollection<Task>(queue)
                : throw new ArgumentNullException(nameof(queue));

            return SetQueue(blockingCollection);
        }

        public TaskScheduler Build()
        {
            BlockingCollection<Task> queue = GetOrDefaultQueue();
            IThreadFactory threadFactory = GetOrDefaultThreadFactory();
            int maximumConcurrencyLevel = GetOrDefaultMaximumConcurrencyLevel();
            return new FixedThreadTaskScheduler(maximumConcurrencyLevel, threadFactory, queue);
        }

        public static TaskScheduler BuildSingleThread()
        {
            var builder = new Builder();
            return builder.SetMaximumConcurrencyLevel(1)
                .SetThreadFactory(ThreadFactoryBuilder.BuildDefault())
                .SetQueue(new BlockingCollection<Task>(new ConcurrentQueue<Task>()))
                .Build();
        }

        public static TaskScheduler BuildWorkStealing(int maximumConcurrencyLevel)
        {
            if (maximumConcurrencyLevel == 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumConcurrencyLevel), maximumConcurrencyLevel, "maximumConcurrencyLevel of BuildWorkStealing() is must not be 1");
            }

            var builder = new Builder();
            return builder.SetMaximumConcurrencyLevel(maximumConcurrencyLevel)
                .SetThreadFactory(ThreadFactoryBuilder.BuildDefault())
                .SetQueue(new BlockingCollection<Task>(new ConcurrentBag<Task>()))
                .Build();
        }

        private int GetOrDefaultMaximumConcurrencyLevel()
        {
            int maximumConcurrencyLevel = _maximumConcurrencyLevel;
            return maximumConcurrencyLevel > 0 ? maximumConcurrencyLevel : Environment.ProcessorCount;
        }

        private IThreadFactory GetOrDefaultThreadFactory()
        {
            return _threadFactory ?? ThreadFactoryBuilder.BuildDefault();
        }

        private BlockingCollection<Task> GetOrDefaultQueue()
        {
            return _queue ?? new BlockingCollection<Task>(new ConcurrentQueue<Task>());
        }
    }
}
