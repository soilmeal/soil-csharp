using System;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator : ByteBufferAllocator
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
            return _parent._bufferArrayPool.Rent(capacityHint);
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
            if (byteBuffer is not PooledByteBuffer pooledByteBuffer)
            {
                return;
            }

            _parent._byteBufferPool.Return(pooledByteBuffer);
            Free(buffer);
        }

        private void Free(byte[] buffer)
        {
            Array.Fill<byte>(buffer, 0);
            _parent._bufferArrayPool.Return(buffer);
        }
    }
}
