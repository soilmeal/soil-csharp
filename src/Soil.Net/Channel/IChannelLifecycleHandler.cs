using System;

namespace Soil.Net.Channel;

public interface IChannelLifecycleHandler
{
    void HandleChannelActive(IChannel channel);

    void HandleChannelInactive(IChannel channel, ChannelInactiveReason reason, Exception? cause);

    public static IChannelLifecycleHandler Create(
        Action<IChannel> activeAction,
        Action<IChannel, ChannelInactiveReason, Exception?> inactiveAction)
    {
        return new ActionWrapper(activeAction, inactiveAction);
    }

    private class ActionWrapper : IChannelLifecycleHandler
    {
        private readonly Action<IChannel> _activeAction;

        private readonly Action<IChannel, ChannelInactiveReason, Exception?> _inactiveAction;

        public ActionWrapper(
            Action<IChannel> activeAction,
            Action<IChannel, ChannelInactiveReason, Exception?> inactiveAction)
        {
            _activeAction = activeAction;
            _inactiveAction = inactiveAction;
        }

        public void HandleChannelActive(IChannel channel)
        {
            _activeAction.Invoke(channel);
        }

        public void HandleChannelInactive(
            IChannel channel,
            ChannelInactiveReason reason,
            Exception? cause)
        {
            _inactiveAction.Invoke(channel, reason, cause);
        }
    }
}
