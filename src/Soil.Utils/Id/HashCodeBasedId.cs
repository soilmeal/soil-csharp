using System;
using System.Buffers.Binary;
using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soil.Utils.Id;

public class HashCodeBasedId : IInt64Id
{
    private readonly long _id;

    private readonly string _hexString;

    public HashCodeBasedId(byte[] bytes)
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
}
