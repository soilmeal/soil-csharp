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
            int newCapacity = BufferUtilities.ComputeNextCapacity(capacityHint);
            return newCapacity > 0
                ? _parent._bufferArrayPool.Rent(capacityHint)
                : throw new InvalidBufferOperationException(InvalidBufferOperationException.MaxCapacityReached);
        }

        public byte[] Reallocate(byte[] oldBuffer)
        {
            byte[] newBuffer = Allocate(oldBuffer.Length + 1);
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
            Array.Fill<byte>(buffer, 0);
            _parent._bufferArrayPool.Return(buffer);
        }
    }
}
