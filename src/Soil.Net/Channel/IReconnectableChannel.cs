namespace Soil.Net.Channel;

public interface IReconnectableChannel : IChannel
{
    IChannelReconnectStrategy? ReconnectStrategy { get; }

    IChannelReconnectHandler? ReconnectHandler { get; }
}
