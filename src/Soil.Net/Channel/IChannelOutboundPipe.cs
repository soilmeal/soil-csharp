using System;
using System.Threading.Tasks;

namespace Soil.Net.Channel;

public interface IChannelOutboundPipe
{
    Task<object> TransformAsync(IChannelHandlerContext ctx, object message);

    public static IChannelOutboundPipe<TOutMsg1, TOutMsg2> Create<TOutMsg1, TOutMsg2>(
        Func<IChannelHandlerContext, TOutMsg1, TOutMsg2> func)
        where TOutMsg1 : class
        where TOutMsg2 : class
    {
        return new FuncWrapper<TOutMsg1, TOutMsg2>(func);
    }

    public static IChannelOutboundPipe<TOutMsg1, TOutMsg3> Connect<TOutMsg1, TOutMsg2, TOutMsg3>(
        IChannelOutboundPipe<TOutMsg1, TOutMsg2> first,
        IChannelOutboundPipe<TOutMsg2, TOutMsg3> second)
        where TOutMsg1 : class
        where TOutMsg2 : class
        where TOutMsg3 : class
    {
        return new Connector<TOutMsg1, TOutMsg2, TOutMsg3>(first, second);
    }

    private class Connector<TOutMsg1, TOutMsg2, TOutMsg3>
        : IChannelOutboundPipe<TOutMsg1, TOutMsg3>
        where TOutMsg1 : class
        where TOutMsg2 : class
        where TOutMsg3 : class
    {
        private readonly IChannelOutboundPipe<TOutMsg1, TOutMsg2> _first;

        private readonly IChannelOutboundPipe<TOutMsg2, TOutMsg3> _second;

        public Connector(
            IChannelOutboundPipe<TOutMsg1, TOutMsg2> first,
            IChannelOutboundPipe<TOutMsg2, TOutMsg3> second)
        {
            _first = first;
            _second = second;
        }

        public Task<object> TransformAsync(
            IChannelHandlerContext ctx,
            object message)
        {
            return ((IChannelOutboundPipe<TOutMsg1, TOutMsg3>)this).TransformAsync(ctx, message);
        }


        public async Task<TOutMsg3> TransformAsync(
            IChannelHandlerContext ctx,
            TOutMsg1 message)
        {
            TOutMsg2 transformed = await _first.TransformAsync(ctx, message);
            return await _second.TransformAsync(ctx, transformed);
        }
    }

    private class FuncWrapper<TOutMsg1, TOutMsg2>
        : IChannelOutboundPipe<TOutMsg1, TOutMsg2>
        where TOutMsg1 : class
        where TOutMsg2 : class
    {
        private readonly Func<IChannelHandlerContext, TOutMsg1, TOutMsg2> _func;

        public FuncWrapper(Func<IChannelHandlerContext, TOutMsg1, TOutMsg2> func)
        {
            _func = func;
        }

        public Task<object> TransformAsync(IChannelHandlerContext ctx, object message)
        {
            return ((IChannelOutboundPipe<TOutMsg1, TOutMsg2>)this).TransformAsync(
                ctx,
                (TOutMsg2)message);
        }

        public Task<TOutMsg2> TransformAsync(IChannelHandlerContext ctx, TOutMsg1 message)
        {
            return ctx.EventLoop.StartNew(() => _func.Invoke(ctx, message));
        }
    }
}

public interface IChannelOutboundPipe<TOutMsg1, TOutMsg2> : IChannelOutboundPipe
    where TOutMsg1 : class
    where TOutMsg2 : class
{
    Task<TOutMsg2> TransformAsync(IChannelHandlerContext ctx, TOutMsg1 message);

    async Task<object> IChannelOutboundPipe.TransformAsync(
        IChannelHandlerContext ctx,
        object message)
    {
        if (message is not TOutMsg1 castedMessage)
        {
            throw new ArgumentException($"not supported type - typename={message.GetType().FullName}");
        }

        return await TransformAsync(ctx, castedMessage);
    }

    public IChannelOutboundPipe<TOutMsg1, TNewMessage> Connect<TNewMessage>(
        IChannelOutboundPipe<TOutMsg2, TNewMessage> other)
        where TNewMessage : class
    {
        return Connect(this, other);
    }
}
