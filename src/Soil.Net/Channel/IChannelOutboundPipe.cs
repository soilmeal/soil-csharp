using System;
using System.ComponentModel;
using Soil.Types;

namespace Soil.Net.Channel;

public interface IChannelOutboundPipe
{
    Result<ChannelPipeResultType, object> Transform(IChannelHandlerContext ctx, object message);

    public static IChannelOutboundPipe<TOutMsg1, TOutMsg2> Create<TOutMsg1, TOutMsg2>(
        Func<IChannelHandlerContext, TOutMsg1, Result<ChannelPipeResultType, TOutMsg2>> func)
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

        public Result<ChannelPipeResultType, object> Transform(
            IChannelHandlerContext ctx,
            object message)
        {
            return ((IChannelOutboundPipe<TOutMsg1, TOutMsg3>)this).Transform(ctx, message);
        }


        public Result<ChannelPipeResultType, TOutMsg3> Transform(
            IChannelHandlerContext ctx,
            TOutMsg1 message)
        {
            Result<ChannelPipeResultType, TOutMsg2> result = _first.Transform(ctx, message);
            return result.Is(ChannelPipeResultType.CallNext) && result.HasValue()
                ? _second.Transform(ctx, result.Value!)
                : Result.CreateDefault<ChannelPipeResultType, TOutMsg3>(result.Type);
        }
    }

    private class FuncWrapper<TOutMsg1, TOutMsg2>
        : IChannelOutboundPipe<TOutMsg1, TOutMsg2>
        where TOutMsg1 : class
        where TOutMsg2 : class
    {
        private readonly Func<IChannelHandlerContext, TOutMsg1, Result<ChannelPipeResultType, TOutMsg2>> _func;

        public FuncWrapper(Func<IChannelHandlerContext, TOutMsg1, Result<ChannelPipeResultType, TOutMsg2>> func)
        {
            _func = func;
        }

        public Result<ChannelPipeResultType, object> Transform(
            IChannelHandlerContext ctx,
            object message)
        {
            return ((IChannelOutboundPipe<TOutMsg1, TOutMsg2>)this).Transform(ctx, message);
        }

        public Result<ChannelPipeResultType, TOutMsg2> Transform(
            IChannelHandlerContext ctx,
            TOutMsg1 message)
        {
            return _func.Invoke(ctx, message);
        }
    }
}

public interface IChannelOutboundPipe<TOutMsg1, TOutMsg2> : IChannelOutboundPipe
    where TOutMsg1 : class
    where TOutMsg2 : class
{
    Result<ChannelPipeResultType, TOutMsg2> Transform(IChannelHandlerContext ctx, TOutMsg1 message);

    Result<ChannelPipeResultType, object> IChannelOutboundPipe.Transform(
        IChannelHandlerContext ctx,
        object message)
    {
        if (message is not TOutMsg1 castedMessage)
        {
            throw new ArgumentException($"not supported type - typename={message.GetType().FullName}");
        }

        return Transform(ctx, castedMessage).Map<object>();
    }

    public IChannelOutboundPipe<TOutMsg1, TNewMessage> Connect<TNewMessage>(
        IChannelOutboundPipe<TOutMsg2, TNewMessage> other)
        where TNewMessage : class
    {
        return Connect(this, other);
    }
}
