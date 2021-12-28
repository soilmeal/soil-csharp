using System;

namespace Soil.Core.Buffers;

public interface IReadOnlyByteBuffer<TDerived>
    where TDerived : IReadOnlyByteBuffer<TDerived>
{
    int ReadIndex { get; }

    int ReadableBytes { get; }

    Endianless Endianless { get; }

    bool Readable();

    bool Readable(int length);

    byte ReadByte();

    sbyte ReadSByte();

    Memory<byte> ReadBytes(int length);

    int ReadBytes(ref byte[] dest);

    int ReadBytes(ref byte[] dest, int length);

    int ReadBytes(ref byte[] dest, int destIndex, int length);

    int ReadBytes(ref Span<byte> dest);

    int ReadBytes(ref Span<byte> dest, int length);

    int ReadBytes(ref Span<byte> dest, int destIndex, int length);

    int ReadBytes(ref Memory<byte> dest);

    int ReadBytes(ref Memory<byte> dest, int length);

    int ReadBytes(ref Memory<byte> dest, int destIndex, int length);

    int ReadBytes<TAnotherDerived>(ref TAnotherDerived dest)
        where TAnotherDerived : struct, IByteBuffer<TAnotherDerived>;

    int ReadBytes<TAnotherDerived>(ref TAnotherDerived dest, int length)
        where TAnotherDerived : struct, IByteBuffer<TAnotherDerived>;

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
}
