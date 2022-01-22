using System;
using System.Buffers;

namespace Soil.Buffers;

public interface IReadOnlyByteBuffer
{
    int ReadIndex { get; }

    int ReadableBytes { get; }

    Endianless Endianless { get; }

    IReadOnlyUnsafeOp ReadOnlyUnsafe { get; }

    bool Readable();

    bool Readable(int length);

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

    void ResetReadIndex();

    void Release();

    public interface IReadOnlyUnsafeOp
    {
        IByteBuffer Parent { get; }

        IByteBufferAllocator Allocator { get; }

        void SetReadIndex(int readIndex);

        void Allocate(int capacityHint, Endianless endianless);

        Span<byte> AsSpanToSend();

        Memory<byte> AsMemoryToSend();

        ArraySegment<byte> AsSegmentToSend();

        ReadOnlySpan<byte> AsReadOnlySpan();

        ReadOnlyMemory<byte> AsReadOnlyMemory();
    }
}
