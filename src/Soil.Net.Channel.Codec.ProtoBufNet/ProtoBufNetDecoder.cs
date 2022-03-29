using System;
using System.Threading.Tasks;
using Google.Protobuf;
using Soil.Buffers;

namespace Soil.Net.Channel.Codec.ProtoBufNet;

public class ProtoBufNetDecoder<T> : IChannelInboundPipe<IByteBuffer, T>
    where T : class, IMessage<T>
{
    private readonly MessageParser<T> _parser;
    public MessageParser<T> Parser { get { return _parser; } }

    public ProtoBufNetDecoder(MessageParser<T> parser)
    {
        _parser = parser;
    }

    public IChannelInboundPipe<IByteBuffer, TNewMessage> Connect<TNewMessage>(IChannelInboundPipe<T, TNewMessage> other)
        where TNewMessage : class
    {
        return IChannelInboundPipe.Connect(this, other);
    }

    public Task<T?> TransformAsync(IChannelHandlerContext ctx, IByteBuffer message)
    {
        return ctx.EventLoop.StartNew(() => DoTransform(ctx, message));
    }

    private T? DoTransform(IChannelHandlerContext ctx, IByteBuffer message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        T decoded = _parser.ParseFrom(message.Unsafe.AsMemoryToSend().Span);
        message.Release();

        return decoded;
    }
}
