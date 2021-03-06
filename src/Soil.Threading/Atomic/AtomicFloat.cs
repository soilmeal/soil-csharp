using System.Threading;

namespace Soil.Threading.Atomic;

public class AtomicFloat : IAtomicNumeric<float>
{
    private float _value;

    public AtomicFloat() : this(0f)
    {
    }

    public AtomicFloat(float initialValue)
    {
        _value = initialValue;
    }

    public float Read()
    {
        return _value;
    }

    public float Add(float other)
    {
        float prevValue;
        float afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue + other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public float Increment()
    {
        return Add(1f);
    }

    public float Decrement()
    {
        return Add(-1f);
    }

    public float Exchange(float other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public float CompareExchange(float other, float comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }

    public static implicit operator AtomicFloat(float value)
    {
        return new AtomicFloat(value);
    }

    public static implicit operator float(AtomicFloat atomic)
    {
        return atomic.Read();
    }

    public static implicit operator double(AtomicFloat atomic)
    {
        return (float)atomic;
    }
}
