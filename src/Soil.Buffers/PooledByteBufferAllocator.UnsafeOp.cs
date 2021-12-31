namespace Soil.Buffers;

public partial class PooledByteBufferAllocator : ByteBufferAllocator
{
    public class UnsafeOp : IByteBufferAllocator.IUnsafeOp<ByteBuffer>
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
            return _parent._bufferPool.Rent(capacityHint);
        }

        public byte[] Reallocate(byte[] oldBuffer, int capacityHint)
        {
            _parent._bufferPool.Return(oldBuffer);
            return Allocate(capacityHint);
        }

        public void Release(ByteBuffer buffer)
        {
            _parent._bufferPool.Return(buffer.Unsafe.Release());
        }
    }
}
