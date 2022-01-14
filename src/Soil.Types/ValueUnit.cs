using System;

namespace Soil.Types;

public struct ValueUnit : IEquatable<ValueUnit>
{
    public static readonly ValueUnit Instance = new();

    public bool Equals(ValueUnit other)
    {
        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is ValueUnit;
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}
