using System;

namespace Soil.Buffers;

public interface IByteBuffer<TDerived> : IReadOnlyByteBuffer<TDerived>
    where TDerived : IByteBuffer<TDerived>
{
    int WrittenIndex { get; }

    int WritableBytes { get; }

    int Capacity { get; }

    int MaxCapacity { get; }

    IByteBufferAllocator<TDerived> Allocator { get; }

    void EnsureCapacity(int length);

    bool Writable();

    bool Writable(int length);

    TDerived WriteByte(byte value);

    TDerived WriteSByte(sbyte value);

    int WriteBytes(byte[] src);

    int WriteBytes(byte[] src, int length);

    int WriteBytes(byte[] src, int srcIndex, int length);

    int WriteBytes(ReadOnlySpan<byte> src);

    int WriteBytes(ReadOnlySpan<byte> src, int length);

    int WriteBytes(ReadOnlySpan<byte> src, int srcIndex, int length);

    int WriteBytes(ReadOnlyMemory<byte> src);

    int WriteBytes(ReadOnlyMemory<byte> src, int length);

    int WriteBytes(ReadOnlyMemory<byte> src, int srcIndex, int length);

    int WriteBytes<TAnotherDerived>(ref TAnotherDerived src)
        where TAnotherDerived : struct, IReadOnlyByteBuffer<TAnotherDerived>;

    int WriteBytes<TAnotherDerived>(ref TAnotherDerived src, int length)
        where TAnotherDerived : struct, IReadOnlyByteBuffer<TAnotherDerived>;

    TDerived WriteChar(char value);

    TDerived WriteChar(char value, Endianless endianless);

    TDerived WriteInt16(short value);

    TDerived WriteInt16(short value, Endianless endianless);

    TDerived WriteUInt16(ushort value);

    TDerived WriteUInt16(ushort value, Endianless endianless);

    TDerived WriteInt32(int value);

    TDerived WriteInt32(int value, Endianless endianless);

    TDerived WriteUInt32(uint value);

    TDerived WriteUInt32(uint value, Endianless endianless);

    TDerived WriteInt64(long value);

    TDerived WriteInt64(long value, Endianless endianless);

    TDerived WriteUInt64(ulong value);

    TDerived WriteUInt64(ulong value, Endianless endianless);

    TDerived WriteSingle(float value);

    TDerived WriteSingle(float value, Endianless endianless);

    TDerived WriteDouble(double value);

    TDerived WriteDouble(double value, Endianless endianless);

    void ResetWrittenIndex();

    void Clear();
}
