using System;
using Google.Protobuf;
using Soil.Buffers;
using Soil.Types;

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

    public Result<ChannelPipeResultType, IByteBuffer> Transform(
        IChannelHandlerContext ctx,
        T message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        IByteBuffer byteBuffer = ctx.Allocator.Allocate();
        message.WriteTo(byteBuffer.Unsafe.BufferWriter);
        return Result.Create(ChannelPipeResultType.CallNext, byteBuffer);
    }
}
