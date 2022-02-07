using System;
using System.Numerics;

namespace Soil.Utils.Id;

public class ObjectId : IBytesId
{
    private readonly ReadOnlyMemory<byte> _bytes;

    private readonly BigInteger _id;

    private readonly string _hexString;

    public ObjectId(byte[] bytes)
    {
        _bytes = bytes;
        _id = new BigInteger(bytes);
        _hexString = BitConverter.ToString(bytes)
            .Replace("-", "");
    }

    public ReadOnlyMemory<byte> AsBytes()
    {
        return _bytes;
    }

    public BigInteger AsBigInteger()
    {
        return _id;
    }

    public string AsString()
    {
        return _hexString;
    }
}
