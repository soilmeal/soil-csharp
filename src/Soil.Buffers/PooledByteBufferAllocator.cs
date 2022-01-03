using System.Buffers;
using Soil.ObjectPool;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator : ByteBufferAllocator
{
    public const int DefaultByteBufferRetainSize = 4 * 1024;

    public const int DefaultBufferArrayBucketSize = 4 * 1024;

    private readonly LimitedObjectPool<PooledByteBuffer> _byteBufferPool;

    private readonly ArrayPool<byte> _bufferPool;

    private readonly UnsafeOp _unsafe;

    public override IByteBufferAllocator.IUnsafeOp Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public PooledByteBufferAllocator()
        : this(DefaultBufferArrayBucketSize, DefaultByteBufferRetainSize)
    {
    }

    public PooledByteBufferAllocator(
        int bufferArrayBucketSize,
        int byteBufferRetainSize)
    {
        _unsafe = new UnsafeOp(this);
        _byteBufferPool = new LimitedObjectPool<PooledByteBuffer>(
            new PooledObjectPolicy(this),
            byteBufferRetainSize);
        _bufferPool = ArrayPool<byte>.Create(bufferArrayBucketSize, MaxCapacity);
    }

    public override IByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian)
    {
        ByteBuffer byteBuffer = _byteBufferPool.Get();
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }
}
