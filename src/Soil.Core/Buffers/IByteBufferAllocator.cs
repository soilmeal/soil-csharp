using System;

namespace Soil.Core.Buffers;

public interface IByteBufferAllocator<TByteBuffer>
    where TByteBuffer : IByteBuffer<TByteBuffer>
{
    IByteBufferAllocator.IUnsafeOp<TByteBuffer> Unsafe { get; }

    TByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian);
}

public interface IByteBufferAllocator
{
    public interface IUnsafeOp<TByteBuffer>
        where TByteBuffer : IByteBuffer<TByteBuffer>
    {
        byte[] Allocate(int capacityHint);

        byte[] Reallocate(byte[] oldBuffer, int capacityHint);

        void Release(TByteBuffer buffer);
    }
}
