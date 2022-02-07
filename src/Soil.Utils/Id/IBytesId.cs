using System;
using System.Numerics;

namespace Soil.Utils.Id;

public interface IBytesId
{
    public ReadOnlyMemory<byte> AsBytes();

    public BigInteger AsBigInteger();

    public string AsString();
}
