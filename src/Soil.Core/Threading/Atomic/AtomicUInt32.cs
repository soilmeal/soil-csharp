using System.Threading;

namespace Soil.Core.Threading.Atomic;

public struct AtomicUInt32 : IAtomicInteger<uint>
{
    private uint _value;

    public AtomicUInt32() : this(0U)
    {
    }

    public AtomicUInt32(uint initialValue)
    {
        _value = initialValue;
    }

    public uint Read()
    {
        return _value;
    }

    public uint Add(uint other)
    {
        return Interlocked.Add(ref _value, other);
    }

    public uint Increment()
    {
        return Add(1);
    }

    public uint Decrement()
    {
        return Interlocked.Decrement(ref _value);
    }

    public uint And(uint other)
    {
        return Interlocked.And(ref _value, other);
    }

    public uint Or(uint other)
    {
        return Interlocked.Or(ref _value, other);
    }

    public uint Exchange(uint other)
    {
        return Interlocked.Exchange(ref _value, other);
    }

    public uint CompareExchange(uint other, uint comparand)
    {
        return Interlocked.CompareExchange(ref _value, other, comparand);
    }

    public static implicit operator long(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return val;
    }

    public static implicit operator uint(AtomicUInt32 atomic)
    {
        return atomic.Read();
    }

    public static implicit operator ulong(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return val;
    }

    public static explicit operator sbyte(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return (sbyte)val;
    }

    public static explicit operator short(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return (short)val;
    }

    public static explicit operator int(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return (int)val;
    }

    public static explicit operator byte(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return (byte)val;
    }

    public static explicit operator ushort(AtomicUInt32 atomic)
    {
        uint val = atomic;
        return (ushort)val;
    }
}
