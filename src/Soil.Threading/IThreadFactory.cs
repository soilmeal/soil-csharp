using System;
using System.Threading;

namespace Soil.Threading;

public interface IThreadFactory
{
    ThreadPriority Priority { get; }

    Thread Create(ThreadStart start);

    Thread Create(ThreadStart start, bool backgound);

    Thread Create(ParameterizedThreadStart start);

    Thread Create(ParameterizedThreadStart start, bool backgound);

    public class Builder
    {
        private ThreadPriority _priority = ThreadPriority.Normal;


        public ThreadPriority Priority
        {
            get
            {
                return _priority;
            }
        }

        public Builder SetPriority(ThreadPriority priority)
        {
            _priority = priority;
            return this;
        }

        public IThreadFactory Build(string name)
        {
            return !string.IsNullOrEmpty(name)
                ? new NameThreadFactory(_priority, name)
                : throw new ArgumentNullException(nameof(name));
        }

        public IThreadFactory Build(ThreadNameFormatter formatter)
        {
            return formatter != null
                ? new FormattedNameThreadFactory(_priority, formatter)
                : throw new ArgumentNullException(nameof(formatter));
        }

        public static IThreadFactory BuildDefault()
        {
            var builder = new Builder();
            return builder.SetPriority(ThreadPriority.Normal)
                .Build(new ThreadNameFormatter());
        }
    }
}
