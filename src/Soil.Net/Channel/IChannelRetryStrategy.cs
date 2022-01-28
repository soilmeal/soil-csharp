namespace Soil.Net.Channel;

public interface IChannelRetryStrategy
{
    public double HandleRetry(int currentRetryCount);
}
