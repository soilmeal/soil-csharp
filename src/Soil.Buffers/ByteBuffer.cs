using System;
using Soil.Buffers.Helper;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer
{
    private static readonly byte[] _defaultBuffer = Array.Empty<byte>();

    private byte[] _buffer;

    private int _readIdx = 0;

    private int _writtenIdx = 0;

    private Endianless _endianless;

    private readonly UnsafeOp _unsafe;

    public int ReadIndex
    {
        get
        {
            return _readIdx;
        }
    }

    public int ReadableBytes
    {
        get
        {
            return _writtenIdx - _readIdx;
        }
    }

    public int WrittenIndex
    {
        get
        {
            return _writtenIdx;
        }
    }

    public int WritableBytes
    {
        get
        {
            return Capacity - _writtenIdx;
        }
    }

    public int Capacity
    {
        get
        {
            return _buffer.Length;
        }
    }

    public int MaxCapacity
    {
        get
        {
            return MaxCapacity;
        }
    }

    public Endianless Endianless
    {
        get
        {
            return _endianless;
        }
    }

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _unsafe.Allocator;
        }
    }

    public bool IsInitialized
    {
        get
        {
            return !ReferenceEquals(_buffer, _defaultBuffer) && _endianless != Endianless.None;
        }
    }

    public IReadOnlyByteBuffer.IReadOnlyUnsafeOp ReadOnlyUnsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public IByteBuffer.IUnsafeOp Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    protected ByteBuffer(ByteBufferAllocator allocator)
    {
        _buffer = _defaultBuffer;
        _endianless = Endianless.None;
        _unsafe = new(this, allocator);

        _readIdx = 0;
        _writtenIdx = 0;
    }

    public bool Readable()
    {
        return ReadableBytes > 0;
    }

    public bool Readable(int length)
    {
        return ReadableBytes >= length;
    }

    public bool Writable()
    {
        return WritableBytes > 0;
    }

    public bool Writable(int length)
    {
        return WritableBytes >= length;
    }

    public void EnsureCapacity()
    {
        if (Writable())
        {
            return;
        }

        _unsafe.Reallocate();
    }

    public void EnsureCapacity(int length)
    {
        if (Writable(length))
        {
            return;
        }

        _unsafe.Reallocate();
    }

    public byte ReadByte()
    {
        int length = sizeof(byte);
        return Readable(length)
            ? AsReadableSlice()[_readIdx++]
            : throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
    }

    public sbyte ReadSByte()
    {
        int length = sizeof(sbyte);
        return Readable(length)
            ? (sbyte)AsReadableSlice()[_readIdx++]
            : throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
    }

    public Memory<byte> ReadBytes(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }

        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        byte[] bytes = new byte[length];
        ReadBytes(bytes, length);

        return bytes.AsMemory();
    }

    public int ReadBytes(byte[] dest)
    {
        return dest != null
            ? ReadBytes(dest, 0, Math.Min(dest.Length, ReadableBytes))
            : throw new ArgumentNullException(nameof(dest));
    }

    public int ReadBytes(byte[] dest, int length)
    {
        return ReadBytes(dest, 0, length);
    }

    public int ReadBytes(byte[] dest, int destIndex, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        _readIdx += length;

        return length;
    }

    public int ReadBytes(Span<byte> dest)
    {
        return ReadBytes(dest, Math.Min(dest.Length, ReadableBytes));
    }

    public int ReadBytes(Span<byte> dest, int length)
    {
        return ReadBytes(dest, 0, length);
    }

    public int ReadBytes(Span<byte> dest, int destIndex, int length)
    {
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        _readIdx += length;

        return length;
    }

    public int ReadBytes(Memory<byte> dest)
    {
        return ReadBytes(dest, Math.Min(dest.Length, ReadableBytes));
    }

    public int ReadBytes(Memory<byte> dest, int length)
    {
        return ReadBytes(dest, 0, length);
    }

    public int ReadBytes(Memory<byte> dest, int destIndex, int length)
    {
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        _readIdx += length;

        return length;
    }

    public int ReadBytes(IByteBuffer dest)
    {
        return ReadBytes(dest, dest.WritableBytes);
    }

    public int ReadBytes(IByteBuffer dest, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        int result = dest.WriteBytes(_buffer, _readIdx, length);

        _readIdx += length;

        return result;
    }

    public char ReadChar()
    {
        return (char)ReadUInt16();
    }

    public char ReadChar(Endianless endianless)
    {
        return (char)ReadUInt16(endianless);
    }

    public short ReadInt16()
    {
        return ReadInt16(_endianless);
    }

    public short ReadInt16(Endianless endianless)
    {
        int length = sizeof(short);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        short result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadInt16BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadInt16LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public ushort ReadUInt16()
    {
        return ReadUInt16(_endianless);
    }

    public ushort ReadUInt16(Endianless endianless)
    {
        int length = sizeof(ushort);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ushort result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadUInt16BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadUInt16LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public int ReadInt32()
    {
        return ReadInt32(_endianless);
    }

    public int ReadInt32(Endianless endianless)
    {
        int length = sizeof(int);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        int result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadInt32BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadInt32LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public uint ReadUInt32()
    {
        return ReadUInt32(_endianless);
    }

    public uint ReadUInt32(Endianless endianless)
    {
        int length = sizeof(uint);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        uint result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadUInt32BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadUInt32LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public long ReadInt64()
    {
        return ReadInt64(_endianless);
    }

    public long ReadInt64(Endianless endianless)
    {
        int length = sizeof(long);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        long result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadInt64BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadInt64LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public ulong ReadUInt64()
    {
        return ReadUInt64(_endianless);
    }

    public ulong ReadUInt64(Endianless endianless)
    {
        int length = sizeof(ulong);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ulong result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadUInt64BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadUInt64LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public float ReadSingle()
    {
        return ReadSingle(_endianless);
    }

    public float ReadSingle(Endianless endianless)
    {
        int length = sizeof(float);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        float result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadSingleBigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadSingleLittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public double ReadDouble()
    {
        return ReadDouble(_endianless);
    }

    public double ReadDouble(Endianless endianless)
    {
        int length = sizeof(double);
        if (!Readable(length))
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        double result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadDoubleBigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadDoubleLittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public int GetBytes(int index, byte[] dest)
    {
        return GetBytes(index, dest, Math.Min(Capacity - index, dest.Length));
    }

    public int GetBytes(int index, byte[] dest, int length)
    {
        return GetBytes(index, dest, 0, length);
    }

    public int GetBytes(int index, byte[] dest, int destIndex, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(index, length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        return length;
    }

    public int GetBytes(int index, Span<byte> dest)
    {
        return GetBytes(index, dest, Math.Min(Capacity - index, dest.Length));
    }

    public int GetBytes(int index, Span<byte> dest, int length)
    {
        return GetBytes(index, dest, 0, length);
    }

    public int GetBytes(int index, Span<byte> dest, int destIndex, int length)
    {
        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(index, length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        return length;
    }

    public int GetBytes(int index, Memory<byte> dest)
    {
        return GetBytes(index, dest, Math.Min(Capacity - index, dest.Length));
    }

    public int GetBytes(int index, Memory<byte> dest, int length)
    {
        return GetBytes(index, dest, 0, length);
    }

    public int GetBytes(int index, Memory<byte> dest, int destIndex, int length)
    {
        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(index, length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        return length;
    }

    public IByteBuffer WriteByte(byte value)
    {
        int length = sizeof(byte);
        EnsureCapacity(length);

        Span<byte> slice = AsWritableSlice();
        slice[0] = value;

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteSByte(sbyte value)
    {
        int length = sizeof(sbyte);
        EnsureCapacity(length);

        Span<byte> slice = AsWritableSlice();
        slice[0] = (byte)value;

        _writtenIdx += length;

        return this;
    }

    public int WriteBytes(byte[] src)
    {
        return WriteBytes(src, WritableBytes);
    }

    public int WriteBytes(byte[] src, int length)
    {
        return WriteBytes(src, 0, length);
    }

    public int WriteBytes(byte[] src, int srcIndex, int length)
    {
        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        EnsureCapacity(length);

        ReadOnlySpan<byte> srcSlice = BufferUtilities.SpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(length);
        srcSlice.CopyTo(bufSlice);

        _writtenIdx += length;

        return length;
    }

    public int WriteBytes(ReadOnlySpan<byte> src)
    {
        return WriteBytes(src, WritableBytes);
    }

    public int WriteBytes(ReadOnlySpan<byte> src, int length)
    {
        return WriteBytes(src, 0, length);
    }

    public int WriteBytes(ReadOnlySpan<byte> src, int srcIndex, int length)
    {
        EnsureCapacity(length);

        ReadOnlySpan<byte> srcSlice = BufferUtilities.ReadOnlySpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(length);
        srcSlice.CopyTo(bufSlice);

        _writtenIdx += length;

        return length;
    }

    public int WriteBytes(ReadOnlyMemory<byte> src)
    {
        return WriteBytes(src, WritableBytes);
    }

    public int WriteBytes(ReadOnlyMemory<byte> src, int length)
    {
        return WriteBytes(src, 0, length);
    }

    public int WriteBytes(ReadOnlyMemory<byte> src, int srcIndex, int length)
    {
        EnsureCapacity(length);

        ReadOnlySpan<byte> srcSlice = BufferUtilities.ReadOnlySpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(length);
        srcSlice.CopyTo(bufSlice);

        _writtenIdx += length;

        return length;
    }

    public int WriteBytes(IByteBuffer src)
    {
        return WriteBytes(src, src.ReadableBytes);
    }

    public int WriteBytes(IByteBuffer src, int length)
    {
        EnsureCapacity(length);

        int result = src.ReadBytes(_buffer, _writtenIdx, length);

        _writtenIdx += length;

        return result;
    }

    public IByteBuffer WriteChar(char value)
    {
        return WriteChar(value, _endianless);
    }

    public IByteBuffer WriteChar(char value, Endianless endianless)
    {
        return WriteUInt16(value, endianless);
    }

    public IByteBuffer WriteInt16(short value)
    {
        return WriteInt16(value, _endianless);
    }

    public IByteBuffer WriteInt16(short value, Endianless endianless)
    {
        int length = sizeof(short);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteInt16BigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteInt16LittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteUInt16(ushort value)
    {
        return WriteUInt16(value, _endianless);
    }

    public IByteBuffer WriteUInt16(ushort value, Endianless endianless)
    {
        int length = sizeof(ushort);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteUInt16BigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteUInt16LittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteInt32(int value)
    {
        return WriteInt32(value, _endianless);
    }

    public IByteBuffer WriteInt32(int value, Endianless endianless)
    {
        int length = sizeof(int);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteInt32BigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteInt32LittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteUInt32(uint value)
    {
        return WriteUInt32(value, _endianless);
    }

    public IByteBuffer WriteUInt32(uint value, Endianless endianless)
    {
        int length = sizeof(uint);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteUInt32BigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteUInt32LittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteInt64(long value)
    {
        return WriteInt64(value, _endianless);
    }

    public IByteBuffer WriteInt64(long value, Endianless endianless)
    {
        int length = sizeof(long);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteInt64BigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteInt64LittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteUInt64(ulong value)
    {
        return WriteUInt64(value, _endianless);
    }

    public IByteBuffer WriteUInt64(ulong value, Endianless endianless)
    {
        int length = sizeof(ulong);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteUInt64BigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteUInt64LittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteSingle(float value)
    {
        return WriteSingle(value, _endianless);
    }

    public IByteBuffer WriteSingle(float value, Endianless endianless)
    {
        int length = sizeof(float);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteSingleBigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteSingleLittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public IByteBuffer WriteDouble(double value)
    {
        return WriteDouble(value, _endianless);
    }

    public IByteBuffer WriteDouble(double value, Endianless endianless)
    {
        int length = sizeof(double);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteDoubleBigEndian(AsWritableSlice(length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteDoubleLittleEndian(AsWritableSlice(length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        _writtenIdx += length;

        return this;
    }

    public int SetBytes(int index, byte[] src)
    {
        return SetBytes(index, src, Math.Min(Capacity - index, src.Length));
    }

    public int SetBytes(int index, byte[] src, int length)
    {
        return SetBytes(index, src, 0, length);
    }

    public int SetBytes(int index, byte[] src, int srcIndex, int length)
    {
        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        ReadOnlySpan<byte> srcSlice = BufferUtilities.ReadOnlySpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(index, length);
        srcSlice.CopyTo(bufSlice);

        return length;
    }

    public int SetBytes(int index, ReadOnlySpan<byte> src)
    {
        return SetBytes(index, src, Math.Min(Capacity - index, src.Length));
    }

    public int SetBytes(int index, ReadOnlySpan<byte> src, int length)
    {
        return SetBytes(index, src, 0, length);
    }

    public int SetBytes(int index, ReadOnlySpan<byte> src, int srcIndex, int length)
    {
        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        ReadOnlySpan<byte> srcSlice = BufferUtilities.ReadOnlySpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(index, length);
        srcSlice.CopyTo(bufSlice);

        return length;
    }

    public int SetBytes(int index, ReadOnlyMemory<byte> src)
    {
        return SetBytes(index, src, Math.Min(Capacity - index, src.Length));
    }

    public int SetBytes(int index, ReadOnlyMemory<byte> src, int length)
    {
        return SetBytes(index, src, 0, length);
    }

    public int SetBytes(int index, ReadOnlyMemory<byte> src, int srcIndex, int length)
    {
        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException("index + length out of range");
        }

        ReadOnlySpan<byte> srcSlice = BufferUtilities.ReadOnlySpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(index, length);
        srcSlice.CopyTo(bufSlice);

        return length;
    }

    public void Clear()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.Uninitialized);
        }

        _readIdx = 0;
        _writtenIdx = 0;

        Array.Fill<byte>(_buffer, 0);
    }

    public void Release()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.ReleaseTwice);
        }

        Clear();

        byte[] buffer = _buffer;
        _buffer = _defaultBuffer;
        _endianless = Endianless.None;

        Allocator.Unsafe.Return(this, buffer);
    }

    public void ResetReadIndex()
    {
        _readIdx = 0;
    }

    public void ResetWrittenIndex()
    {
        _writtenIdx = 0;
    }

    private ReadOnlySpan<byte> AsReadableSlice()
    {
        return AsReadableSlice(ReadableBytes);
    }

    private ReadOnlySpan<byte> AsReadableSlice(int length)
    {
        return AsReadableSlice(_readIdx, length);
    }

    private ReadOnlySpan<byte> AsReadableSlice(int readIdx, int length)
    {
        return BufferUtilities.SpanSlice(_buffer, readIdx, length);
    }

    private Span<byte> AsWritableSlice()
    {
        return AsWritableSlice(WritableBytes);
    }

    private Span<byte> AsWritableSlice(int length)
    {
        return AsWritableSlice(_writtenIdx, length);
    }

    private Span<byte> AsWritableSlice(int writtenIdx, int length)
    {
        return BufferUtilities.SpanSlice(_buffer, writtenIdx, length);
    }
}
