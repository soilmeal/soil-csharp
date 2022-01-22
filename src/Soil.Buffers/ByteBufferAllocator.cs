using Soil.Buffers.Helper;

namespace Soil.Buffers;

public abstract class ByteBufferAllocator : IByteBufferAllocator
{
    public abstract IByteBufferAllocator.IUnsafeOp Unsafe { get; }

    public abstract IByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian);
}

