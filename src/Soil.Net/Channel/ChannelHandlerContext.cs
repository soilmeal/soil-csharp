using Soil.Buffers;
using Soil.Net.Event;

namespace Soil.Net.Channel;

public class ChannelHandlerContext : IChannelHandlerContext
{
    private readonly IChannel _channel;

    public ChannelId Id
    {
        get
        {
            return _channel.Id;
        }
    }

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _channel.Allocator;
        }
    }

    public IEventLoop EventLoop
    {
        get
        {
            return _channel.EventLoop;
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
