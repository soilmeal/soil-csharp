using System;
using System.Collections.Generic;

namespace Soil.Types;

public struct ValueResult<TType, TValue>
    where TType : struct, Enum, IComparable, IConvertible, IFormattable
    where TValue : struct
{
    public readonly TType Type;

    public readonly TValue Value;

    public ValueResult(TType type)
        : this(type, default)
    {
    }

    public ValueResult(TType type, TValue value)
    {
        Type = type;
        Value = value;
    }

    public bool Is(TType type)
    {
        return EqualityComparer<TType>.Default.Equals(Type, type);
    }

    public bool HasDefaultValue()
    {
        return Has(default);
    }

    public bool Has(TValue value)
    {
        return EqualityComparer<TValue>.Default.Equals(Value, value);
    }

    public void If(TType type, Action<TValue> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!Is(type))
        {
            return;
        }

        action.Invoke(Value);
    }

    public void IfNot(TType type, Action<TValue> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (Is(type))
        {
            return;
        }

        action.Invoke(Value);
    }

    public void IfValueIs(TValue value, Action<TValue> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!Has(value))
        {
            return;
        }

        action.Invoke(Value!);
    }

    public void IfValueIsNot(TValue value, Action<TValue?> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (Has(value))
        {
            return;
        }

        action.Invoke(Value);
    }

    public ValueResult<TType, TNewValue> Map<TNewValue>(Func<TValue, TNewValue> func)
        where TNewValue : struct
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return ValueResult.Create(Type, func.Invoke(Value));
    }
}

public struct ValueResult
{
    public static ValueResult<TType, TValue> Create<TType, TValue>(
        TType type,
        TValue value)
        where TType : struct, Enum, IComparable, IConvertible, IFormattable
        where TValue : struct
    {
        return new ValueResult<TType, TValue>(type, value);
    }
}
