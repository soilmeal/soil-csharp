using System;

namespace Soil.Buffers;

public interface IByteBuffer : IReadOnlyByteBuffer
{
    int WrittenIndex { get; }

    int WritableBytes { get; }

    int Capacity { get; }

    int MaxCapacity { get; }

    IByteBufferAllocator Allocator { get; }

    bool IsInitialized { get; }

    void EnsureCapacity(int length);

    bool Writable();

    bool Writable(int length);

    IByteBuffer WriteByte(byte value);

    IByteBuffer WriteSByte(sbyte value);

    int WriteBytes(byte[] src);

    int WriteBytes(byte[] src, int length);

    int WriteBytes(byte[] src, int srcIndex, int length);

    int WriteBytes(ReadOnlySpan<byte> src);

    int WriteBytes(ReadOnlySpan<byte> src, int length);

    int WriteBytes(ReadOnlySpan<byte> src, int srcIndex, int length);

    int WriteBytes(ReadOnlyMemory<byte> src);

    int WriteBytes(ReadOnlyMemory<byte> src, int length);

    int WriteBytes(ReadOnlyMemory<byte> src, int srcIndex, int length);

    int WriteBytes(IByteBuffer src);

    int WriteBytes(IByteBuffer src, int length);

    IByteBuffer WriteChar(char value);

    IByteBuffer WriteChar(char value, Endianless endianless);

    IByteBuffer WriteInt16(short value);

    IByteBuffer WriteInt16(short value, Endianless endianless);

    IByteBuffer WriteUInt16(ushort value);

    IByteBuffer WriteUInt16(ushort value, Endianless endianless);

    IByteBuffer WriteInt32(int value);

    IByteBuffer WriteInt32(int value, Endianless endianless);

    IByteBuffer WriteUInt32(uint value);

    IByteBuffer WriteUInt32(uint value, Endianless endianless);

    IByteBuffer WriteInt64(long value);

    IByteBuffer WriteInt64(long value, Endianless endianless);

    IByteBuffer WriteUInt64(ulong value);

    IByteBuffer WriteUInt64(ulong value, Endianless endianless);

    IByteBuffer WriteSingle(float value);

    IByteBuffer WriteSingle(float value, Endianless endianless);

    IByteBuffer WriteDouble(double value);

    IByteBuffer WriteDouble(double value, Endianless endianless);

    int SetBytes(int index, byte[] src);

    int SetBytes(int index, byte[] src, int length);

    int SetBytes(int index, byte[] src, int srcIndex, int length);

    int SetBytes(int index, ReadOnlySpan<byte> src);

    int SetBytes(int index, ReadOnlySpan<byte> src, int length);

    int SetBytes(int index, ReadOnlySpan<byte> src, int srcIndex, int length);

    int SetBytes(int index, ReadOnlyMemory<byte> src);

    int SetBytes(int index, ReadOnlyMemory<byte> src, int length);

    int SetBytes(int index, ReadOnlyMemory<byte> src, int srcIndex, int length);

    void ResetWrittenIndex();

    void Clear();
}
