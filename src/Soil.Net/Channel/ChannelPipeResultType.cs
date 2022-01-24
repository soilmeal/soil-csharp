using System;

namespace Soil.Net.Channel;

public enum ChannelPipeResultType
{
    None = 0,
    CallNext = 1,
    ContinueIO = 2,
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
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}
