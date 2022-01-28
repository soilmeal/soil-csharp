namespace Soil.Net.Channel;

public interface IRetryableChannel : IChannel
{
    IChannelRetryStrategy? RetryStrategy { get; }
}
