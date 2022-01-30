using System;

namespace Soil.Net.Channel;

public enum ChannelReconnectReason
{
    None = 0,

    ThrownWhenStart = 1,

    InactivedByLocal = 2,

    InactivedByRemote = 3,

    InactivedByException = 4,
}

public static class ChannelReconnectReasonExtensions
{
    public static string FastToString(this ChannelReconnectReason reason)
    {
        return reason switch
        {
            ChannelReconnectReason.None => nameof(ChannelReconnectReason.None),
            ChannelReconnectReason.ThrownWhenStart => nameof(ChannelReconnectReason.ThrownWhenStart),
            ChannelReconnectReason.InactivedByLocal => nameof(ChannelReconnectReason.InactivedByLocal),
            ChannelReconnectReason.InactivedByRemote => nameof(ChannelReconnectReason.InactivedByRemote),
            ChannelReconnectReason.InactivedByException => nameof(ChannelReconnectReason.InactivedByException),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null),
        };
    }
}
