using System;
using System.Net;
using System.Net.NetworkInformation;
using System.Net.Sockets;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Soil.Net.Channel;

namespace Soil.Net.Extensions.Configuration;

public static class ServerBootstrapExtensions
{
    public static Task<IServerChannel> BindAsync(
        this ServerChannelBootstrap bootstrap,
        IConfigurationSection section)
    {
        ChannelSection channelSection = section.Get<ChannelSection>();
        ThrowIfPortOutOfRange(channelSection);

        IPAddress address = DetectAddress(channelSection);
        ThrowIfIPAddressIsNone(channelSection, address);

        return bootstrap.BindAsync(new IPEndPoint(address, channelSection.Port));
    }

    public static Task<IServerChannel> StartAsync(
            this ServerChannelBootstrap bootstrap,
            IConfigurationSection section)
    {
        ChannelSection channelSection = section.Get<ChannelSection>();
        ThrowIfPortOutOfRange(channelSection);

        IPAddress address = DetectAddress(channelSection);
        ThrowIfIPAddressIsNone(channelSection, address);

        return bootstrap.StartAsync(
            new IPEndPoint(address, channelSection.Port),
            channelSection.Backlog);
    }

    private static IPAddress DetectAddress(ChannelSection section)
    {
        // IPAddress? address = null;
        if (!section.AutoDetectAddress)
        {
            IPAddress? address;
            return IPAddress.TryParse(section.Address, out address) ? address : IPAddress.None;
        }

        NetworkInterface[] networkInterfaces = NetworkInterface.GetAllNetworkInterfaces();
        foreach (var networkInterface in networkInterfaces)
        {
            if (networkInterface.OperationalStatus != OperationalStatus.Up)
            {
                continue;
            }

            if (networkInterface.IsReceiveOnly)
            {
                continue;
            }

            NetworkInterfaceType networkInterfaceType = networkInterface.NetworkInterfaceType;
            if (networkInterfaceType != NetworkInterfaceType.Ethernet
                && networkInterfaceType != NetworkInterfaceType.Wireless80211)
            {
                continue;
            }

            foreach (var addressInfo in networkInterface.GetIPProperties().UnicastAddresses)
            {
                AddressFamily addressFamily = addressInfo.Address.AddressFamily;
                if (addressFamily == AddressFamily.InterNetwork
                    || addressFamily == AddressFamily.InterNetworkV6)
                {
                    return addressInfo.Address;
                }
            }
        }

        return IPAddress.None;
    }

    private static void ThrowIfPortOutOfRange(ChannelSection channelSection)
    {
        if (channelSection.Port < IPEndPoint.MinPort
            || channelSection.Port > IPEndPoint.MaxPort)
        {
            throw new InvalidOperationException($"\"Port\" out of range. Port={channelSection.Port}");
        }
    }

    private static void ThrowIfIPAddressIsNone(ChannelSection channelSection, IPAddress address)
    {
        if (address == IPAddress.None)
        {
            throw new InvalidOperationException($"\"Address\" not found. Address={channelSection.Address}, AutoDetectAddress={channelSection.AutoDetectAddress}");
        }
    }
}
