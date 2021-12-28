using System.Numerics;

namespace Soil.Core.Buffers;

public abstract class ByteBufferAllocator : IByteBufferAllocator<ByteBuffer>
{
    internal static readonly int MaxCapacity = (int)BitOperations.RoundUpToPowerOf2((uint)int.MaxValue >> 1);

    public abstract IByteBufferAllocator.IUnsafeOp<ByteBuffer> Unsafe { get; }

    public abstract ByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian);
}

