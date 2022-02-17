using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Soil.Utils.Id;

public abstract class HashCodeBasedId : IInt64Id
{
    private readonly long _id;

    private readonly string _hexString;

    protected HashCodeBasedId(byte[] bytes)
    {
        _id = BinaryPrimitives.ReadInt64BigEndian(bytes);
        _hexString = BitConverter.ToString(bytes)
            .Replace("-", "");
    }

    public long Value
    {
        get
        {
            return _id;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public long AsInt64()
    {
        return Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string AsString()
    {
        return _hexString;
    }

    public abstract bool Equals(IInt64Id? other);

    public abstract bool Equals(long other);
}
