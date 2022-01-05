
using System;
using System.Net.Sockets;

namespace Soil.Net.Sockets;

public struct SocketOptionPair : IEquatable<SocketOptionPair>
{

    public readonly SocketOptionLevel Level;

    public readonly SocketOptionName Name;

    public SocketOptionPair(SocketOptionLevel level, SocketOptionName name)
    {
        Level = level;
        Name = name;
    }

    public override int GetHashCode()
    {
        return HashCode.Combine((int)Level, (int)Name);
    }

    public bool Equals(SocketOptionPair other)
    {
        return Level == other.Level && Name == other.Name;
    }

    public override bool Equals(object? obj)
    {
        return obj is SocketOptionPair key && Equals(key);
    }
}
