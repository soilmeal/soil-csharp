using System;

namespace Soil.Net.Channel;

public interface IChannelReconnectStrategy
{
    public double TryReconnect(
        int currentReconnectCount,
        ChannelReconnectReason reason,
        Exception? cause);
}
