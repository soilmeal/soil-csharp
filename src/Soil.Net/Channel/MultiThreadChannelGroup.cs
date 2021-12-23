using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Soil.Core.Threading;
using Soil.Net.Threading.Tasks;

namespace Soil.Net.Channel;

public class MultiThreadChannelGroup<TChannel> : IChannelGroup<TChannel>
    where TChannel : IChannel
{
    private readonly Dictionary<ulong, TChannel> _channels = new();

    private readonly MultiTaskSchedulerGroup _schedulerGroup;

    public MultiThreadChannelGroup(int maxConcurrencyLevel, IThreadFactory threadFactory)
    {
        _schedulerGroup = new MultiTaskSchedulerGroup(maxConcurrencyLevel, threadFactory);
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
        return Register(channelTask, CancellationToken.None);
    }

    public Task<bool> Register(Task<TChannel> channelTask, CancellationToken cancellationToken)
    {
        return channelTask != null
            ? channelTask.ContinueWith(
                (channelTask) => TryRegister(channelTask.Result),
                cancellationToken,
                TaskContinuationOptions.OnlyOnRanToCompletion,
                _schedulerGroup.NextScheduler())
            : throw new ArgumentNullException(nameof(channelTask));
    }

    private bool TryRegister(TChannel? channel)
    {
        return channel != null && _channels.TryAdd(channel.Id, channel);
    }
}

