using System;

namespace Soil.Net.Channel;

public interface IChannelReconnectHandler
{
    public void HandleReconnectStart(IChannelHandlerContext ctx);

    public void HandleReconnectEnd(IChannelHandlerContext ctx, bool isSuccess);

    public static IChannelReconnectHandler Create(
        Action<IChannelHandlerContext> onStart,
        Action<IChannelHandlerContext, bool> onEnd)
    {
        return new ActionWrapper(onStart, onEnd);
    }

    private class ActionWrapper : IChannelReconnectHandler
    {
        private readonly Action<IChannelHandlerContext> _onStart;

        private readonly Action<IChannelHandlerContext, bool> _onEnd;

        public ActionWrapper(
            Action<IChannelHandlerContext> onStart,
            Action<IChannelHandlerContext, bool> onEnd)
        {
            _onStart = onStart;
            _onEnd = onEnd;
        }

        public void HandleReconnectStart(IChannelHandlerContext ctx)
        {
            _onStart(ctx);
        }

        public void HandleReconnectEnd(IChannelHandlerContext ctx, bool isSuccess)
        {
            _onEnd(ctx, isSuccess);
        }
    }
}
