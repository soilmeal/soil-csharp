using System;

namespace Soil.Buffers;

public partial class UnpooledByteBufferAllocator : ByteBufferAllocator
{
    public class UnsafeOp : IByteBufferAllocator.IUnsafeOp
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

        public byte[] Reallocate(byte[] oldBuffer)
        {
            return oldBuffer != null
                ? Allocate(oldBuffer.Length + 1)
                : throw new ArgumentNullException(nameof(oldBuffer));
        }

        public void Return(IByteBuffer byteBuffer, byte[] buffer)
        {
            if (byteBuffer == null)
            {
                throw new ArgumentNullException(nameof(byteBuffer));
            }

            if (buffer == null)
            {
                throw new ArgumentNullException(nameof(buffer));
            }
        }
    }
}
