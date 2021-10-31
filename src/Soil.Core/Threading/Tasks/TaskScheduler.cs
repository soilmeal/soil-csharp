namespace Soil.Core.Threading.Tasks
{
    public abstract class TaskScheduler : System.Threading.Tasks.TaskScheduler
    {
        private readonly IThreadFactory _threadFactory;
        public IThreadFactory ThreadFactory { get => _threadFactory; }

        public TaskScheduler(IThreadFactory threadFactory)
        {
            _threadFactory = threadFactory;
        }
    }
}
