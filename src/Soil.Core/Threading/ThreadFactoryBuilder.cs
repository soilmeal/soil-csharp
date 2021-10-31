using System.Threading;

namespace Soil.Core.Threading
{
    public class ThreadFactoryBuilder
    {
        private ThreadPriority _priority = ThreadPriority.Normal;
        public ThreadPriority Priority => _priority;

        public ThreadFactoryBuilder() { }

        public ThreadFactoryBuilder SetPriority(ThreadPriority priority_)
        {
            _priority = priority_;
            return this;
        }

        public IThreadFactory Build(string name_)
        {
            return new NameThreadFactory(_priority, name_);
        }

        public IThreadFactory Build(ThreadNameFormatter formatter_)
        {
            return new FormattedNameThreadFactory(_priority, formatter_);
        }
    }
}
