using System;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

public class DefaultChannelHandlerSet : IChannelHandlerSet<IByteBuffer>
{
    public static readonly DefaultChannelHandlerSet Instance = new();

    public void HandleChannelActive(IChannel channel)
    {
        Constants.DefaultLifecycleHandler.HandleChannelActive(channel);
    }


    public void HandleChannelInactive(
        IChannel channel,
        ChannelInactiveReason reason,
        Exception? cause)
    {
        Constants.DefaultLifecycleHandler.HandleChannelInactive(channel, reason, cause);
    }

    public void HandleException(IChannelHandlerContext ctx, Exception ex)
    {
        throw new NotImplementedException();
    }

    public Result<ChannelPipeResultType, Unit> HandleRead(
        IChannelHandlerContext ctx,
        IByteBuffer byteBuffer)
    {
        return Constants.DefaultInboundPipe.Transform(ctx, byteBuffer);
    }

    public Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        IByteBuffer message)
    {
        return Constants.DefaultOutboundPipe.Transform(ctx, message);
    }
}
