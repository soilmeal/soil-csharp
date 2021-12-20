using System.Threading;

namespace Soil.Core.Threading.Atomic;

public struct AtomicUInt64 : IAtomicInteger<ulong>
{
    private ulong _value;

    public AtomicUInt64() : this(0UL)
    {
    }

    public AtomicUInt64(ulong initialValue)
    {
        _value = initialValue;
    }

    public ulong Read()
    {
        return Interlocked.Read(ref _value);
    }

    public ulong Add(ulong other)
    {
        return Interlocked.Add(ref _value, other);
    }

    public ulong Increment()
    {
        return Add(1UL);
    }

    public ulong Decrement()
    {
        return Interlocked.Decrement(ref _value);
    }

    public ulong And(ulong other)
    {
        return Interlocked.And(ref _value, other);
    }

    public ulong Or(ulong other)
    {
        return Interlocked.Or(ref _value, other);
    }

    public ulong Exchange(ulong other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public ulong CompareExchange(ulong other, ulong comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
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
