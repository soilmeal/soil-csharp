namespace Soil.Core.Buffers;

public class PooledByteBuffer : ByteBuffer
{
    public PooledByteBuffer(ByteBufferAllocator allocator)
        : base(allocator)
    {
    }
}
