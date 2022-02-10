using System.Threading;

namespace Soil.Threading.Atomic;

public class AtomicReference<T> : IAtomic<T>
    where T : class?
{
    private T _value;

    public AtomicReference()
    {
        _value = null!;
    }

    public AtomicReference(T value)
    {
        _value = value;
    }

    public T CompareExchange(T other, T comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }

    public T Exchange(T other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public T Read()
    {
        return _value;
    }

    public static implicit operator AtomicReference<T>(T target)
    {
        return new AtomicReference<T>(target);
    }

    public static implicit operator T(AtomicReference<T> target)
    {
        return target.Read();
    }
}
