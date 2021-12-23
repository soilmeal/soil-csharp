using System;

namespace Soil.Core.Threading.Atomic;

public struct AtomicDouble : IAtomicNumeric<double>
{
    private readonly IAtomicNumeric<double> _impl;

    public AtomicDouble() : this(0.0) { }

    public AtomicDouble(double initialValue)
    {
        bool is64Bit = Environment.Is64BitOperatingSystem && Environment.Is64BitProcess;
        _impl = is64Bit
            ? new AtomicDoubleIn64BitSys(initialValue)
            : new AtomicDoubleIn32BitSys(initialValue);
    }

    public double Read()
    {
        return _impl.Read();
    }

    public double Add(double other)
    {
        return _impl.Add(other);
    }

    public double Increment()
    {
        return Add(1.0);
    }

    public double Decrement()
    {
        return Add(-1.0);
    }

    public double Exchange(double other)
    {
        return _impl.Exchange(other);
    }

    public double CompareExchange(double other, double comparand)
    {
        return _impl.CompareExchange(other, comparand);
    }

    public static implicit operator double(AtomicDouble atomic)
    {
        return atomic.Read();
    }

    public static explicit operator float(AtomicDouble atomic)
    {
        double val = atomic;
        return (float)val;
    }
}
