using Soil.Buffers;

namespace Soil.Net.Channel;

public interface IChannelHandlerContext
{
    ChannelId Id { get; }

    IByteBufferAllocator Allocator { get; }

    void RequestRead(IByteBuffer? byteBuffer = null);

    void Write(object message);

    void Close();
}
