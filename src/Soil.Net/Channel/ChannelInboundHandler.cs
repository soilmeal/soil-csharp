using System;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

public abstract class ChannelInboundHandler
{
    public static ChannelInboundHandler<TInMsg> Create<TInMsg>(
        Action<IChannelHandlerContext, TInMsg> action)
        where TInMsg : class
    {
        return new ActionWrapper<TInMsg>(action);
    }

    private class ActionWrapper<TInMsg> : ChannelInboundHandler<TInMsg>
        where TInMsg : class
    {
        private readonly Action<IChannelHandlerContext, TInMsg> _action;

        public ActionWrapper(Action<IChannelHandlerContext, TInMsg> action)
        {
            _action = action;
        }

        protected override void HandleReadComplete(IChannelHandlerContext ctx, TInMsg message)
        {
            _action.Invoke(ctx, message);
        }
    }
}


public abstract class ChannelInboundHandler<TInMsg> : IChannelInboundPipe<TInMsg, Unit>
    where TInMsg : class
{
    public Result<ChannelPipeResultType, Unit> Transform(
        IChannelHandlerContext ctx,
        TInMsg message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        HandleReadComplete(ctx, message);

        if (message is IByteBuffer byteBuffer && byteBuffer.IsInitialized)
        {
            byteBuffer.Release();
        }

        return UnitResult.Create(ChannelPipeResultType.Completed);
    }

    protected abstract void HandleReadComplete(IChannelHandlerContext ctx, TInMsg message);
}

