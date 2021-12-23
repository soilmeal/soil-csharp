using System.Threading;

namespace Soil.Core.Threading;

public interface IThreadFactory
{
    ThreadPriority Priority { get; }

    Thread Create(ThreadStart start);

    Thread Create(ParameterizedThreadStart start);
}
