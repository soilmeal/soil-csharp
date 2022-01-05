using System;

namespace Soil.Types;

public static class UnitResult
{
    public static Result<TType, Unit> Create<TType>(TType type)
        where TType : struct, Enum, IComparable, IConvertible, IFormattable
    {
        return new Result<TType, Unit>(type, Unit.Instance);
    }
}
