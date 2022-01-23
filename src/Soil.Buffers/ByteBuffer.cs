using System;
using Soil.Buffers.Helper;

namespace Soil.Buffers;

public abstract partial class ByteBuffer : IByteBuffer
{
    private byte[] _buffer;

    private int _readIdx = 0;

    private int _writeIdx = 0;

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
            return _writeIdx - _readIdx;
        }
    }

    public int WriteIndex
    {
        get
        {
            return _writeIdx;
        }
    }

    public int WritableBytes
    {
        get
        {
            return Capacity - _writeIdx;
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
            return Constants.MaxCapacity;
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
            return !ReferenceEquals(_buffer, Constants.DefaultBuffer) && _endianless != Endianless.None;
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
        _buffer = Constants.DefaultBuffer;
        _endianless = Endianless.None;
        _unsafe = new(this, allocator);

        _readIdx = 0;
        _writeIdx = 0;
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

    public byte GetByte(int index)
    {
        GetByteInternal(index, out byte result);
        return result;
    }

    public sbyte GetSByte(int index)
    {
        GetSByteInternal(index, out sbyte result);
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
        return GetBytesInternal(index, dest, destIndex, length);
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
        return GetBytesInternal(index, dest, destIndex, length);
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
        return GetBytesInternal(index, dest, destIndex, length);
    }

    public int GetBytes(int index, IByteBuffer dest)
    {
        return GetBytes(index, dest, Math.Min(Capacity - index, dest.WritableBytes));
    }

    public int GetBytes(int index, IByteBuffer dest, int length)
    {
        return GetBytesInternal(index, dest, length);
    }

    public int GetBytes(int index, IByteBuffer dest, int destIndex, int length)
    {
        return GetBytesInternal(index, dest, destIndex, length);
    }

    public char GetChar(int index)
    {
        return GetChar(index, _endianless);
    }

    public char GetChar(int index, Endianless endianless)
    {
        return (char)GetUInt16(index, endianless);
    }

    public short GetInt16(int index)
    {
        return GetInt16(index, _endianless);
    }

    public short GetInt16(int index, Endianless endianless)
    {
        GetInt16Internal(index, endianless, out short result);
        return result;
    }

    public ushort GetUInt16(int index)
    {
        return GetUInt16(index, _endianless);
    }

    public ushort GetUInt16(int index, Endianless endianless)
    {
        GetUInt16Internal(index, endianless, out ushort result);
        return result;
    }

    public int GetInt32(int index)
    {
        return GetInt32(index, _endianless);
    }

    public int GetInt32(int index, Endianless endianless)
    {
        GetInt32Internal(index, endianless, out int result);
        return result;
    }

    public uint GetUInt32(int index)
    {
        return GetUInt32(index, _endianless);
    }

    public uint GetUInt32(int index, Endianless endianless)
    {
        GetUInt32Interanl(index, endianless, out uint result);
        return result;
    }

    public long GetInt64(int index)
    {
        return GetInt64(index, _endianless);
    }

    public long GetInt64(int index, Endianless endianless)
    {
        GetInt64Internal(index, endianless, out long result);
        return result;
    }

    public ulong GetUInt64(int index)
    {
        return GetUInt64(index, _endianless);
    }

    public ulong GetUInt64(int index, Endianless endianless)
    {
        GetUInt64Internal(index, endianless, out ulong result);
        return result;
    }

    public float GetSingle(int index)
    {
        return GetSingle(index, _endianless);
    }

    public float GetSingle(int index, Endianless endianless)
    {
        GetSingleInternal(index, endianless, out float result);
        return result;
    }

    public double GetDouble(int index)
    {
        return GetDouble(index, _endianless);
    }

    public double GetDouble(int index, Endianless endianless)
    {
        GetDoubleInternal(index, endianless, out double result);
        return result;
    }

    public byte ReadByte()
    {
        if (!Readable())
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetByteInternal(_readIdx++, out byte result);
        return result;
    }

    public sbyte ReadSByte()
    {
        if (!Readable())
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetSByteInternal(_readIdx++, out sbyte result);
        return result;
    }

    public Memory<byte> ReadBytes(int length)
    {
        if (length <= 0)
        {
            throw new ArgumentOutOfRangeException(nameof(length), length, null);
        }

        byte[] bytes = new byte[length];
        ReadBytes(bytes, length);

        return bytes.AsMemory();
    }

    public int ReadBytes(byte[] dest)
    {
        return ReadBytes(dest, Math.Min(ReadableBytes, dest.Length));
    }

    public int ReadBytes(byte[] dest, int length)
    {
        return ReadBytes(dest, 0, length);
    }

    public int ReadBytes(byte[] dest, int destIndex, int length)
    {
        if (!Readable(length))
        {
            throw new IndexOutOfRangeException();
        }

        int result = GetBytesInternal(_readIdx, dest, destIndex, length);
        _readIdx += result;

        return result;
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
            throw new IndexOutOfRangeException();
        }

        int result = GetBytesInternal(_readIdx, dest, destIndex, length);
        _readIdx += result;

        return result;
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
            throw new IndexOutOfRangeException();
        }

        int result = GetBytesInternal(_readIdx, dest, destIndex, length);
        _readIdx += result;

        return result;
    }

    public int ReadBytes(IByteBuffer dest)
    {
        return ReadBytes(dest, Math.Min(ReadableBytes, dest.WritableBytes));
    }

    public int ReadBytes(IByteBuffer dest, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (!Readable(length))
        {
            throw new IndexOutOfRangeException();
        }

        int result = GetBytesInternal(_readIdx, dest, length);
        _readIdx += result;

        return result;
    }

    public int ReadBytes(IByteBuffer dest, int destIndex, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        if (!Readable(length))
        {
            throw new IndexOutOfRangeException();
        }

        int result = GetBytesInternal(_readIdx, dest, destIndex, length);
        _readIdx += result;

        return result;
    }

    public char ReadChar()
    {
        return ReadChar(_endianless);
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
        if (!Readable(sizeof(short)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetInt16Internal(_readIdx, endianless, out short result);
        return result;
    }

    public ushort ReadUInt16()
    {
        return ReadUInt16(_endianless);
    }

    public ushort ReadUInt16(Endianless endianless)
    {
        if (!Readable(sizeof(ushort)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetUInt16Internal(_readIdx, endianless, out ushort result);

        return result;
    }

    public int ReadInt32()
    {
        return ReadInt32(_endianless);
    }

    public int ReadInt32(Endianless endianless)
    {
        if (!Readable(sizeof(int)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetInt32Internal(_readIdx, endianless, out int result);

        return result;
    }

    public uint ReadUInt32()
    {
        return ReadUInt32(_endianless);
    }

    public uint ReadUInt32(Endianless endianless)
    {
        if (!Readable(sizeof(uint)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetUInt32Interanl(_readIdx, endianless, out uint result);

        return result;
    }

    public long ReadInt64()
    {
        return ReadInt64(_endianless);
    }

    public long ReadInt64(Endianless endianless)
    {
        if (!Readable(sizeof(long)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetInt64Internal(_readIdx, endianless, out long result);

        return result;
    }

    public ulong ReadUInt64()
    {
        return ReadUInt64(_endianless);
    }

    public ulong ReadUInt64(Endianless endianless)
    {
        if (!Readable(sizeof(ulong)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetUInt64Internal(_readIdx, endianless, out ulong result);

        return result;
    }

    public float ReadSingle()
    {
        return ReadSingle(_endianless);
    }

    public float ReadSingle(Endianless endianless)
    {
        if (!Readable(sizeof(float)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetSingleInternal(_readIdx, endianless, out float result);

        return result;
    }

    public double ReadDouble()
    {
        return ReadDouble(_endianless);
    }

    public double ReadDouble(Endianless endianless)
    {
        if (!Readable(sizeof(double)))
        {
            throw new IndexOutOfRangeException();
        }

        _readIdx += GetDoubleInternal(_readIdx, endianless, out double result);

        return result;
    }

    public IByteBuffer SetByte(int index, byte value)
    {
        SetByteInternal(index, value);

        return this;
    }

    public IByteBuffer SetSByte(int index, sbyte value)
    {
        SetSByteInternal(index, value);

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
        return SetBytesInternal(index, src, srcIndex, length);
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
        return SetBytesInternal(index, src, srcIndex, length);
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
        return SetBytesInternal(index, src, srcIndex, length);
    }

    public int SetBytes(int index, IReadOnlyByteBuffer src)
    {
        return SetBytes(index, src, Math.Min(Capacity - index, src.ReadableBytes));
    }

    public int SetBytes(int index, IReadOnlyByteBuffer src, int length)
    {
        return SetBytesInternal(index, src, length);
    }

    public int SetBytes(int index, IReadOnlyByteBuffer src, int srcIndex, int length)
    {
        return SetBytesInternal(index, src, srcIndex, length);
    }

    public IByteBuffer SetChar(int index, char value)
    {
        return SetChar(index, value, _endianless);
    }

    public IByteBuffer SetChar(int index, char value, Endianless endianless)
    {
        return SetUInt16(index, value, endianless);
    }

    public IByteBuffer SetInt16(int index, short value)
    {
        return SetInt16(index, value, _endianless);
    }

    public IByteBuffer SetInt16(int index, short value, Endianless endianless)
    {
        SetInt16Internal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetUInt16(int index, ushort value)
    {
        return SetUInt16(index, value, _endianless);
    }

    public IByteBuffer SetUInt16(int index, ushort value, Endianless endianless)
    {
        SetUInt16Internal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetInt32(int index, int value)
    {
        return SetInt32(index, value, _endianless);
    }

    public IByteBuffer SetInt32(int index, int value, Endianless endianless)
    {
        SetInt32Internal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetUInt32(int index, uint value)
    {
        return SetUInt32(index, value, _endianless);
    }

    public IByteBuffer SetUInt32(int index, uint value, Endianless endianless)
    {
        SetUInt32Internal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetInt64(int index, long value)
    {
        return SetInt64(index, value, _endianless);
    }

    public IByteBuffer SetInt64(int index, long value, Endianless endianless)
    {
        SetInt64Internal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetUInt64(int index, ulong value)
    {
        return SetUInt64(index, value, _endianless);
    }

    public IByteBuffer SetUInt64(int index, ulong value, Endianless endianless)
    {
        SetUInt64Internal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetSingle(int index, float value)
    {
        return SetSingle(index, value, _endianless);
    }

    public IByteBuffer SetSingle(int index, float value, Endianless endianless)
    {
        SetSingleInternal(index, value, endianless);

        return this;
    }

    public IByteBuffer SetDouble(int index, double value)
    {
        return SetDouble(index, value, _endianless);
    }

    public IByteBuffer SetDouble(int index, double value, Endianless endianless)
    {
        SetDoubleInternal(index, value, endianless);

        return this;
    }

    public IByteBuffer WriteByte(byte value)
    {
        EnsureCapacity(sizeof(byte));

        _writeIdx += SetByteInternal(_writeIdx, value);

        return this;
    }

    public IByteBuffer WriteSByte(sbyte value)
    {
        EnsureCapacity(sizeof(sbyte));

        _writeIdx += SetSByteInternal(_writeIdx, value);

        return this;
    }

    public int WriteBytes(byte[] src)
    {
        return WriteBytes(src, Math.Min(WritableBytes, src.Length));
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

        _writeIdx += SetBytesInternal(_writeIdx, src, srcIndex, length);

        return length;
    }

    public int WriteBytes(ReadOnlySpan<byte> src)
    {
        return WriteBytes(src, Math.Min(WritableBytes, src.Length));
    }

    public int WriteBytes(ReadOnlySpan<byte> src, int length)
    {
        return WriteBytes(src, 0, length);
    }

    public int WriteBytes(ReadOnlySpan<byte> src, int srcIndex, int length)
    {
        EnsureCapacity(length);

        _writeIdx += SetBytesInternal(_writeIdx, src, srcIndex, length);

        return length;
    }

    public int WriteBytes(ReadOnlyMemory<byte> src)
    {
        return WriteBytes(src, Math.Min(WritableBytes, src.Length));
    }

    public int WriteBytes(ReadOnlyMemory<byte> src, int length)
    {
        return WriteBytes(src, 0, length);
    }

    public int WriteBytes(ReadOnlyMemory<byte> src, int srcIndex, int length)
    {
        EnsureCapacity(length);

        _writeIdx += SetBytesInternal(_writeIdx, src, srcIndex, length);

        return length;
    }

    public int WriteBytes(IReadOnlyByteBuffer src)
    {
        return WriteBytes(src, src.ReadableBytes);
    }

    public int WriteBytes(IReadOnlyByteBuffer src, int length)
    {
        EnsureCapacity(length);

        int result = SetBytesInternal(_writeIdx, src, length);

        _writeIdx += length;

        return result;
    }

    public int WriteBytes(IReadOnlyByteBuffer src, int srcIndex, int length)
    {
        EnsureCapacity(length);

        int result = SetBytesInternal(_writeIdx, src, srcIndex, length);

        _writeIdx += result;

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
        EnsureCapacity(sizeof(short));

        _writeIdx += SetInt16Internal(_writeIdx, value, endianless);

        return this;
    }

    public IByteBuffer WriteUInt16(ushort value)
    {
        return WriteUInt16(value, _endianless);
    }

    public IByteBuffer WriteUInt16(ushort value, Endianless endianless)
    {
        EnsureCapacity(sizeof(ushort));

        _writeIdx += SetUInt16Internal(_writeIdx, value, endianless);

        return this;
    }

    public IByteBuffer WriteInt32(int value)
    {
        return WriteInt32(value, _endianless);
    }

    public IByteBuffer WriteInt32(int value, Endianless endianless)
    {
        EnsureCapacity(sizeof(int));

        _writeIdx += SetInt32Internal(_writeIdx, value, endianless);

        return this;
    }

    public IByteBuffer WriteUInt32(uint value)
    {
        return WriteUInt32(value, _endianless);
    }

    public IByteBuffer WriteUInt32(uint value, Endianless endianless)
    {
        EnsureCapacity(sizeof(uint));

        _writeIdx += SetUInt32Internal(_writeIdx, value, endianless);

        return this;
    }

    public IByteBuffer WriteInt64(long value)
    {
        return WriteInt64(value, _endianless);
    }

    public IByteBuffer WriteInt64(long value, Endianless endianless)
    {
        EnsureCapacity(sizeof(long));

        _writeIdx += SetInt64Internal(_writeIdx, value, endianless);

        return this;
    }

    public IByteBuffer WriteUInt64(ulong value)
    {
        return WriteUInt64(value, _endianless);
    }

    public IByteBuffer WriteUInt64(ulong value, Endianless endianless)
    {
        EnsureCapacity(sizeof(ulong));

        _writeIdx += SetUInt64Internal(_writeIdx, value, endianless);

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

        _writeIdx += SetSingleInternal(_writeIdx, value, endianless);

        return this;
    }

    public IByteBuffer WriteDouble(double value)
    {
        return WriteDouble(value, _endianless);
    }

    public IByteBuffer WriteDouble(double value, Endianless endianless)
    {
        EnsureCapacity(sizeof(double));

        _writeIdx += SetDoubleInternal(_writeIdx, value, endianless);

        return this;
    }

    public void Clear()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.Uninitialized);
        }

        _readIdx = 0;
        _writeIdx = 0;
    }

    public void Release()
    {
        if (!IsInitialized)
        {
            throw new InvalidBufferOperationException(InvalidBufferOperationException.AlreadyReleased);
        }

        Clear();

        byte[] buffer = _buffer;
        _buffer = Constants.DefaultBuffer;
        _endianless = Endianless.None;

        Allocator.Unsafe.Return(this, buffer);
    }

    public void ResetReadIndex()
    {
        _readIdx = 0;
    }

    public void ResetWriteIndex()
    {
        _writeIdx = 0;
    }

    private ReadOnlySpan<byte> AsReadableSlice(int readIdx, int length)
    {
        return BufferUtilities.SpanSlice(_buffer, readIdx, length);
    }

    private Span<byte> AsWritableSlice(int writeIdx, int length)
    {
        return BufferUtilities.SpanSlice(_buffer, writeIdx, length);
    }

    private void ThrowIfOutOfRange(int index, int length)
    {
        if (index < 0 || (index + length) >= Capacity)
        {
            throw new IndexOutOfRangeException();
        }
    }

    private int GetByteInternal(int index, out byte value)
    {
        int length = sizeof(byte);

        ThrowIfOutOfRange(index, length);

        value = AsReadableSlice(index, length)[0];

        return length;
    }

    private int GetSByteInternal(int index, out sbyte value)
    {
        int result = GetByteInternal(index, out byte outVal);
        value = unchecked((sbyte)outVal);
        return result;
    }

    private int GetBytesInternal(int index, byte[] dest, int destIndex, int length)
    {
        if (dest == null)
        {
            throw new ArgumentNullException(nameof(dest));
        }

        return GetBytesInternal(index, dest.AsSpan(), destIndex, length);
    }

    private int GetBytesInternal(int index, Span<byte> dest, int destIndex, int length)
    {
        ThrowIfOutOfRange(index, length);

        ReadOnlySpan<byte> srcSlice = AsReadableSlice(index, length);
        srcSlice.CopyTo(BufferUtilities.SpanSlice(dest, destIndex, length));

        return length;
    }

    private int GetBytesInternal(int index, Memory<byte> dest, int destIndex, int length)
    {
        return GetBytesInternal(index, dest.Span, destIndex, length);
    }

    private int GetBytesInternal(int index, IByteBuffer dest, int length)
    {
        ThrowIfOutOfRange(index, length);

        return dest.WriteBytes(_buffer, index, length);
    }

    private int GetBytesInternal(int index, IByteBuffer dest, int destIndex, int length)
    {
        ThrowIfOutOfRange(index, length);

        return dest.SetBytes(destIndex, _buffer, index, length);
    }

    private int GetInt16Internal(int index, Endianless endianless, out short result)
    {
        int length = sizeof(short);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadInt16BigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadInt16LittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetUInt16Internal(int index, Endianless endianless, out ushort result)
    {
        int length = sizeof(ushort);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadUInt16BigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadUInt16LittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetInt32Internal(int index, Endianless endianless, out int result)
    {
        int length = sizeof(int);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadInt32BigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadInt32LittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetUInt32Interanl(int index, Endianless endianless, out uint result)
    {
        int length = sizeof(uint);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadUInt32BigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadUInt32LittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetInt64Internal(int index, Endianless endianless, out long result)
    {
        int length = sizeof(long);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadInt64BigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadInt64LittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetUInt64Internal(int index, Endianless endianless, out ulong result)
    {
        int length = sizeof(ulong);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadUInt64BigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadUInt64LittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetSingleInternal(int index, Endianless endianless, out float result)
    {
        int length = sizeof(float);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadSingleBigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadSingleLittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int GetDoubleInternal(int index, Endianless endianless, out double result)
    {
        int length = sizeof(double);

        ThrowIfOutOfRange(index, length);

        result = endianless switch
        {
            Endianless.BigEndian => BinaryPrimitivesHelper.ReadDoubleBigEndian(AsReadableSlice(index, length)),
            Endianless.LittleEndian => BinaryPrimitivesHelper.ReadDoubleLittleEndian(AsReadableSlice(index, length)),
            _ => throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless),
        };

        return length;
    }

    private int SetByteInternal(int index, byte value)
    {
        int length = sizeof(byte);

        ThrowIfOutOfRange(index, length);

        Span<byte> slice = AsWritableSlice(index, length);
        slice[0] = value;

        return length;
    }

    private int SetSByteInternal(int index, sbyte value)
    {
        return SetByteInternal(index, unchecked((byte)value));
    }

    private int SetBytesInternal(int index, byte[] src, int srcIndex, int length)
    {
        if (src == null)
        {
            throw new ArgumentNullException(nameof(src));
        }

        return SetBytesInternal(index, src.AsSpan(), srcIndex, length);
    }

    private int SetBytesInternal(int index, ReadOnlySpan<byte> src, int srcIndex, int length)
    {
        ThrowIfOutOfRange(index, length);

        ReadOnlySpan<byte> srcSlice = BufferUtilities.ReadOnlySpanSlice(src, srcIndex, length);
        Span<byte> bufSlice = AsWritableSlice(index, length);
        srcSlice.CopyTo(bufSlice);

        return length;
    }

    private int SetBytesInternal(int index, ReadOnlyMemory<byte> src, int srcIndex, int length)
    {
        return SetBytesInternal(index, src.Span, srcIndex, length);
    }

    private int SetBytesInternal(int index, IReadOnlyByteBuffer src, int length)
    {
        ThrowIfOutOfRange(index, length);

        return src.ReadBytes(_buffer, index, length);
    }

    private int SetBytesInternal(int index, IReadOnlyByteBuffer src, int srcIndex, int length)
    {
        ThrowIfOutOfRange(index, length);

        return src.GetBytes(srcIndex, _buffer, index, length);
    }

    private int SetInt16Internal(int index, short value, Endianless endianless)
    {
        int length = sizeof(short);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteInt16BigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteInt16LittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetUInt16Internal(int index, ushort value, Endianless endianless)
    {
        int length = sizeof(ushort);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteUInt16BigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteUInt16LittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetInt32Internal(int index, int value, Endianless endianless)
    {
        int length = sizeof(int);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteInt32BigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteInt32LittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetUInt32Internal(int index, uint value, Endianless endianless)
    {
        int length = sizeof(uint);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteUInt32BigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteUInt32LittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetInt64Internal(int index, long value, Endianless endianless)
    {
        int length = sizeof(long);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteInt64BigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteInt64LittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetUInt64Internal(int index, ulong value, Endianless endianless)
    {
        int length = sizeof(ulong);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteUInt64BigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteUInt64LittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetSingleInternal(int index, float value, Endianless endianless)
    {
        int length = sizeof(float);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteSingleBigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteSingleLittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }

    private int SetDoubleInternal(int index, double value, Endianless endianless)
    {
        int length = sizeof(double);

        ThrowIfOutOfRange(index, length);

        switch (endianless)
        {
            case Endianless.BigEndian:
            {
                BinaryPrimitivesHelper.WriteDoubleBigEndian(AsWritableSlice(index, length), value);
                break;
            }
            case Endianless.LittleEndian:
            {
                BinaryPrimitivesHelper.WriteDoubleLittleEndian(AsWritableSlice(index, length), value);
                break;
            }
            default:
            {
                throw new InvalidBufferOperationException(InvalidBufferOperationException.InvalidEndianless);
            }
        }

        return length;
    }
}
