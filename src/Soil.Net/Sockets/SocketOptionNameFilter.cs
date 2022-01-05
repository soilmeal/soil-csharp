using System;
using System.Net.Sockets;

namespace Soil.Net.Sockets;

// https://github.com/dotnet/runtime/blob/main/src/libraries/System.Net.Sockets/src/System/Net/Sockets/SocketOptionName.cs
public static class SocketOptionNameValidator
{
    public static bool Validate(SocketOptionLevel level, SocketOptionName name)
    {
        return level switch
        {
            SocketOptionLevel.Socket => name switch
            {
                SocketOptionName.Debug
                or SocketOptionName.AcceptConnection
                or SocketOptionName.ReuseAddress
                or SocketOptionName.KeepAlive
                or SocketOptionName.DontRoute
                or SocketOptionName.Broadcast
                or SocketOptionName.UseLoopback
                or SocketOptionName.Linger
                or SocketOptionName.OutOfBandInline
                or SocketOptionName.DontLinger
                or SocketOptionName.ExclusiveAddressUse
                or SocketOptionName.SendBuffer
                or SocketOptionName.ReceiveBuffer
                or SocketOptionName.SendLowWater
                or SocketOptionName.ReceiveLowWater
                or SocketOptionName.SendTimeout
                or SocketOptionName.ReceiveTimeout
                or SocketOptionName.Error
                or SocketOptionName.Type
                or SocketOptionName.ReuseUnicastPort
                or SocketOptionName.MaxConnections => true,
                _ => false,
            },
            SocketOptionLevel.IP => name switch
            {
                SocketOptionName.IPOptions
                or SocketOptionName.HeaderIncluded
                or SocketOptionName.TypeOfService
                or SocketOptionName.IpTimeToLive
                or SocketOptionName.MulticastInterface
                or SocketOptionName.MulticastTimeToLive
                or SocketOptionName.MulticastLoopback
                or SocketOptionName.AddMembership
                or SocketOptionName.DropMembership
                or SocketOptionName.DontFragment
                or SocketOptionName.AddSourceMembership
                or SocketOptionName.DropSourceMembership
                or SocketOptionName.BlockSource
                or SocketOptionName.UnblockSource
                or SocketOptionName.PacketInformation => true,
                _ => false,
            },
            SocketOptionLevel.IPv6 => name switch
            {
                SocketOptionName.HopLimit
                or SocketOptionName.IPProtectionLevel
                or SocketOptionName.IPv6Only => true,
                _ => false,
            },
            SocketOptionLevel.Tcp => name switch
            {
                SocketOptionName.NoDelay
                or SocketOptionName.BsdUrgent
                or SocketOptionName.Expedited => true,
                // or SocketOptionName.TcpKeepAliveRetryCount
                // or SocketOptionName.TcpKeepAliveTime
                // or SocketOptionName.TcpKeepAliveInterval => true,
                _ => false,
            },
            SocketOptionLevel.Udp => name switch
            {
                SocketOptionName.NoChecksum
                or SocketOptionName.ChecksumCoverage
                or SocketOptionName.UpdateAcceptContext
                or SocketOptionName.UpdateConnectContext => true,
                _ => false,
            },
            _ => throw new ArgumentOutOfRangeException(nameof(level), level, null),
        };
    }
}
