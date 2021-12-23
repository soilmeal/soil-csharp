using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soil.Core.Threading;
using Soil.Net.Threading.Tasks;

namespace Soil.Net.Channel;

public class SingleThreadChannelGroup<TChannel> : IChannelGroup<TChannel>
    where TChannel : IChannel
{
    private readonly Dictionary<ulong, TChannel> _channels = new();

    private readonly SingleTaskSchedulerGroup _schedulerGroup;

    public SingleThreadChannelGroup(IThreadFactory threadFactory)
    {
        _schedulerGroup = new SingleTaskSchedulerGroup(threadFactory);
    }

    public Task<bool> Register(TChannel? channel)
    {
        return Register(channel, CancellationToken.None);
    }

    public Task<bool> Register(TChannel? channel, CancellationToken cancellationToken)
    {
        return _schedulerGroup.StartNewOnNextScheduler(
            () => TryRegister(channel),
            cancellationToken);
    }

    public Task<bool> Register(Task<TChannel> channelTask)
    {
        throw new NotImplementedException();
    }

    public Task<bool> Register(Task<TChannel> channelTask, CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }

    private bool TryRegister(TChannel? channel)
    {
        return channel != null && _channels.TryAdd(channel.Id, channel);
    }
}
