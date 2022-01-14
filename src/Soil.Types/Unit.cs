using System;

namespace Soil.Types;

public class Unit : IEquatable<Unit>
{
    public static readonly Unit Instance = new();

    private Unit()
    {
    }

    public bool Equals(Unit? other)
    {
        return true;
    }

    public override bool Equals(object? obj)
    {
        return obj is Unit;
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
