using Soil.Buffers.Helper;

namespace Soil.Buffers;

public abstract class ByteBufferAllocator : IByteBufferAllocator
{
    internal static readonly int MaxCapacity = (int)BitOperationsHelper.RoundUpToPowerOf2((uint)int.MaxValue >> 1);

    public abstract IByteBufferAllocator.IUnsafeOp Unsafe { get; }

    public abstract IByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian);
}

