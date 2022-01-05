using Soil.Buffers;

namespace Soil.Net.Channel;

public class ChannelHandlerContext : IChannelHandlerContext
{
    private readonly IChannel _channel;

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _channel.Allocator;
        }
    }

    public ChannelHandlerContext(IChannel channel)
    {
        _channel = channel;
    }

    public void Close()
    {
        _channel.CloseAsync();
    }

    public void RequestRead(IByteBuffer? byteBuffer = null)
    {
        _channel.RequestRead(byteBuffer);
    }

    public void Write(object message)
    {
        _channel.WriteAsync(message);
    }
}
