using System;

namespace Soil.Buffers;

public sealed class UnpooledByteBuffer : ByteBuffer, IDisposable
{
    public UnpooledByteBuffer(UnpooledByteBufferAllocator allocator)
        : base(allocator)
    {
    }

    public void Dispose()
    {
        Release();
        GC.SuppressFinalize(this);
    }
}
