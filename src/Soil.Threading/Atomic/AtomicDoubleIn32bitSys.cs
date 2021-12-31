using System;
using System.Threading;

namespace Soil.Threading.Atomic;

internal struct AtomicDoubleIn32BitSys : IAtomicNumeric<double>
{
    private long _value;

    internal AtomicDoubleIn32BitSys(double initialValue)
    {
        _value = BitConverter.DoubleToInt64Bits(initialValue);
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
        long otherBits = BitConverter.DoubleToInt64Bits(other);

        long resultBits = ExchangeRaw(otherBits);
        return BitConverter.Int64BitsToDouble(resultBits);
    }

    public double CompareExchange(double other, double comparand)
    {
        long otherBits = BitConverter.DoubleToInt64Bits(other);
        long comparndBits = BitConverter.DoubleToInt64Bits(comparand);

        long resultBits = CompareExchangeRaw(otherBits, comparndBits);
        return BitConverter.Int64BitsToDouble(resultBits);
    }

    public double Read()
    {
        return BitConverter.Int64BitsToDouble(ReadRaw());
    }

    public long ExchangeRaw(long other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public long CompareExchangeRaw(long other, long comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }

    public long ReadRaw()
    {
        return Interlocked.Read(ref _value);
    }
}

