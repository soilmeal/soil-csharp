using System;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

internal class ChannelHandlerSet<TOutMsg> : IChannelHandlerSet<TOutMsg>
    where TOutMsg : class
{
    private readonly IChannelLifecycleHandler _lifecycleHandler;

    private readonly IChannelExceptionHandler _exceptionHandler;

    private readonly IChannelInboundPipe<IByteBuffer, Unit> _inboundPipe;

    private readonly IChannelOutboundPipe<TOutMsg, IByteBuffer> _outboundPipe;

    internal ChannelHandlerSet(
        IChannelLifecycleHandler lifecycleHandler,
        IChannelExceptionHandler exceptionHandler,
        IChannelInboundPipe<IByteBuffer, Unit> inboundPipe,
        IChannelOutboundPipe<TOutMsg, IByteBuffer> outboundPipe)
    {
        _exceptionHandler = exceptionHandler;
        _lifecycleHandler = lifecycleHandler;
        _inboundPipe = inboundPipe;
        _outboundPipe = outboundPipe;
    }

    public void HandleChannelActive(IChannel channel)
    {
        _lifecycleHandler.HandleChannelActive(channel);
    }

    public void HandleChannelInactive(
        IChannel channel,
        ChannelInactiveReason reason,
        Exception? cause)
    {
        _lifecycleHandler.HandleChannelInactive(channel, reason, cause);
    }

    public void HandleException(IChannelHandlerContext ctx, Exception ex)
    {
        _exceptionHandler.HandleException(ctx, ex);
    }

    public Result<ChannelPipeResultType, Unit> HandleRead(
        IChannelHandlerContext ctx,
        IByteBuffer byteBuffer)
    {
        return _inboundPipe.Transform(ctx, byteBuffer);
    }

    public Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        TOutMsg message)
    {
        return _outboundPipe.Transform(ctx, message);
    }

    public Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        object message)
    {
        return ((IChannelHandlerSet<TOutMsg>)this).HandleWrite(ctx, (TOutMsg)message);
    }
}
