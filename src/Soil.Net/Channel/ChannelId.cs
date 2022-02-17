using System;
using Soil.Utils.Id;

namespace Soil.Net.Channel;

#pragma warning disable CS0659, CS0661
public class ChannelId : HashCodeBasedId, IEquatable<ChannelId>
{
    public ChannelId(byte[] bytes)
        : base(bytes)
    {
    }

    public override bool Equals(object? obj)
    {
        return obj is ChannelId id && Equals(id);
    }

    public override bool Equals(IInt64Id? other)
    {
        return other is ChannelId id && Equals(id);
    }

    public override bool Equals(long other)
    {
        return Value == other;
    }

    public bool Equals(ChannelId? other)
    {
        return Equals(this, other);
    }

    public static bool operator ==(ChannelId? channelId, long other)
    {
        return channelId != null && channelId.Equals(other);
    }

    public static bool operator !=(ChannelId? channelId, long other)
    {
        return channelId != null && !channelId.Equals(other);
    }

    public static bool operator ==(ChannelId? lhs, ChannelId? rhs)
    {
        return Equals(lhs, rhs);
    }

    public static bool operator !=(ChannelId? lhs, ChannelId? rhs)
    {
        return !Equals(lhs, rhs);
    }

    private static bool Equals(ChannelId? lhs, ChannelId? rhs)
    {
        if (ReferenceEquals(lhs, rhs))
        {
            return true;
        }

        if (lhs is null || rhs is null)
        {
            return false;
        }

        return lhs.Value == rhs.Value;
    }
}
#pragma warning restore CS0659, CS0661
