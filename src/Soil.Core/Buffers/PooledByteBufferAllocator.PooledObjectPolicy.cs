using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.ObjectPool;

namespace Soil.Core.Buffers;

public partial class PooledByteBufferAllocator
{
    public class PooledObjectPolicy : IPooledObjectPolicy<PooledByteBuffer>
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
