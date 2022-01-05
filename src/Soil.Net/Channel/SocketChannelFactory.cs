using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Soil.Net.Channel.Configuration;

namespace Soil.Net.Channel;

public class SocketChannelFactory
{
    private delegate IChannel Constructor(ChannelConfiguration masterConfiguration);

    private static readonly Dictionary<ValueTuple<AddressFamily, SocketType, ProtocolType>, Constructor> _factory = new();

    static SocketChannelFactory()
    {
        _factory.Add(
            ValueTuple.Create(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp),
            CreateIPv4TcpChannel);
        _factory.Add(
            ValueTuple.Create(AddressFamily.InterNetworkV6, SocketType.Stream, ProtocolType.Tcp),
            CreateIPv6TcpChannel);
    }

    public static IChannel? Create(
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType,
        ChannelConfiguration configuration)
    {
        var key = ValueTuple.Create(addressFamily, socketType, protocolType);
        if (!_factory.TryGetValue(key, out Constructor? constructor))
        {
            return null;
        }

        return constructor.Invoke(configuration);
    }

    private static IChannel CreateIPv4TcpChannel(ChannelConfiguration configuration)
    {
        return new TcpSocketChannel(AddressFamily.InterNetwork, configuration);
    }

    private static IChannel CreateIPv6TcpChannel(ChannelConfiguration configuration)
    {
        return new TcpSocketChannel(AddressFamily.InterNetworkV6, configuration);
    }
}
