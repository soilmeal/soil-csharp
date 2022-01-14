using System.Threading;

namespace Soil.Threading.Atomic;

public class AtomicUInt32 : IAtomicInteger<uint>
{
    private int _value;

    public AtomicUInt32() : this(0U)
    {
    }

    public AtomicUInt32(uint initialValue)
    {
        _value = ToInt32(initialValue);
    }

    private static int ToInt32(uint value)
    {
        return unchecked((int)value);
    }

    private static uint ToUInt32(int value)
    {
        return unchecked((uint)value);
    }

    public uint Read()
    {
        return ToUInt32(_value);
    }

    public uint Add(uint other)
    {
        return ToUInt32(Interlocked.Add(ref _value, (int)other));
    }

    public uint Increment()
    {
        return Add(1);
    }

    public uint Decrement()
    {
        return ToUInt32(Interlocked.Add(ref _value, -1));
    }

    public uint And(uint other)
    {
        // return Interlocked.And(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        uint prevValue;
        uint afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue & other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public uint Or(uint other)
    {
        // return Interlocked.Or(ref _value, other);

        // Dotnet runtime implement this operation using spin-lock.
        // Therefore, we also implement this ourselve using spin-lock.

        uint prevValue;
        uint afterValue;
        do
        {
            prevValue = Read();
            afterValue = prevValue | other;
        } while (prevValue != CompareExchange(afterValue, prevValue));

        return afterValue;
    }

    public uint Exchange(uint other)
    {
        return ToUInt32(Interlocked.Exchange(ref _value, ToInt32(other)));
    }

    public uint CompareExchange(uint other, uint comparand)
    {
        return ToUInt32(Interlocked.CompareExchange(
            ref _value,
            ToInt32(other),
            ToInt32(comparand)));
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
