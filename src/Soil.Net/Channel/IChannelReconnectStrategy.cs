using System;

namespace Soil.Net.Channel;

public interface IChannelReconnectStrategy
{
    public double TryReconnecct(
        int currentReconnectCount,
        ChannelReconnectReason reason,
        Exception? cause);
}
