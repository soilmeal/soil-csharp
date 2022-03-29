using System;
using System.Threading.Tasks;

namespace Soil.Net.Channel;

public interface IChannelInboundPipe
{
    Task<object?> TransformAsync(IChannelHandlerContext ctx, object message);

    public static IChannelInboundPipe<TInMsg1, TInMsg2> Create<TInMsg1, TInMsg2>(
        Func<IChannelHandlerContext, TInMsg1, TInMsg2?> func)
        where TInMsg1 : class
        where TInMsg2 : class
    {
        return new FuncWrapper<TInMsg1, TInMsg2>(func);
    }

    public static IChannelInboundPipe<TInMsg1, TInMsg3> Connect<TInMsg1, TInMsg2, TInMsg3>(
        IChannelInboundPipe<TInMsg1, TInMsg2> first,
        IChannelInboundPipe<TInMsg2, TInMsg3> second)
        where TInMsg1 : class
        where TInMsg2 : class
        where TInMsg3 : class
    {
        return new Connector<TInMsg1, TInMsg2, TInMsg3>(first, second);
    }

    private class Connector<TInMsg1, TInMsg2, TInMsg3> : IChannelInboundPipe<TInMsg1, TInMsg3>
        where TInMsg1 : class
        where TInMsg2 : class
        where TInMsg3 : class
    {
        private readonly IChannelInboundPipe<TInMsg1, TInMsg2> _first;

        private readonly IChannelInboundPipe<TInMsg2, TInMsg3> _second;

        public Connector(
            IChannelInboundPipe<TInMsg1, TInMsg2> first,
            IChannelInboundPipe<TInMsg2, TInMsg3> second)
        {
            _first = first;
            _second = second;
        }

        public Task<object?> TransformAsync(IChannelHandlerContext ctx, object message)
        {
            return ((IChannelInboundPipe<TInMsg1, TInMsg3>)this).TransformAsync(
                ctx,
                (TInMsg3)message);
        }

        public async Task<TInMsg3?> TransformAsync(IChannelHandlerContext ctx, TInMsg1 message)
        {
            TInMsg2? transformed = await _first.TransformAsync(ctx, message);
            if (transformed == null)
            {
                return null;
            }

            return await _second.TransformAsync(ctx, transformed);
        }
    }

    private class FuncWrapper<TInMsg1, TInMsg2> : IChannelInboundPipe<TInMsg1, TInMsg2>
        where TInMsg1 : class
        where TInMsg2 : class
    {
        private readonly Func<IChannelHandlerContext, TInMsg1, TInMsg2?> _func;

        public FuncWrapper(Func<IChannelHandlerContext, TInMsg1, TInMsg2?> delegator)
        {
            _func = delegator;
        }

        public Task<object?> TransformAsync(IChannelHandlerContext ctx, object message)
        {
            return ((IChannelInboundPipe<TInMsg1, TInMsg2>)this).TransformAsync(ctx, message);
        }

        public Task<TInMsg2?> TransformAsync(IChannelHandlerContext ctx, TInMsg1 message)
        {
            return ctx.EventLoop.StartNew(() => _func.Invoke(ctx, message));
        }
    }
}

public interface IChannelInboundPipe<TInMsg1, TInMsg2> : IChannelInboundPipe
    where TInMsg1 : class
    where TInMsg2 : class
{
    Task<TInMsg2?> TransformAsync(IChannelHandlerContext ctx, TInMsg1 message);

    async Task<object?> IChannelInboundPipe.TransformAsync(
        IChannelHandlerContext ctx,
        object message)
    {
        if (message is not TInMsg1 castedMessage)
        {
            throw new ArgumentException($"not supported type - typename={message.GetType().FullName}");
        }

        return await TransformAsync(ctx, castedMessage);
    }

    public IChannelInboundPipe<TInMsg1, TInMsg3> Connect<TInMsg3>(
        IChannelInboundPipe<TInMsg2, TInMsg3> other)
        where TInMsg3 : class
    {
        return Connect(this, other);
    }
}
