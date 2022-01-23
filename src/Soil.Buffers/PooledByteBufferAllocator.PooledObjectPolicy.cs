using Soil.ObjectPool;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator
{
    public class BufferPoolPolicy : IObjectPoolPolicy<PooledByteBuffer>
    {
        private readonly PooledByteBufferAllocator _parent;

        public BufferPoolPolicy(PooledByteBufferAllocator parent)
        {
            _parent = parent;
        }

        public PooledByteBuffer Create()
        {
            return new PooledByteBuffer(_parent);
        }

        public bool Return(PooledByteBuffer obj)
        {
            return true;
        }
    }

    public class CompositeBufferPoolPolicy : IObjectPoolPolicy<CompositeByteBuffer>
    {
        private readonly PooledByteBufferAllocator _parent;

        public CompositeBufferPoolPolicy(PooledByteBufferAllocator parent)
        {
            _parent = parent;
        }

        public CompositeByteBuffer Create()
        {
            return new CompositeByteBuffer(_parent);
        }

        public bool Return(CompositeByteBuffer obj)
        {
            return true;
        }
    }
}
