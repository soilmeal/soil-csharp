using System.Net;
using Soil.Net.Channel;

namespace Soil.Net.Extensions.Configuration;

public class ChannelSection
{
    public string Address { get; set; } = IPAddress.Loopback.ToString();

    public int Port { get; set; }

    public int Backlog { get; set; } = Constants.DefaultBacklog;

    public bool AutoDetectAddress { get; set; } = false;
}
