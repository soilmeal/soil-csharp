using Microsoft.Extensions.Logging;

namespace Soil.Core.Buffers;

public partial class UnpooledByteBufferAllocator : ByteBufferAllocator
{
    private readonly ILogger<UnpooledByteBufferAllocator> _logger;

    private readonly ILoggerFactory _loggerFactory;

    private readonly UnsafeOp _unsafe;

    public override IByteBufferAllocator.IUnsafeOp<ByteBuffer> Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public UnpooledByteBufferAllocator(ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<UnpooledByteBufferAllocator>();
        _loggerFactory = loggerFactory;

        _unsafe = new UnsafeOp(this, loggerFactory);
    }

    public override ByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian)
    {
        var byteBuffer = new UnpooledByteBuffer(this, _loggerFactory);
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }
}
