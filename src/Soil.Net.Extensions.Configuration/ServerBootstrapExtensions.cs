using System;
using System.Net;
using System.Runtime.CompilerServices;
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
        ArgumentNullException.ThrowIfNull(section);

        ChannelSection channelSection = section.Get<ChannelSection>();
        ArgumentNullException.ThrowIfNull(channelSection);
        ThrowIfPortOutOfRange(channelSection.Port);

        IPAddress? address;
        if (!IPAddress.TryParse(channelSection.Address, out address))
        {
            address = IPAddress.None;

        }
        ThrowIfIPAddressIsNone(address);

        return bootstrap.BindAsync(new IPEndPoint(address, channelSection.Port));
    }

    public static Task<IServerChannel> StartAsync(
            this ServerChannelBootstrap bootstrap,
            IConfigurationSection section)
    {
        ArgumentNullException.ThrowIfNull(section);

        ChannelSection channelSection = section.Get<ChannelSection>();
        ArgumentNullException.ThrowIfNull(channelSection);
        ThrowIfPortOutOfRange(channelSection.Port);

        IPAddress? address;
        if (!IPAddress.TryParse(channelSection.Address, out address))
        {
            address = IPAddress.None;

        }
        ThrowIfIPAddressIsNone(address);

        return bootstrap.StartAsync(
            new IPEndPoint(address, channelSection.Port),
            channelSection.Backlog);
    }

    private static void ThrowIfPortOutOfRange(
        int port,
        [CallerArgumentExpression("port")] string? portExpression = null)
    {
        if (port < IPEndPoint.MinPort || port > IPEndPoint.MaxPort)
        {
            throw new ArgumentOutOfRangeException(portExpression ?? nameof(port), port, null);
        }
    }

    private static void ThrowIfIPAddressIsNone(IPAddress address)
    {
        if (address == IPAddress.None)
        {
            throw new ArgumentException($"invalid address");
        }
    }
}
