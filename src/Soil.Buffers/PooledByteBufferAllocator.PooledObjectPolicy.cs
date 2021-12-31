using Soil.ObjectPool;

namespace Soil.Buffers;

public partial class PooledByteBufferAllocator
{
    public class PooledObjectPolicy : IObjectPoolPolicy<PooledByteBuffer>
    {
        private readonly PooledByteBufferAllocator _parent;

        public PooledObjectPolicy(PooledByteBufferAllocator parent)
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
}
