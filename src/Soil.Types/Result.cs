using System;
using System.Collections.Generic;

namespace Soil.Types;

public struct Result<TType, TValue>
    where TType : struct, Enum, IComparable, IConvertible, IFormattable
    where TValue : class
{
    public readonly TType Type;

    public readonly TValue? Value;

    internal Result(TType type, TValue? value)
    {
        Type = type;
        Value = value;
    }

    public bool Is(TType type)
    {
        return EqualityComparer<TType>.Default.Equals(Type, type);
    }

    public bool HasValue()
    {
        return Value != null;
    }

    public bool Has(TValue? value)
    {
        return Value == value;
    }

    public void If(TType type, Action<TValue?> action)
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

    public void IfNot(TType type, Action<TValue?> action)
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

    public void IfHas(Action<TValue> action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (!HasValue())
        {
            return;
        }

        action.Invoke(Value!);
    }

    public void IfNotHas(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        if (HasValue())
        {
            return;
        }

        action.Invoke();
    }

    public void IfValueIs(TValue value, Action<TValue> action)
    {
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

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
        if (value == null)
        {
            throw new ArgumentNullException(nameof(value));
        }

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

    public Result<TType, TNewValue> Map<TNewValue>(Func<TValue?, TNewValue?> func)
        where TNewValue : class
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Result.Create(Type, func.Invoke(Value));
    }

    public Result<TType, TNewValue> Map<TNewValue>()
        where TNewValue : class
    {
        return Result.Create(Type, Value as TNewValue);
    }
}

public struct Result
{
    public static Result<TType, TValue> Create<TType, TValue>(
        TType type,
        TValue? value)
        where TType : struct, Enum, IComparable, IConvertible, IFormattable
        where TValue : class
    {
        return new Result<TType, TValue>(type, value);
    }
    public static Result<TType, TValue> CreateDefault<TType, TValue>(
        TType type)
        where TType : struct, Enum, IComparable, IConvertible, IFormattable
        where TValue : class
    {
        return new Result<TType, TValue>(type, default);
    }
}
