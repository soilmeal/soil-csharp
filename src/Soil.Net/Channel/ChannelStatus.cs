using System;

namespace Soil.Net.Channel;

public enum ChannelStatus
{
    None = 0,

    Starting = 1,

    Running = 2,

    Closing = 3,
}

public static class ChannelStatusExtensions
{
    public static string FastToString(this ChannelStatus value)
    {
        return value switch
        {
            ChannelStatus.None => nameof(ChannelStatus.None),
            ChannelStatus.Starting => nameof(ChannelStatus.Starting),
            ChannelStatus.Running => nameof(ChannelStatus.Running),
            ChannelStatus.Closing => nameof(ChannelStatus.Closing),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}
