using System;

namespace Soil.Net.Channel;

public class ChannelMaxReconnectCountStrategy : IChannelReconnectStrategy
{
    private readonly int _maxReconnectCount;

    private readonly double _waitMillisecondsBeforeReconnect;

    public int MaxReconnectCount
    {
        get
        {
            return _maxReconnectCount;
        }
    }

    public double WaitMilliseconds
    {
        get
        {
            return _waitMillisecondsBeforeReconnect;
        }
    }

    public ChannelMaxReconnectCountStrategy(int maxReconnectCount)
        : this(maxReconnectCount, Constants.DefaultWaitMillisecondsBeforeReconnect)
    {
    }

    public ChannelMaxReconnectCountStrategy(int maxReconnectCount, double waitMilliseconds)
    {
        _maxReconnectCount = maxReconnectCount;
        _waitMillisecondsBeforeReconnect = waitMilliseconds;
    }

    public double TryReconnecct(
        int currentReconnectCount,
        ChannelReconnectReason reason,
        Exception? cause)
    {
        switch (reason)
        {
            case ChannelReconnectReason.InactivedByLocal:
            case ChannelReconnectReason.InactivedByRemote:
            {
                return 0.0;
            }
        }

        return currentReconnectCount <= _maxReconnectCount ? _waitMillisecondsBeforeReconnect : 0.0;
    }
}
