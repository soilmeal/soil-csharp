using System.Buffers;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.ObjectPool;

namespace Soil.Core.Buffers;

public partial class PooledByteBufferAllocator : ByteBufferAllocator
{
    public const int DefaultByteBufferRetainSize = 4 * 1024;

    public const int DefaultBufferArrayBucketSize = 4 * 1024;

    private readonly ILogger<PooledByteBufferAllocator> _logger;

    private readonly DefaultObjectPool<PooledByteBuffer> _byteBufferPool;

    private readonly ArrayPool<byte> _bufferPool;

    private readonly UnsafeOp _unsafe;

    public override IByteBufferAllocator.IUnsafeOp<ByteBuffer> Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public PooledByteBufferAllocator(ILoggerFactory loggerFactory)
        : this(DefaultBufferArrayBucketSize, DefaultByteBufferRetainSize, loggerFactory)
    {
    }

    public PooledByteBufferAllocator(
        int bufferArrayBucketSize,
        int byteBufferRetainSize,
        ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<PooledByteBufferAllocator>();

        _unsafe = new UnsafeOp(this, loggerFactory);
        _byteBufferPool = new DefaultObjectPool<PooledByteBuffer>(
            new PooledObjectPolicy(this, loggerFactory),
            byteBufferRetainSize);
        _bufferPool = ArrayPool<byte>.Create(bufferArrayBucketSize, MaxCapacity);
    }

    public override ByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian)
    {
        PooledByteBuffer byteBuffer = _byteBufferPool.Get();
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }
}
