namespace Soil.Buffers;

public partial class UnpooledByteBufferAllocator : ByteBufferAllocator
{
    public class UnsafeOp : IByteBufferAllocator.IUnsafeOp<ByteBuffer>
    {
        private readonly UnpooledByteBufferAllocator _parent;

        public UnpooledByteBufferAllocator Parent
        {
            get
            {
                return _parent;
            }
        }

        public UnsafeOp(UnpooledByteBufferAllocator parent)
        {
            _parent = parent;
        }

        public byte[] Allocate(int capacityHint)
        {
            int newCapacity = BufferUtilities.ComputeNextCapacity(capacityHint);
            return newCapacity > 0
                ? new byte[newCapacity]
                : throw new InvalidBufferOperationException(InvalidBufferOperationException.MaxCapacityReached);
        }

        public byte[] Reallocate(byte[] oldBuffer, int capacityHint)
        {
            return oldBuffer.Length < MaxCapacity
                ? Allocate(capacityHint)
                : throw new InvalidBufferOperationException(InvalidBufferOperationException.MaxCapacityReached);
        }

        public void Release(ByteBuffer buffer)
        {
            _ = buffer.Unsafe.Release();
        }
    }
}
