using System;
using System.Diagnostics.CodeAnalysis;
using System.Net.Sockets;

namespace Soil.Net.Sockets;

public class SocketOptionValue
{
    private object? _value = null;

    private SocketOptionValueType _type = SocketOptionValueType.Unknown;

    public SocketOptionValueType Type
    {
        get
        {
            return _type;
        }
    }
    public void SetValue(int value)
    {
        SetValue(value, SocketOptionValueType.Int32);
    }

    public void SetValue(bool value)
    {
        SetValue(value, SocketOptionValueType.Bool);
    }

    public void SetValue(byte[] value)
    {
        SetValue(value, SocketOptionValueType.Bytes);
    }

    public void SetValue(object value)
    {
        SetValue(value, SocketOptionValueType.Object);
    }

    public bool TryGetValue(out int value)
    {
        if (_value is int intVal)
        {
            value = intVal;
            return true;
        }

        value = default;
        return false;
    }

    public bool TryGetValue(out bool value)
    {
        if (_value is bool boolVal)
        {
            value = boolVal;
            return true;
        }

        value = default;
        return false;
    }

    public void Apply(Socket socket)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }
    }

    public bool TryGetValue([NotNullWhen(true)] out byte[]? value)
    {
        if (_value is byte[] bytesVal)
        {
            value = bytesVal;
            return true;
        }

        value = null;
        return false;
    }

    public bool TryGetValue([NotNullWhen(true)] out object? value)
    {
        value = _value;
        return value != null;
    }

    private void SetValue(object value, SocketOptionValueType type)
    {
        _value = value;
        _type = type;
    }
}
