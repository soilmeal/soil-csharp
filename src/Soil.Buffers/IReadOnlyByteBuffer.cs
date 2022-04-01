using System;

namespace Soil.Buffers;

public interface IReadOnlyByteBuffer
{
    int ReadIndex { get; }

    int ReadableBytes { get; }

    Endianless Endianless { get; }

    IReadOnlyUnsafeOp ReadOnlyUnsafe { get; }

    bool Readable();

    bool Readable(int length);

    void DiscardReadBytes(int length);

    byte GetByte(int index);

    sbyte GetSByte(int index);

    char GetChar(int index);

    char GetChar(int index, Endianless endianless);

    short GetInt16(int index);

    short GetInt16(int index, Endianless endianless);

    ushort GetUInt16(int index);

    ushort GetUInt16(int index, Endianless endianless);

    int GetInt32(int index);

    int GetInt32(int index, Endianless endianless);

    uint GetUInt32(int index);

    uint GetUInt32(int index, Endianless endianless);

    long GetInt64(int index);

    long GetInt64(int index, Endianless endianless);

    ulong GetUInt64(int index);

    ulong GetUInt64(int index, Endianless endianless);

    float GetSingle(int index);

    float GetSingle(int index, Endianless endianless);

    double GetDouble(int index);

    double GetDouble(int index, Endianless endianless);

    int GetBytes(int index, byte[] dest);

    int GetBytes(int index, byte[] dest, int length);

    int GetBytes(int index, byte[] dest, int destIndex, int length);

    int GetBytes(int index, Span<byte> dest);

    int GetBytes(int index, Span<byte> dest, int length);

    int GetBytes(int index, Span<byte> dest, int destIndex, int length);

    int GetBytes(int index, Memory<byte> dest);

    int GetBytes(int index, Memory<byte> dest, int length);

    int GetBytes(int index, Memory<byte> dest, int destIndex, int length);

    int GetBytes(int index, IByteBuffer dest);

    int GetBytes(int index, IByteBuffer dest, int length);

    int GetBytes(int index, IByteBuffer dest, int destIndex, int length);

    byte ReadByte();

    sbyte ReadSByte();

    Memory<byte> ReadBytes(int length);

    int ReadBytes(byte[] dest);

    int ReadBytes(byte[] dest, int length);

    int ReadBytes(byte[] dest, int destIndex, int length);

    int ReadBytes(Span<byte> dest);

    int ReadBytes(Span<byte> dest, int length);

    int ReadBytes(Span<byte> dest, int destIndex, int length);

    int ReadBytes(Memory<byte> dest);

    int ReadBytes(Memory<byte> dest, int length);

    int ReadBytes(Memory<byte> dest, int destIndex, int length);

    int ReadBytes(IByteBuffer dest);

    int ReadBytes(IByteBuffer dest, int length);

    int ReadBytes(IByteBuffer dest, int destIndex, int length);

    char ReadChar();

    char ReadChar(Endianless endianless);

    short ReadInt16();

    short ReadInt16(Endianless endianless);

    ushort ReadUInt16();

    ushort ReadUInt16(Endianless endianless);

    int ReadInt32();

    int ReadInt32(Endianless endianless);

    uint ReadUInt32();

    uint ReadUInt32(Endianless endianless);

    long ReadInt64();

    long ReadInt64(Endianless endianless);

    ulong ReadUInt64();

    ulong ReadUInt64(Endianless endianless);

    float ReadSingle();

    float ReadSingle(Endianless endianless);

    double ReadDouble();

    double ReadDouble(Endianless endianless);

    void ResetReadIndex();

    void Release();

    public interface IReadOnlyUnsafeOp
    {
        IByteBuffer Parent { get; }

        IByteBufferAllocator Allocator { get; }

        void SetReadIndex(int readIndex);

        void Allocate(int capacityHint, Endianless endianless);

        ReadOnlyMemory<byte> AsReadOnlyMemory();

        ReadOnlySpan<byte> AsReadOnlySpan();

        Memory<byte> AsMemoryToSend();

        ArraySegment<byte> AsSegmentToSend();
    }
}
