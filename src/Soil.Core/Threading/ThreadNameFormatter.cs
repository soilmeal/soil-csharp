using Microsoft.Extensions.Logging;

namespace Soil.Core.Threading;

public class ThreadNameFormatter
{
    private readonly ILogger<ThreadNameFormatter> _logger;

    private readonly string _threadNameFormat = string.Empty;

    public string ThreadNameFormat
    {
        get
        {
            return _threadNameFormat;
        }
    }

    public ThreadNameFormatter(ILoggerFactory loggerFactory)
        : this("thread-{0}", loggerFactory)
    {
    }

    public ThreadNameFormatter(string threadNameFormat, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ThreadNameFormatter>();

        _threadNameFormat = threadNameFormat;
    }

    public string Format(int threadId)
    {
        return string.Format(_threadNameFormat, threadId);
    }
}
