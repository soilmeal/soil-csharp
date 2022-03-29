using System;
using System.Threading.Tasks;
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

        protected override Task HandleReadComplete(IChannelHandlerContext ctx, TInMsg message)
        {
            return ctx.EventLoop.StartNew(() => _action.Invoke(ctx, message));
        }
    }
}


public abstract class ChannelInboundHandler<TInMsg> : IChannelInboundPipe<TInMsg, Unit>
    where TInMsg : class
{
    public async Task<Unit?> TransformAsync(IChannelHandlerContext ctx, TInMsg message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        await HandleReadComplete(ctx, message);

        return Unit.Instance;
    }

    protected abstract Task HandleReadComplete(IChannelHandlerContext ctx, TInMsg message);
}

