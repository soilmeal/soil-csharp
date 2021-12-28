using System;
using System.Threading.Tasks;
using Soil.Net.Channel;

namespace Soil.Net.Event;

public class SingleThreadEventLoop<TChannel> : IEventLoop
    where TChannel : IChannel

{
    private readonly bool _initSynchronizationContextAlways = false;
    public bool InitSynchronizationContextAlways
    {
        get
        {
            return _initSynchronizationContextAlways;
        }
    }

    private readonly TaskFactory _taskFactory;
    public TaskFactory TaskFactory
    {
        get
        {
            return _taskFactory;
        }
    }

    public SingleThreadEventLoop()
    {

    }

    public Task RegisterAsync(IChannel channel)
    {
        return channel != null
            ? channel.EventSource.RegisterAsync(this)
            : throw new ArgumentNullException(nameof(channel));
    }

    public void Register(IChannel channel)
    {
        RegisterAsync(channel)
            .GetAwaiter()
            .GetResult();
    }
}
