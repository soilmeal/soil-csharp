using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Soil.Buffers;

namespace Soil.Net.Channel.Codec.ProtoBufNet;

public class ProtoBufNetEncoder<T> : IChannelOutboundPipe<T, IByteBuffer>
    where T : class, IMessage<T>
{
    public IChannelOutboundPipe<T, TNewMessage> Connect<TNewMessage>(
        IChannelOutboundPipe<IByteBuffer, TNewMessage> other)
        where TNewMessage : class
    {
        return IChannelOutboundPipe.Connect(this, other);
    }

    public Task<IByteBuffer> TransformAsync(
        IChannelHandlerContext ctx,
        T message)
    {
        return ctx.EventLoop.StartNew(() => DoTransform(ctx, message));
    }

    private IByteBuffer DoTransform(IChannelHandlerContext ctx, T message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        IByteBuffer byteBuffer = ctx.Allocator.Allocate();
        message.WriteTo(byteBuffer.Unsafe.BufferWriter);

        return byteBuffer;
    }
}
