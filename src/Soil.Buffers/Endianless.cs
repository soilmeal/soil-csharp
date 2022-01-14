using System;

namespace Soil.Buffers;

public enum Endianless
{
    None = 0,

    BigEndian = 1,

    LittleEndian = 2,
}

public static class EndianlessExtensions
{
    public static string FastToString(this Endianless value)
    {
        return value switch
        {
            Endianless.None => nameof(Endianless.None),
            Endianless.BigEndian => nameof(Endianless.BigEndian),
            Endianless.LittleEndian => nameof(Endianless.LittleEndian),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}
