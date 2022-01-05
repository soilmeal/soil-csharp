using Soil.Threading;

namespace Soil.Net.Event;

public class SingleThreadEventLoopGroup : SingleThreadEventLoop, IEventLoopGroup
{
    public SingleThreadEventLoopGroup(IThreadFactory threadFactory)
        : base(threadFactory)
    {
    }

    public SingleThreadEventLoopGroup(
        IThreadFactory threadFactory,
        bool initSynchronizationContextAlways)
        : base(threadFactory, initSynchronizationContextAlways)
    {
    }

    public IEventLoop Next()
    {
        return this;
    }
}
