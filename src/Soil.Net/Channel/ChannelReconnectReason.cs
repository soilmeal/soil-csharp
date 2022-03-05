using System;

namespace Soil.Net.Channel;

public enum ChannelReconnectReason
{
    None = 0,

    ThrownWhenStart = 1,

    InactiveByLocal = 2,

    InactiveByRemote = 3,

    InactiveByException = 4,
}

public static class ChannelReconnectReasonExtensions
{
    public static string FastToString(this ChannelReconnectReason reason)
    {
        return reason switch
        {
            ChannelReconnectReason.None => nameof(ChannelReconnectReason.None),
            ChannelReconnectReason.ThrownWhenStart => nameof(ChannelReconnectReason.ThrownWhenStart),
            ChannelReconnectReason.InactiveByLocal => nameof(ChannelReconnectReason.InactiveByLocal),
            ChannelReconnectReason.InactiveByRemote => nameof(ChannelReconnectReason.InactiveByRemote),
            ChannelReconnectReason.InactiveByException => nameof(ChannelReconnectReason.InactiveByException),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null),
        };
    }
}
