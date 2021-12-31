namespace Soil.Threading;

public class ThreadNameFormatter
{
    private readonly string _threadNameFormat = string.Empty;

    public string ThreadNameFormat
    {
        get
        {
            return _threadNameFormat;
        }
    }

    public ThreadNameFormatter()
        : this("thread-{0}")
    {
    }

    public ThreadNameFormatter(string threadNameFormat)
    {
        _threadNameFormat = threadNameFormat;
    }

    public string Format(int threadId)
    {
        return string.Format(_threadNameFormat, threadId);
    }
}
