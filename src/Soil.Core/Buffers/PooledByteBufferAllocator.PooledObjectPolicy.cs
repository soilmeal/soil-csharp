using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Soil.Core.Buffers;

public partial class PooledByteBufferAllocator
{
    public class PooledObjectPolicy : IPooledObjectPolicy<PooledByteBuffer>
    {
        private readonly ILogger<PooledObjectPolicy> _logger;

        private readonly ILoggerFactory _loggerFactory;

        private readonly PooledByteBufferAllocator _parent;

        public PooledObjectPolicy(PooledByteBufferAllocator parent, ILoggerFactory loggerFactory)
        {
            _logger = loggerFactory.CreateLogger<PooledObjectPolicy>();
            _loggerFactory = loggerFactory;

            _parent = parent;
        }

        public PooledByteBuffer Create()
        {
            return new PooledByteBuffer(_parent, _loggerFactory);
        }

        public bool Return(PooledByteBuffer obj)
        {
            return true;
        }
    }
}
