using System;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator
{
    public class UnsafeOp : IByteBufferAllocator.IUnsafeOp
    {
        private readonly PooledByteBufferAllocator _parent;

        public PooledByteBufferAllocator Parent
        {
            get
            {
                return _parent;
            }
        }

        public UnsafeOp(PooledByteBufferAllocator parent)
        {
            _parent = parent;
        }

        public byte[] Allocate(int capacityHint)
        {
            int newCapacity = BufferUtilities.ComputeActualCapacity(capacityHint);
            if (newCapacity <= 0)
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.MaxCapacityReached);
            }

            byte[] arr = _parent._bufferArrayPool.Rent(capacityHint);
            Array.Fill<byte>(arr, 0);

            return arr;
        }

        public byte[] Reallocate(byte[] oldBuffer, int addSizeHint = 0)
        {
            if (addSizeHint <= 0)
            {
                addSizeHint = 1;
            }

            byte[] newBuffer = Allocate(oldBuffer.Length + addSizeHint);

            Array.Copy(oldBuffer, newBuffer, oldBuffer.Length);
            Free(oldBuffer);
            return newBuffer;
        }

        public void Return(IByteBuffer byteBuffer, byte[] buffer)
        {
            switch (byteBuffer)
            {
                case PooledByteBuffer pooledByteBuffer:
                {
                    _parent._byteBufferPool.Return(pooledByteBuffer);
                    break;
                }
                case CompositeByteBuffer compositeByteBuffer:
                {
                    _parent._compositeByteBufferPool.Return(compositeByteBuffer);
                    break;
                }
                default:
                {
                    return;
                }
            }

            Free(buffer);
        }

        private void Free(byte[] buffer)
        {
            _parent._bufferArrayPool.Return(buffer);
        }
    }
}
