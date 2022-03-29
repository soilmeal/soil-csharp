using Soil.Buffers;
using Soil.Net.Event;

namespace Soil.Net.Channel;

public interface IChannelHandlerContext
{
    ChannelId Id { get; }

    IByteBufferAllocator Allocator { get; }

    IEventLoop EventLoop { get; }

    void RequestRead(IByteBuffer? byteBuffer = null);

    void Write(object message);

    void Close();
}
