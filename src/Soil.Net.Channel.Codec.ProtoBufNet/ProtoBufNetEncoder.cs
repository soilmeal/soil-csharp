using System;
using ProtoBuf;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel.Codec.ProtoBufNet;

public class ProtoBufNetEncoder<T> : IChannelOutboundPipe<T, IByteBuffer>
    where T : class, IExtensible
{
    public Result<ChannelPipeResultType, IByteBuffer> Transform(
        IChannelHandlerContext ctx,
        T message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        IByteBuffer byteBuffer = ctx.Allocator.Allocate();
        Serializer.Serialize(byteBuffer.Unsafe.BufferWriter, message);
        return Result.Create(ChannelPipeResultType.CallNext, byteBuffer);
    }

    public IChannelOutboundPipe<T, TNewMessage> Connect<TNewMessage>(
        IChannelOutboundPipe<IByteBuffer, TNewMessage> other)
        where TNewMessage : class
    {
        return ((IChannelOutboundPipe<T, IByteBuffer>)this).Connect(other);
    }
}
