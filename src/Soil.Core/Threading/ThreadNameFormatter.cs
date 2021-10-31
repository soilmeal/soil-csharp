namespace Soil.Core.Threading
{
    public class ThreadNameFormatter
    {
        public static readonly ThreadNameFormatter Default = new();

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
}
