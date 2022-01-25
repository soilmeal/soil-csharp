using System;
using ProtoBuf;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel.Codec.ProtoBufNet;

public class ProtoBufNetDecoder<T> : IChannelInboundPipe<IByteBuffer, T>
    where T : class, IExtensible
{
    public Result<ChannelPipeResultType, T> Transform(
        IChannelHandlerContext ctx,
        IByteBuffer message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }


        T decoded = Serializer.Deserialize<T>(message.Unsafe.AsMemoryToSend());
        message.Release();
        return Result.Create(ChannelPipeResultType.CallNext, decoded);
    }

    public IChannelInboundPipe<IByteBuffer, TNewMessage> Connect<TNewMessage>(IChannelInboundPipe<T, TNewMessage> other)
        where TNewMessage : class
    {
        return ((IChannelInboundPipe<IByteBuffer, T>)this).Connect(other);
    }
}
