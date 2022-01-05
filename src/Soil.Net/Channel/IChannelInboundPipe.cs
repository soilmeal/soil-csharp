using System;
using Soil.Types;

namespace Soil.Net.Channel;

public interface IChannelInboundPipe
{
    Result<ChannelPipeResultType, object> Transform(IChannelHandlerContext ctx, object message);

    public static IChannelInboundPipe<TInMsg1, TInMsg2> Create<TInMsg1, TInMsg2>(
        Func<IChannelHandlerContext, TInMsg1, Result<ChannelPipeResultType, TInMsg2>> func)
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

        public Result<ChannelPipeResultType, object> Transform(
            IChannelHandlerContext ctx,
            object message)
        {
            return ((IChannelInboundPipe<TInMsg1, TInMsg3>)this).Transform(ctx, message);
        }

        public Result<ChannelPipeResultType, TInMsg3> Transform(
            IChannelHandlerContext ctx,
            TInMsg1 message)
        {
            Result<ChannelPipeResultType, TInMsg2> result = _first.Transform(ctx, message);
            return result.Is(ChannelPipeResultType.CallNext) && result.HasValue()
                ? _second.Transform(ctx, result.Value!)
                : Result.CreateDefault<ChannelPipeResultType, TInMsg3>(result.Type);
        }
    }

    private class FuncWrapper<TInMsg1, TInMsg2> : IChannelInboundPipe<TInMsg1, TInMsg2>
        where TInMsg1 : class
        where TInMsg2 : class
    {
        private readonly Func<IChannelHandlerContext, TInMsg1, Result<ChannelPipeResultType, TInMsg2>> _func;

        public FuncWrapper(
            Func<IChannelHandlerContext, TInMsg1, Result<ChannelPipeResultType, TInMsg2>> delegator)
        {
            _func = delegator;
        }

        public Result<ChannelPipeResultType, object> Transform(
            IChannelHandlerContext ctx,
            object message)
        {
            return ((IChannelInboundPipe<TInMsg1, TInMsg2>)this).Transform(ctx, message);
        }

        public Result<ChannelPipeResultType, TInMsg2> Transform(
            IChannelHandlerContext ctx,
            TInMsg1 message)
        {
            return _func.Invoke(ctx, message);
        }
    }
}

public interface IChannelInboundPipe<TInMsg1, TInMsg2> : IChannelInboundPipe
    where TInMsg1 : class
    where TInMsg2 : class
{
    Result<ChannelPipeResultType, TInMsg2> Transform(IChannelHandlerContext ctx, TInMsg1 message);

    Result<ChannelPipeResultType, object> IChannelInboundPipe.Transform(
        IChannelHandlerContext ctx,
        object message)
    {
        if (message is not TInMsg1 castedMessage)
        {
            throw new ArgumentException($"not supported type - typename={message.GetType().FullName}");
        }

        return Transform(ctx, castedMessage).Map<object>();
    }

    public IChannelInboundPipe<TInMsg1, TInMsg3> Connect<TInMsg3>(
        IChannelInboundPipe<TInMsg2, TInMsg3> other)
        where TInMsg3 : class
    {
        return Connect(this, other);
    }
}
