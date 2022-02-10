using System.Threading;

namespace Soil.Threading.Atomic;

public class AtomicUInt64 : IAtomicInteger<ulong>
{
    private long _value;

    public AtomicUInt64() : this(0UL)
    {
    }

    public AtomicUInt64(ulong initialValue)
    {
        _value = ToInt64(initialValue);
    }

    private static long ToInt64(ulong value)
    {
        return unchecked((long)value);
    }

    private static ulong ToUInt64(long value)
    {
        return unchecked((ulong)value);
    }

    public ulong Read()
    {
        return ToUInt64(Interlocked.Read(ref _value));
    }

    public ulong Add(ulong other)
    {
        return ToUInt64(Interlocked.Add(ref _value, (long)other));
    }

    public ulong Increment()
    {
        return Add(1UL);
    }

    public ulong Decrement()
    {
        return ToUInt64(Interlocked.Add(ref _value, -1));
    }

    public ulong And(ulong other)
    {
        // return Interlocked.And(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        ulong prevValue;
        ulong afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue & other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public ulong Or(ulong other)
    {
        // return Interlocked.Or(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        ulong prevValue;
        ulong afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue | other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public ulong Exchange(ulong other)
    {
        return ToUInt64(Interlocked.Exchange(ref _value, ToInt64(other)));
    }

    public ulong CompareExchange(ulong other, ulong comparand)
    {
        return ToUInt64(Interlocked.CompareExchange(
            ref _value,
            ToInt64(other),
            ToInt64(comparand)));
    }

    public static implicit operator AtomicUInt64(ulong value)
    {
        return new AtomicUInt64(value);
    }

    public static implicit operator ulong(AtomicUInt64 atomic)
    {
        return atomic.Read();
    }

    public static explicit operator sbyte(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (sbyte)val;
    }

    public static explicit operator short(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (short)val;
    }

    public static explicit operator int(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (int)val;
    }

    public static explicit operator long(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (long)val;
    }

    public static explicit operator byte(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (byte)val;
    }

    public static explicit operator ushort(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (ushort)val;
    }

    public static explicit operator uint(AtomicUInt64 atomic)
    {
        ulong val = atomic;
        return (uint)val;
    }
}
