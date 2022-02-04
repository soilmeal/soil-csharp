using System;

namespace Soil.Net.Channel;

public interface IChannelInitHandler
{
    void InitChannel(IChannel channel);

    public static IChannelInitHandler Create(Func<IChannel, IChannel> func)
    {
        return new FuncWrapper(func);
    }

    private class FuncWrapper : IChannelInitHandler
    {
        private readonly Func<IChannel, IChannel> _action;

        public FuncWrapper(Func<IChannel, IChannel> func)
        {
            _action = func;
        }

        public void InitChannel(IChannel channel)
        {
            _action.Invoke(channel);
        }
    }
}
