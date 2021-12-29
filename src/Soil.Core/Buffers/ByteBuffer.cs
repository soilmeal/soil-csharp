using System;
using System.Buffers.Binary;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Buffers;

public abstract partial class ByteBuffer : IByteBuffer<ByteBuffer>
{
    private static readonly byte[] _defaultBuffer = Array.Empty<byte>();

    private readonly ILogger<ByteBuffer> _logger;

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

    public UnsafeOp Unsafe
    {
        get
        {
            return _unsafe;
        }
    }

    public IByteBufferAllocator<ByteBuffer> Allocator
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
            return ReferenceEquals(_buffer, _defaultBuffer) || _endianless == Endianless.None;
        }
    }

    protected ByteBuffer(ByteBufferAllocator allocator, ILoggerFactory loggerFactory)
    {
        _logger = loggerFactory.CreateLogger<ByteBuffer>();

        _buffer = _defaultBuffer;
        _endianless = Endianless.None;
        _unsafe = new(this, allocator, loggerFactory);

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

    public void EnsureCapacity(int length)
    {
        if (Writable(length))
        {
            return;
        }

        // if (Capacity >= MaxCapacity)
        // {
        //     throw new InvalidBufferOperation(InvalidBufferOperation.MaxCapacityReached);
        // }

        // int newCapacity = BufferUtilities.ComputeNextCapacity(Capacity);
        // if (newCapacity <= 0)
        // {
        //     throw new InvalidBufferOperation(InvalidBufferOperation.MaxCapacityReached);
        // }

        // byte[] newBuf = new byte[newCapacity];
        // Array.Copy(_buffer, 0, newBuf, 0, _buffer.Length);

        // _buffer = newBuf;

        _unsafe.Reallocate(length);
    }

    public byte ReadByte()
    {
        int length = sizeof(byte);
        return Readable(length)
            ? _buffer[_readIdx++]
            : throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
    }

    public sbyte ReadSByte()
    {
        int length = sizeof(sbyte);
        return Readable(length)
            ? (sbyte)_buffer[_readIdx++]
            : throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
    }

    public Memory<byte> ReadBytes(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }

        if (!Readable(length))
        {
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        byte[] bytes = new byte[length];

        ReadBytes(ref bytes, length);

        return bytes.AsMemory();
    }

    public int ReadBytes(ref byte[] dest)
    {
        return dest != null
            ? ReadBytes(ref dest, 0, Math.Min(dest.Length, ReadableBytes))
            : throw new ArgumentNullException(nameof(dest));
    }

    public int ReadBytes(ref byte[] dest, int length)
    {
        return ReadBytes(ref dest, 0, length);
    }

    public int ReadBytes(ref byte[] dest, int destIndex, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (!Readable(length))
        {
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(length);
        srcSlice.CopyTo(dest);

        _readIdx += length;

        return length;
    }

    public int ReadBytes(ref Span<byte> dest)
    {
        return ReadBytes(ref dest, Math.Min(dest.Length, ReadableBytes));
    }

    public int ReadBytes(ref Span<byte> dest, int length)
    {
        return ReadBytes(ref dest, 0, length);
    }

    public int ReadBytes(ref Span<byte> dest, int destIndex, int length)
    {
        if (!Readable(length))
        {
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(length);
        srcSlice.CopyTo(dest[destIndex..]);

        _readIdx += length;

        return length;
    }

    public int ReadBytes(ref Memory<byte> dest)
    {
        return ReadBytes(ref dest, Math.Min(dest.Length, ReadableBytes));
    }

    public int ReadBytes(ref Memory<byte> dest, int length)
    {
        return ReadBytes(ref dest, 0, length);
    }

    public int ReadBytes(ref Memory<byte> dest, int destIndex, int length)
    {
        if (!Readable(length))
        {
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(length);
        srcSlice.CopyTo(dest.Span[destIndex..]);

        _readIdx += length;

        return length;
    }

    public int ReadBytes<TAnotherDerived>(ref TAnotherDerived dest)
        where TAnotherDerived : struct, IByteBuffer<TAnotherDerived>
    {
        return ReadBytes(ref dest, dest.WritableBytes);
    }

    public int ReadBytes<TAnotherDerived>(ref TAnotherDerived dest, int length)
        where TAnotherDerived : struct, IByteBuffer<TAnotherDerived>
    {
        if (!Readable(length))
        {
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        short result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadInt16BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadInt16LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ushort result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadUInt16BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadUInt16LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        int result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadInt32BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadInt32LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        uint result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadUInt32BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadUInt32LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        long result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadInt64BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadInt64LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        ulong result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadUInt64BigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadUInt64LittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        float result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadSingleBigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadSingleLittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
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
            throw new InvalidBufferOperation(InvalidBufferOperation.ReadIndexExceed, _readIdx, _writtenIdx, length);
        }

        double result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitives.ReadDoubleBigEndian(AsReadableSlice(length)),
            Endianless.LittleEndian => BinaryPrimitives.ReadDoubleLittleEndian(AsReadableSlice(length)),
            _ => throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless),
        };

        _readIdx += length;

        return result;
    }

    public ByteBuffer WriteByte(byte value)
    {
        int length = sizeof(byte);
        EnsureCapacity(length);

        Span<byte> slice = AsWritableSlice();
        slice[0] = value;

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteSByte(sbyte value)
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
        Span<byte> bufSlice = AsWritableSlice();
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
        Span<byte> bufSlice = AsWritableSlice();
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
        Span<byte> bufSlice = AsWritableSlice();
        srcSlice.CopyTo(bufSlice);

        _writtenIdx += length;

        return length;
    }

    public int WriteBytes<TAnotherDerived>(ref TAnotherDerived src)
        where TAnotherDerived : struct, IReadOnlyByteBuffer<TAnotherDerived>
    {
        return WriteBytes(ref src, src.ReadableBytes);
    }

    public int WriteBytes<TAnotherDerived>(ref TAnotherDerived src, int length)
        where TAnotherDerived : struct, IReadOnlyByteBuffer<TAnotherDerived>
    {
        EnsureCapacity(length);

        int result = src.ReadBytes(ref _buffer, _writtenIdx, length);

        _writtenIdx += length;

        return result;
    }

    public ByteBuffer WriteChar(char value)
    {
        return WriteChar(value, _endianless);
    }

    public ByteBuffer WriteChar(char value, Endianless endianless)
    {
        return WriteUInt16(value, endianless);
    }

    public ByteBuffer WriteInt16(short value)
    {
        return WriteInt16(value, _endianless);
    }

    public ByteBuffer WriteInt16(short value, Endianless endianless)
    {
        int length = sizeof(short);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteInt16BigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteInt16LittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteUInt16(ushort value)
    {
        return WriteUInt16(value, _endianless);
    }

    public ByteBuffer WriteUInt16(ushort value, Endianless endianless)
    {
        int length = sizeof(ushort);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteUInt16BigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteUInt16LittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteInt32(int value)
    {
        return WriteInt32(value, _endianless);
    }

    public ByteBuffer WriteInt32(int value, Endianless endianless)
    {
        int length = sizeof(int);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteInt32BigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteInt32LittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteUInt32(uint value)
    {
        return WriteUInt32(value, _endianless);
    }

    public ByteBuffer WriteUInt32(uint value, Endianless endianless)
    {
        int length = sizeof(uint);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteUInt32BigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteUInt32LittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteInt64(long value)
    {
        return WriteInt64(value, _endianless);
    }

    public ByteBuffer WriteInt64(long value, Endianless endianless)
    {
        int length = sizeof(long);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteInt64BigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteInt64LittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteUInt64(ulong value)
    {
        return WriteUInt64(value, _endianless);
    }

    public ByteBuffer WriteUInt64(ulong value, Endianless endianless)
    {
        int length = sizeof(ulong);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteUInt64BigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteUInt64LittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteSingle(float value)
    {
        return WriteSingle(value, _endianless);
    }

    public ByteBuffer WriteSingle(float value, Endianless endianless)
    {
        int length = sizeof(float);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteSingleBigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteSingleLittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public ByteBuffer WriteDouble(double value)
    {
        return WriteDouble(value, _endianless);
    }

    public ByteBuffer WriteDouble(double value, Endianless endianless)
    {
        int length = sizeof(double);
        EnsureCapacity(length);

        switch (endianless)
        {
            case Endianless.BigEndian:
                {
                    BinaryPrimitives.WriteDoubleBigEndian(AsWritableSlice(length), value);
                    break;
                }
            case Endianless.LittleEndian:
                {
                    BinaryPrimitives.WriteDoubleLittleEndian(AsWritableSlice(length), value);
                    break;
                }
            default:
                {
                    throw new InvalidBufferOperation(InvalidBufferOperation.InvalidEndianless);
                }
        }

        _writtenIdx += length;

        return this;
    }

    public void Clear()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperation(InvalidBufferOperation.Uninitialized);
        }

        _readIdx = 0;
        _writtenIdx = 0;

        Array.Fill<byte>(_buffer, 0);
    }

    public void Release()
    {
        Allocator.Unsafe.Release(this);
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
        return BufferUtilities.SpanSlice(_buffer, _readIdx, length);
    }

    private Span<byte> AsWritableSlice()
    {
        return AsWritableSlice(WritableBytes);
    }

    private Span<byte> AsWritableSlice(int length)
    {
        return BufferUtilities.SpanSlice(_buffer, _writtenIdx, length);
    }
}
