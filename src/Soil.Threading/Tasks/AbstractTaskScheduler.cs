using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;

namespace Soil.Threading.Tasks;

public abstract class AbstractTaskScheduler : TaskScheduler, IDisposable
{
    public abstract IThreadFactory ThreadFactory { get; }

    public abstract void Dispose();

    protected abstract void Dispose(bool disposing);

    public class Builder
    {
        private int _maximumConcurrencyLevel = 0;

        private IThreadFactory? _threadFactory;

        private BlockingCollection<Task>? _queue;

        public int MaximumConcurrencyLevel
        {
            get
            {
                return _maximumConcurrencyLevel;
            }
        }

        public IThreadFactory? ThreadFactory
        {
            get
            {
                return _threadFactory;
            }
        }

        public BlockingCollection<Task>? Queue
        {
            get
            {
                return _queue;
            }
        }

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

        public AbstractTaskScheduler Build()
        {
            return new FixedThreadTaskScheduler(
                GetOrDefaultMaximumConcurrencyLevel(),
                GetOrDefaultThreadFactory(),
                GetOrDefaultQueue());
        }

        public static AbstractTaskScheduler BuildSingleThread()
        {
            var builder = new Builder();
            return builder.SetMaximumConcurrencyLevel(1)
                .SetThreadFactory(IThreadFactory.Builder.BuildDefault())
                .SetQueue(new BlockingCollection<Task>(new ConcurrentQueue<Task>()))
                .Build();
        }

        public static AbstractTaskScheduler BuildWorkStealing(
            int maximumConcurrencyLevel)
        {
            if (maximumConcurrencyLevel == 1)
            {
                throw new ArgumentOutOfRangeException(nameof(maximumConcurrencyLevel), maximumConcurrencyLevel, "maximumConcurrencyLevel of BuildWorkStealing() is must not be 1");
            }

            var builder = new Builder();
            return builder.SetMaximumConcurrencyLevel(maximumConcurrencyLevel)
                .SetThreadFactory(IThreadFactory.Builder.BuildDefault())
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
            return _threadFactory ?? IThreadFactory.Builder.BuildDefault();
        }

        private BlockingCollection<Task> GetOrDefaultQueue()
        {
            return _queue ?? new BlockingCollection<Task>(new ConcurrentQueue<Task>());
        }
    }
}
