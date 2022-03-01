using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Soil.Utils.Id;

public abstract class HashCodeBasedId : IUInt64Id
{
    private readonly ulong _id;

    private readonly string _hexString;

    protected HashCodeBasedId(byte[] bytes)
    {
        _id = BinaryPrimitives.ReadUInt64BigEndian(bytes);
        _hexString = BitConverter.ToString(bytes)
            .Replace("-", "");
    }

    public ulong Value
    {
        get
        {
            return _id;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public ulong AsUInt64()
    {
        return Value;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public string AsString()
    {
        return _hexString;
    }

    public abstract bool Equals(IUInt64Id? other);

    public abstract bool Equals(ulong other);
}
