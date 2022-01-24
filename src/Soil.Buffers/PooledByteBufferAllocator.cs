using System.Buffers;
using Soil.Buffers.Helper;
using Soil.ObjectPool;
using Soil.ObjectPool.Concurrent;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator : IByteBufferAllocator
{
    public const int DefaultByteBufferRetainSize = 4 * 1024;

    public const int DefaultBufferArrayBucketSize = 4 * 1024;

    private readonly IObjectPool<PooledByteBuffer> _byteBufferPool;

    private readonly IObjectPool<CompositeByteBuffer> _compositeByteBufferPool;

    private readonly ArrayPool<byte> _bufferArrayPool;

    private readonly int _maxCapacity;

    private readonly UnsafeOp _unsafe;

    public int MaxCapacity
    {
        get
        {
            return _maxCapacity;
        }
    }

    public IByteBufferAllocator.IUnsafeOp Unsafe
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
        : this(Constants.DefaultMaxCapacity, bufferArrayBucketSize, byteBufferRetainSize)
    {
    }

    public PooledByteBufferAllocator(
        int maxCapacityHint,
        int bufferArrayBucketSize,
        int byteBufferRetainSize)
    {
        _maxCapacity = (int)BitOperationsHelper.RoundUpToPowerOf2((uint)maxCapacityHint);

        _unsafe = new UnsafeOp(this);
        _bufferArrayPool = ArrayPool<byte>.Create(_maxCapacity, bufferArrayBucketSize);

        IObjectPoolPolicy<PooledByteBuffer> bufferPoolPolicy = new BufferPoolPolicy(this);
        _byteBufferPool = byteBufferRetainSize > 0
            ? new TLSObjectPool<PooledByteBuffer>(bufferPoolPolicy, byteBufferRetainSize)
            : new TLSUnlimitedObjectPool<PooledByteBuffer>(bufferPoolPolicy);


        IObjectPoolPolicy<CompositeByteBuffer> compositionBufferPoolPolicy = new CompositeBufferPoolPolicy(this);
        _compositeByteBufferPool = byteBufferRetainSize > 0
            ? new TLSObjectPool<CompositeByteBuffer>(
                compositionBufferPoolPolicy,
                byteBufferRetainSize)
            : new TLSUnlimitedObjectPool<CompositeByteBuffer>(compositionBufferPoolPolicy);
    }

    public IByteBuffer Allocate(
        int capacityHint = Constants.DefaultCapacity,
        Endianless endianless = Endianless.BigEndian)
    {
        ByteBuffer byteBuffer = _byteBufferPool.Get();
        byteBuffer.Unsafe.Allocate(capacityHint, endianless);

        return byteBuffer;
    }

    public CompositeByteBuffer CompositeByteBuffer(Endianless endianless = Endianless.BigEndian)
    {
        CompositeByteBuffer byteBuffer = _compositeByteBufferPool.Get();
        byteBuffer.Unsafe.Allocate(Constants.CompositionByteBufferCapacityHint, endianless);

        return byteBuffer;
    }
}
