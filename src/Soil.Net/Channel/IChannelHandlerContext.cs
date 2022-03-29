using Soil.Buffers;

namespace Soil.Net.Channel;

public interface IChannelHandlerContext
{
    ChannelId Id { get; }

    IByteBufferAllocator Allocator { get; }

    void RequestRead();

    void Write(object message);

    void Close();
}
