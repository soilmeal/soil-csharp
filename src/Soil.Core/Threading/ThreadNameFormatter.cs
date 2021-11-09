namespace Soil.Core.Threading;

public class ThreadNameFormatter
{
    private static readonly ThreadNameFormatter _default = new();
    public static ThreadNameFormatter Default => _default;

    private readonly string _threadNameFormat = string.Empty;
    public string ThreadNameFormat => _threadNameFormat;

    private ThreadNameFormatter() : this("thread-{0}") { }

    public ThreadNameFormatter(string threadNameFormat_)
    {
        _threadNameFormat = threadNameFormat_;
    }

    public string Format(int threadId)
    {
        return string.Format(_threadNameFormat, threadId);
    }
}
