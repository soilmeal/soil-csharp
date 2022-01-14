using System.Buffers;
using Soil.ObjectPool;
using Soil.ObjectPool.Concurrent;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator : ByteBufferAllocator
{
    public const int DefaultByteBufferRetainSize = 4 * 1024;

    public const int DefaultBufferArrayBucketSize = 4 * 1024;

    private readonly IObjectPool<PooledByteBuffer> _byteBufferPool;

    private readonly ArrayPool<byte> _bufferArrayPool;

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
        _bufferArrayPool = ArrayPool<byte>.Create(MaxCapacity, bufferArrayBucketSize);

        IObjectPoolPolicy<PooledByteBuffer> policy = new PooledObjectPolicy(this);
        _byteBufferPool = byteBufferRetainSize > 0
            ? new TLSObjectPool<PooledByteBuffer>(policy, byteBufferRetainSize)
            : new TLSUnlimitedObjectPool<PooledByteBuffer>(policy);
    }

    public override IByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian)
    {
        ByteBuffer byteBuffer = _byteBufferPool.Get();
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }
}
