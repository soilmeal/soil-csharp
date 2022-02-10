using System.Threading;

namespace Soil.Threading.Atomic;

public class AtomicInt64 : IAtomicInteger<long>
{
    private long _value;

    public AtomicInt64() : this(0L)
    {
    }

    public AtomicInt64(long initialValue)
    {
        _value = initialValue;
    }

    public long Read()
    {
        return Interlocked.Read(ref _value);
    }

    public long Add(long other)
    {
        return Interlocked.Add(ref _value, other);
    }

    public long Increment()
    {
        return Add(1L);
    }

    public long Decrement()
    {
        return Interlocked.Decrement(ref _value);
    }

    public long And(long other)
    {
        // return Interlocked.And(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        long prevValue;
        long afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue & other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public long Or(long other)
    {
        // return Interlocked.Or(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        long prevValue;
        long afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue | other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public long Exchange(long other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public long CompareExchange(long other, long comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }

    public static implicit operator AtomicInt64(long value)
    {
        return new AtomicInt64(value);
    }

    public static implicit operator long(AtomicInt64 atomic)
    {
        return atomic.Read();
    }

    public static explicit operator sbyte(AtomicInt64 atomic)
    {
        long val = atomic;
        return (sbyte)val;
    }

    public static explicit operator short(AtomicInt64 atomic)
    {
        long val = atomic;
        return (short)val;
    }

    public static explicit operator int(AtomicInt64 atomic)
    {
        long val = atomic;
        return (int)val;
    }

    public static explicit operator byte(AtomicInt64 atomic)
    {
        long val = atomic;
        return (byte)val;
    }

    public static explicit operator ushort(AtomicInt64 atomic)
    {
        long val = atomic;
        return (ushort)val;
    }

    public static explicit operator uint(AtomicInt64 atomic)
    {
        long val = atomic;
        return (uint)val;
    }

    public static explicit operator ulong(AtomicInt64 atomic)
    {
        long val = atomic;
        return (ulong)val;
    }
}
