namespace Soil.Buffers;

public interface IByteBufferAllocator
{
    IUnsafeOp Unsafe { get; }

    IByteBuffer Allocate(int capacityHint, Endianless endianless = Endianless.BigEndian);

    public interface IUnsafeOp
    {
        byte[] Allocate(int capacityHint);

        byte[] Reallocate(byte[] oldBuffer);

        void Return(IByteBuffer byteBuffer, byte[] buffer);
    }
}
