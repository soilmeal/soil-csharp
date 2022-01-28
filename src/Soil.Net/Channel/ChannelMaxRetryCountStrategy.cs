namespace Soil.Net.Channel;

public class ChannelMaxRetryCountStrategy : IChannelRetryStrategy
{
    private readonly int _maxRetryCount;

    private readonly double _waitMillisecondsBeforeRetry;

    public int MaxRetryCount
    {
        get
        {
            return _maxRetryCount;
        }
    }

    public double WaitMilliseconds
    {
        get
        {
            return _waitMillisecondsBeforeRetry;
        }
    }

    public ChannelMaxRetryCountStrategy(int maxRetryCount)
        : this(maxRetryCount, Constants.DefaultWaitMillisecondsBeforeRetry)
    {
    }

    public ChannelMaxRetryCountStrategy(int maxRetryCount, double waitMilliseconds)
    {
        _maxRetryCount = maxRetryCount;
        _waitMillisecondsBeforeRetry = waitMilliseconds;
    }

    public double HandleRetry(int currentRetryCount)
    {
        return currentRetryCount < _maxRetryCount ? _waitMillisecondsBeforeRetry : 0.0;
    }
}
