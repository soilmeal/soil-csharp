using System;
using System.Threading;

namespace Soil.Core.Threading;

public class ThreadFactoryBuilder
{
    private ThreadPriority _priority = ThreadPriority.Normal;
    public ThreadPriority Priority
    {
        get
        {
            return _priority;
        }
    }

    public ThreadFactoryBuilder() { }

    public ThreadFactoryBuilder SetPriority(ThreadPriority priority)
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
        var builder = new ThreadFactoryBuilder();
        return builder.SetPriority(ThreadPriority.Normal)
            .Build(ThreadNameFormatter.Default);
    }
}
