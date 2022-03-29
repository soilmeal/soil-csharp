using System;
using System.Threading.Tasks;
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

    public Task<Unit?> HandleReadAsync(IChannelHandlerContext ctx, IByteBuffer byteBuffer)
    {
        return Constants.DefaultInboundPipe.TransformAsync(ctx, byteBuffer);
    }

    public Task<IByteBuffer> HandleWriteAsync(IChannelHandlerContext ctx, IByteBuffer message)
    {
        return Constants.DefaultOutboundPipe.TransformAsync(ctx, message);
    }
}
