namespace Soil.Buffers;

public interface IByteBufferAllocator
{
    int MaxCapacity { get; }

    IUnsafeOp Unsafe { get; }


    IByteBuffer Allocate(int capacityHint = Constants.DefaultCapacity, Endianless endianless = Endianless.BigEndian);

    CompositeByteBuffer CompositeByteBuffer(Endianless endianless = Endianless.BigEndian);

    public interface IUnsafeOp
    {
        byte[] Allocate(int capacityHint);

        byte[] Reallocate(byte[] oldBuffer);

        void Return(IByteBuffer byteBuffer, byte[] buffer);
    }
}
