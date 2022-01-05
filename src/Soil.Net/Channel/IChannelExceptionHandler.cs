using System;

namespace Soil.Net.Channel;

public interface IChannelExceptionHandler
{
    void HandleException(IChannelHandlerContext ctx, Exception ex);

    public static IChannelExceptionHandler Create(Action<IChannelHandlerContext, Exception> action)
    {
        return new ActionWrapper(action);
    }

    private class ActionWrapper : IChannelExceptionHandler
    {
        private readonly Action<IChannelHandlerContext, Exception> _action;

        public ActionWrapper(Action<IChannelHandlerContext, Exception> action)
        {
            _action = action;
        }

        public void HandleException(IChannelHandlerContext ctx, Exception ex)
        {
            _action.Invoke(ctx, ex);
        }
    }
}
