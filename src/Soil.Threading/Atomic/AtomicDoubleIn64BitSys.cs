using System.Threading;

namespace Soil.Threading.Atomic;

internal struct AtomicDoubleIn64BitSys : IAtomicNumeric<double>
{
    private double _value;

    internal AtomicDoubleIn64BitSys(double initialValue)
    {
        _value = initialValue;
    }

    public double Read()
    {
        return _value;
    }

    public double Add(double other)
    {
        double prevValue;
        double afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue + other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
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
        return Interlocked.Exchange(ref _value, other);
    }

    public double CompareExchange(double other, double comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }
}
