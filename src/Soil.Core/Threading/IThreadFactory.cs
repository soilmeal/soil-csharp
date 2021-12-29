using System;
using System.Threading;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Threading;

public interface IThreadFactory
{
    ThreadPriority Priority { get; }

    Thread Create(ThreadStart start);

    Thread Create(ThreadStart start, bool backgound);

    Thread Create(ParameterizedThreadStart start);

    Thread Create(ParameterizedThreadStart start, bool backgound);

    public class Builder
    {
        private readonly ILogger<Builder> _logger;

        private readonly ILoggerFactory _loggerFactory;

        private ThreadPriority _priority = ThreadPriority.Normal;


        public ThreadPriority Priority
        {
            get
            {
                return _priority;
            }
        }

        public Builder(ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<Builder>();
            _loggerFactory = loggerFactory;
        }

        public Builder SetPriority(ThreadPriority priority)
        {
            _priority = priority;
            return this;
        }

        public IThreadFactory Build(string name)
        {
            return !string.IsNullOrEmpty(name)
                ? new NameThreadFactory(_priority, name, _loggerFactory)
                : throw new ArgumentNullException(nameof(name));
        }

        public IThreadFactory Build(ThreadNameFormatter formatter)
        {
            return formatter != null
                ? new FormattedNameThreadFactory(_priority, formatter, _loggerFactory)
                : throw new ArgumentNullException(nameof(formatter));
        }

        public static IThreadFactory BuildDefault(ILoggerFactory loggerFactory)
        {
            var builder = new Builder(loggerFactory);
            return builder.SetPriority(ThreadPriority.Normal)
                .Build(new ThreadNameFormatter(loggerFactory));
        }
    }
}
