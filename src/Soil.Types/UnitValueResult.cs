using System;

namespace Soil.Types;

public static class UnitValueResult
{
    public static ValueResult<TType, ValueUnit> Create<TType>(TType type)
        where TType : struct, Enum, IComparable, IConvertible, IFormattable
    {
        return new ValueResult<TType, ValueUnit>(type, ValueUnit.Instance);
    }
}
