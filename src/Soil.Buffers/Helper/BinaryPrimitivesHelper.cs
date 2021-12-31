using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Soil.Buffers.Helper;

public static partial class BinaryPrimitivesHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadInt16BigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadInt16BigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static short ReadInt16LittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadInt16LittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16BigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadUInt16BigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ushort ReadUInt16LittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadUInt16LittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt32BigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadInt32BigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReadInt32LittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadInt32LittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32BigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadUInt32BigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint ReadUInt32LittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadUInt32LittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadInt64BigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadInt64BigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReadInt64LittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadInt64LittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadUInt64BigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadUInt64BigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong ReadUInt64LittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadUInt64LittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16BigEndian(Span<byte> dest, short value)
    {
        BinaryPrimitives.WriteInt16BigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt16LittleEndian(Span<byte> dest, short value)
    {
        BinaryPrimitives.WriteInt16LittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16BigEndian(Span<byte> dest, ushort value)
    {
        BinaryPrimitives.WriteUInt16BigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt16LittleEndian(Span<byte> dest, ushort value)
    {
        BinaryPrimitives.WriteUInt16LittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32BigEndian(Span<byte> dest, int value)
    {
        BinaryPrimitives.WriteInt32BigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt32LittleEndian(Span<byte> dest, int value)
    {
        BinaryPrimitives.WriteInt32LittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32BigEndian(Span<byte> dest, uint value)
    {
        BinaryPrimitives.WriteUInt32BigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt32LittleEndian(Span<byte> dest, uint value)
    {
        BinaryPrimitives.WriteUInt32LittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64BigEndian(Span<byte> dest, long value)
    {
        BinaryPrimitives.WriteInt64BigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteInt64LittleEndian(Span<byte> dest, long value)
    {
        BinaryPrimitives.WriteInt64LittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64BigEndian(Span<byte> dest, ulong value)
    {
        BinaryPrimitives.WriteUInt64BigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteUInt64LittleEndian(Span<byte> dest, ulong value)
    {
        BinaryPrimitives.WriteUInt64LittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ReverseEndianness(int value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static long ReverseEndianness(long value)
    {
        return BinaryPrimitives.ReverseEndianness(value);
    }
}
