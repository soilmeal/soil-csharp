using System;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Buffers;

public sealed class UnpooledByteBuffer : ByteBuffer, IDisposable
{
    private readonly ILogger<UnpooledByteBuffer> _logger;

    public UnpooledByteBuffer(UnpooledByteBufferAllocator allocator, ILoggerFactory loggerFactory)
        : base(allocator, loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UnpooledByteBuffer>();
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}
