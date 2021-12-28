using System.Buffers;
using Microsoft.Extensions.ObjectPool;

namespace Soil.Core.Buffers;

public partial class PooledByteBufferAllocator : ByteBufferAllocator
{
    public const int DefaultByteBufferRetainSize = 4 * 1024;

    public const int DefaultBufferArrayBucketSize = 4 * 1024;

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

    public PooledByteBufferAllocator()
        : this(DefaultBufferArrayBucketSize, DefaultByteBufferRetainSize)
    {
    }

    public PooledByteBufferAllocator(int bufferArrayBucketSize, int byteBufferRetainSize)
    {
        _unsafe = new UnsafeOp(this);
        _byteBufferPool = new DefaultObjectPool<PooledByteBuffer>(
            new PooledObjectPolicy(this),
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
