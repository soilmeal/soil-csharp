using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Soil.Net.Channel.Configuration;

namespace Soil.Net.Channel;

public static class SocketServerChannelFactory
{
    private delegate IServerChannel Constructor(
        ChannelConfiguration masterConfiguration,
        ChannelConfiguration childConfiguration);

    private static readonly Dictionary<ValueTuple<AddressFamily, SocketType, ProtocolType>, Constructor> _factory = new();

    static SocketServerChannelFactory()
    {
        _factory.Add(
            ValueTuple.Create(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
            CreateIPv4TcpChannel);
        _factory.Add(
            ValueTuple.Create(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp),
            CreateIPv6TcpChannel);
    }

    public static IServerChannel? Create(
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType,
        ChannelConfiguration masterConfiguration,
        ChannelConfiguration childConfiguration)
    {
        var key = ValueTuple.Create(addressFamily, socketType, protocolType);
        if (!_factory.TryGetValue(key, out Constructor? constructor))
        {
            return null;
        }

        return constructor.Invoke(masterConfiguration, childConfiguration);
    }

    private static IServerChannel CreateIPv4TcpChannel(
        ChannelConfiguration masterConfiguration,
        ChannelConfiguration childConfiguration)
    {
        return new TcpSocketServerChannel(
            AddressFamily.InterNetwork,
            masterConfiguration,
            childConfiguration);
    }

    private static IServerChannel CreateIPv6TcpChannel(
        ChannelConfiguration masterConfiguration,
        ChannelConfiguration childConfiguration)
    {
        return new TcpSocketServerChannel(
            AddressFamily.InterNetworkV6,
            masterConfiguration,
            childConfiguration);
    }
}
