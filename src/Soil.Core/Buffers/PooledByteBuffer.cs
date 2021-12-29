using Microsoft.Extensions.Logging;

namespace Soil.Core.Buffers;

public class PooledByteBuffer : ByteBuffer
{
    private readonly ILogger<PooledByteBuffer> _logger;

    public PooledByteBuffer(ByteBufferAllocator allocator, ILoggerFactory loggerFactory)
        : base(allocator, loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PooledByteBuffer>();
    }
}
