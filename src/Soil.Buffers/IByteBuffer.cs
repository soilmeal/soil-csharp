using System;
using System.Collections.Generic;

namespace Soil.Buffers;

public interface IByteBuffer : IReadOnlyByteBuffer
{
    int WriteIndex { get; }

    int WritableBytes { get; }

    int Capacity { get; }

    int MaxCapacity { get; }

    IByteBufferAllocator Allocator { get; }

    IUnsafeOp Unsafe { get; }

    bool IsInitialized { get; }

    void EnsureCapacity();

    void EnsureCapacity(int length);

    bool Writable();

    bool Writable(int length);

    IByteBuffer SetByte(int index, byte value);

    IByteBuffer SetSByte(int index, sbyte value);

    int SetBytes(int index, byte[] src);

    int SetBytes(int index, byte[] src, int length);

    int SetBytes(int index, byte[] src, int srcIndex, int length);

    int SetBytes(int index, ReadOnlySpan<byte> src);

    int SetBytes(int index, ReadOnlySpan<byte> src, int length);

    int SetBytes(int index, ReadOnlySpan<byte> src, int srcIndex, int length);

    int SetBytes(int index, ReadOnlyMemory<byte> src);

    int SetBytes(int index, ReadOnlyMemory<byte> src, int length);

    int SetBytes(int index, ReadOnlyMemory<byte> src, int srcIndex, int length);

    int SetBytes(int index, IReadOnlyByteBuffer src);

    int SetBytes(int index, IReadOnlyByteBuffer src, int length);

    int SetBytes(int index, IReadOnlyByteBuffer src, int srcIndex, int length);

    IByteBuffer SetChar(int index, char value);

    IByteBuffer SetChar(int index, char value, Endianless endianless);

    IByteBuffer SetInt16(int index, short value);

    IByteBuffer SetInt16(int index, short value, Endianless endianless);

    IByteBuffer SetUInt16(int index, ushort value);

    IByteBuffer SetUInt16(int index, ushort value, Endianless endianless);

    IByteBuffer SetInt32(int index, int value);

    IByteBuffer SetInt32(int index, int value, Endianless endianless);

    IByteBuffer SetUInt32(int index, uint value);

    IByteBuffer SetUInt32(int index, uint value, Endianless endianless);

    IByteBuffer SetInt64(int index, long value);

    IByteBuffer SetInt64(int index, long value, Endianless endianless);

    IByteBuffer SetUInt64(int index, ulong value);

    IByteBuffer SetUInt64(int index, ulong value, Endianless endianless);

    IByteBuffer SetSingle(int index, float value);

    IByteBuffer SetSingle(int index, float value, Endianless endianless);

    IByteBuffer SetDouble(int index, double value);

    IByteBuffer SetDouble(int index, double value, Endianless endianless);

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

    int WriteBytes(IReadOnlyByteBuffer src);

    int WriteBytes(IReadOnlyByteBuffer src, int length);

    int WriteBytes(IReadOnlyByteBuffer src, int srcIndex, int length);

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

    void ResetWriteIndex();

    void Clear();

    public interface IUnsafeOp : IReadOnlyUnsafeOp
    {
        void Reallocate();

        void SetWriteIndex(int writeIndex);

        Memory<byte> AsMemoryToRecv();

        List<ArraySegment<byte>> AsSegmentsToRecv();
    }
}
