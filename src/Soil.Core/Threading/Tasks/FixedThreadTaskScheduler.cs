using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace Soil.Core.Threading.Tasks
{
    public class FixedThreadTaskScheduler : TaskScheduler
    {
        private readonly int _maximumCuncurrencyLevel;
        public override int MaximumConcurrencyLevel => _maximumCuncurrencyLevel;

        private readonly BlockingCollection<Task> _tasks;

        private readonly Thread[] _threads;

        public FixedThreadTaskScheduler(IThreadFactory threadFactory_, int maximumConcurrencyLevel_) : base(threadFactory_)
        {
            _maximumCuncurrencyLevel = maximumConcurrencyLevel_;

            _tasks = new BlockingCollection<Task>();

            _threads = Enumerable.Range(0, maximumConcurrencyLevel_)
                .Select((i) => ThreadFactory.Create(new ThreadStart(Execute)))
                .ToArray();
            foreach (var thread in _threads)
            {
                thread.Start();
            }
        }

        protected override IEnumerable<Task>? GetScheduledTasks()
        {
            throw new NotImplementedException();
        }

        protected override void QueueTask(Task task)
        {
            throw new NotImplementedException();
        }

        protected override bool TryExecuteTaskInline(Task task, bool taskWasPreviouslyQueued)
        {
            return false;
        }

        protected override bool TryDequeue(Task task)
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
}
