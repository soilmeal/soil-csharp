using System;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

public interface IChannelPipeline
{
    Result<ChannelPipeResultType, Unit> HandleRead(
        IChannelHandlerContext ctx,
        IByteBuffer byteBuffer);

    Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        object message);

    public static IChannelPipeline<IByteBuffer> Create(IChannelInboundPipe<IByteBuffer, Unit> inboundPipe)
    {
        return new ChannelPipeline<IByteBuffer>(inboundPipe, Constants.DefaultOutboundPipe);
    }

    public static IChannelPipeline<TOutMsg> Create<TOutMsg>(
        IChannelInboundPipe<IByteBuffer, Unit> inboundPipe,
        IChannelOutboundPipe<TOutMsg, IByteBuffer> outboundPipe)
        where TOutMsg : class
    {
        return new ChannelPipeline<TOutMsg>(inboundPipe, outboundPipe);
    }
}

public interface IChannelPipeline<TOutMsg> : IChannelPipeline
    where TOutMsg : class
{
    Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        TOutMsg message);

    Result<ChannelPipeResultType, IByteBuffer> IChannelPipeline.HandleWrite(
        IChannelHandlerContext ctx,
        object message)
    {
        if (message is not TOutMsg castedMessage)
        {
            throw new ArgumentException($"not supported type - typename={message.GetType().FullName}");
        }

        return HandleWrite(ctx, castedMessage);
    }
}
