using System;

namespace Soil.Net;

public enum ServerStatus
{
    None = 0,

    Starting = 1,

    Listening = 2,

    Closing = 3,

    Closed = 4,
}

public static class ServerStatusExtensions
{
    public static string Name(this ServerStatus value)
    {
        return value switch
        {
            ServerStatus.None => nameof(ServerStatus.None),
            ServerStatus.Starting => nameof(ServerStatus.Starting),
            ServerStatus.Listening => nameof(ServerStatus.Listening),
            ServerStatus.Closing => nameof(ServerStatus.Closing),
            ServerStatus.Closed => nameof(ServerStatus.Closed),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}
