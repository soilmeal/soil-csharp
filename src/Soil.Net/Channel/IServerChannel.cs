using Soil.Net.Channel.Configuration;

namespace Soil.Net.Channel;

public interface IServerChannel : IChannel
{
    IChannelInitHandler InitHandler { get; }

    ChannelConfiguration ChildConfiguration { get; }

    void RequestAccept();
}
