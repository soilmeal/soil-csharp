using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;

namespace Soil.Core.Threading.Tasks
{
    public class SingleThreadedTaskScheduler : TaskScheduler
    {
        private readonly BlockingCollection<Task> _tasks;

        private readonly Thread _thread;

        public override int MaximumConcurrencyLevel => 1;

        public SingleThreadedTaskScheduler(IThreadFactory threadFactory_) : base(threadFactory_)
        {
            _tasks = new();
            _thread = ThreadFactory.Create(new ThreadStart(Execute));
            _thread.Start();
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
}
