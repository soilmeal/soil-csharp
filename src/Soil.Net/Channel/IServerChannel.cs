using System;
using System.Net.Sockets;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;

namespace Soil.Net.Channel;

public interface IServerChannel : IChannel
{
    IChannelInitHandler InitHandler { get; }

    ChannelConfiguration ChildConfiguration { get; }

    bool TryGetChild(ulong id, out IChannel? child);

    void RequestAccept();
}
