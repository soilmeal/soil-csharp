using System.Threading;

namespace Soil.Threading.Atomic;

public struct AtomicInt32 : IAtomicInteger<int>
{
    private int _value;

    public AtomicInt32() : this(0)
    {
    }

    public AtomicInt32(int initialValue)
    {
        _value = initialValue;
    }

    public int Read()
    {
        return _value;
    }

    public int Add(int other)
    {
        return Interlocked.Add(ref _value, other);
    }

    public int Increment()
    {
        return Add(1);
    }

    public int Decrement()
    {
        return Interlocked.Decrement(ref _value);
    }

    public int And(int other)
    {
        // return Interlocked.And(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        int prevValue;
        int afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue & other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public int Or(int other)
    {
        // return Interlocked.Or(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        int prevValue;
        int afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue | other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public int Exchange(int other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public int CompareExchange(int other, int comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }

    public static implicit operator int(AtomicInt32 atomic)
    {
        return atomic.Read();
    }

    public static implicit operator long(AtomicInt32 atomic)
    {
        int val = atomic;
        return val;
    }

    public static implicit operator float(AtomicInt32 atomic)
    {
        int val = atomic;
        return val;
    }

    public static implicit operator double(AtomicInt32 atomic)
    {
        int val = atomic;
        return val;
    }

    public static explicit operator sbyte(AtomicInt32 atomic)
    {
        int val = atomic;
        return (sbyte)val;
    }

    public static explicit operator short(AtomicInt32 atomic)
    {
        int value = atomic;
        return (short)value;
    }

    public static explicit operator byte(AtomicInt32 atomic)
    {
        int value = atomic;
        return (byte)value;
    }

    public static explicit operator ushort(AtomicInt32 atomic)
    {
        int value = atomic;
        return (ushort)value;
    }

    public static explicit operator uint(AtomicInt32 atomic)
    {
        int val = atomic;
        return (uint)val;
    }

    public static explicit operator ulong(AtomicInt32 atomic)
    {
        int val = atomic;
        return (ulong)val;
    }
}
