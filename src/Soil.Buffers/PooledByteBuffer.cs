namespace Soil.Buffers;

public class PooledByteBuffer : ByteBuffer
{
    public PooledByteBuffer(PooledByteBufferAllocator allocator)
        : base(allocator)
    {
    }
}
