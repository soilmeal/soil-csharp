using System;

namespace Soil.Net.Channel;

public enum ChannelPipeResultType
{
    None = 0,
    CallNext = 1,
    ContinueIO = 2,
    Completed = 3,
}

public static class ChannelInboundPipeResultTypeExtensions
{
    public static string FastToString(this ChannelPipeResultType value)
    {
        return value switch
        {
            ChannelPipeResultType.None => nameof(ChannelPipeResultType.None),
            ChannelPipeResultType.CallNext => nameof(ChannelPipeResultType.CallNext),
            ChannelPipeResultType.ContinueIO => nameof(ChannelPipeResultType.ContinueIO),
            ChannelPipeResultType.Completed => nameof(ChannelPipeResultType.Completed),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}
