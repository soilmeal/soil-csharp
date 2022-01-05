using System;

namespace Soil.Net.Channel;

public interface IChannelLifecycleHandler
{
    void HandleChannelActive(IChannel channel);

    void HandleChannelInactive(IChannel channel);

    public static IChannelLifecycleHandler Create(
        Action<IChannel> activeAction,
        Action<IChannel> inactiveAction)
    {
        return new ActionWrapper(activeAction, inactiveAction);
    }

    private class ActionWrapper : IChannelLifecycleHandler
    {
        private readonly Action<IChannel> _activeAction;

        private readonly Action<IChannel> _inactiveAction;

        public ActionWrapper(Action<IChannel> activeAction, Action<IChannel> inactiveAction)
        {
            _activeAction = activeAction;
            _inactiveAction = inactiveAction;
        }

        public void HandleChannelActive(IChannel channel)
        {
            _activeAction.Invoke(channel);
        }

        public void HandleChannelInactive(IChannel channel)
        {
            _inactiveAction.Invoke(channel);
        }
    }
}
