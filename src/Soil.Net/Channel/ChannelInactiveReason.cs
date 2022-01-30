using System;

namespace Soil.Net.Channel;

public enum ChannelInactiveReason
{
    None = 0,

    ByLocal = 1,

    ByRemote = 2,

    ByException = 3,
}

public static class ChannelInactiveReasonExtensions
{
    public static string FastToString(this ChannelInactiveReason reason)
    {
        return reason switch
        {
            ChannelInactiveReason.None => nameof(ChannelInactiveReason.None),
            ChannelInactiveReason.ByLocal => nameof(ChannelInactiveReason.ByLocal),
            ChannelInactiveReason.ByRemote => nameof(ChannelInactiveReason.ByRemote),
            ChannelInactiveReason.ByException => nameof(ChannelInactiveReason.ByException),
            _ => throw new ArgumentOutOfRangeException(nameof(reason), reason, null),
        };
    }

    public static ChannelReconnectReason ConvertToReconnectReason(this ChannelInactiveReason reason)
    {
        return reason switch
        {
            ChannelInactiveReason.ByLocal => ChannelReconnectReason.InactivedByLocal,
            ChannelInactiveReason.ByRemote => ChannelReconnectReason.InactivedByRemote,
            ChannelInactiveReason.ByException => ChannelReconnectReason.InactivedByException,
            _ => ChannelReconnectReason.None,
        };
    }
}
