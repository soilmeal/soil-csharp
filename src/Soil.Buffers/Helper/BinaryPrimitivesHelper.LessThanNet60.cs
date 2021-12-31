
#if !NET6_0_OR_GREATER

using System;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

namespace Soil.Buffers.Helper;

public static partial class BinaryPrimitivesHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadSingleBigEndian(ReadOnlySpan<byte> source)
    {
        return BitConverter.IsLittleEndian
            ? BitConverter.Int32BitsToSingle(ReverseEndianness(MemoryMarshal.Read<int>(source)))
            : MemoryMarshal.Read<float>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadSingleLittleEndian(ReadOnlySpan<byte> source)
    {
        return !BitConverter.IsLittleEndian
            ? BitConverter.Int32BitsToSingle(ReverseEndianness(MemoryMarshal.Read<int>(source)))
            : MemoryMarshal.Read<float>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleBigEndian(ReadOnlySpan<byte> source)
    {
        return BitConverter.IsLittleEndian
            ? BitConverter.Int64BitsToDouble(ReverseEndianness(MemoryMarshal.Read<long>(source)))
            : MemoryMarshal.Read<double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleLittleEndian(ReadOnlySpan<byte> source)
    {
        return !BitConverter.IsLittleEndian
            ? BitConverter.Int64BitsToDouble(ReverseEndianness(MemoryMarshal.Read<long>(source)))
            : MemoryMarshal.Read<double>(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleBigEndian(Span<byte> dest, float value)
    {
        if (BitConverter.IsLittleEndian)
        {
            int tmp = ReverseEndianness(BitConverter.SingleToInt32Bits(value));
            MemoryMarshal.Write(dest, ref tmp);
        }
        else
        {
            MemoryMarshal.Write(dest, ref value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleLittleEndian(Span<byte> dest, float value)
    {
        if (!BitConverter.IsLittleEndian)
        {
            int tmp = ReverseEndianness(BitConverter.SingleToInt32Bits(value));
            MemoryMarshal.Write(dest, ref tmp);
        }
        else
        {
            MemoryMarshal.Write(dest, ref value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleBigEndian(Span<byte> dest, double value)
    {
        if (BitConverter.IsLittleEndian)
        {
            long tmp = ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
            MemoryMarshal.Write(dest, ref tmp);
        }
        else
        {
            MemoryMarshal.Write(dest, ref value);
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleLittleEndian(Span<byte> dest, double value)
    {
        if (!BitConverter.IsLittleEndian)
        {
            long tmp = ReverseEndianness(BitConverter.DoubleToInt64Bits(value));
            MemoryMarshal.Write(dest, ref tmp);
        }
        else
        {
            MemoryMarshal.Write(dest, ref value);
        }
    }
}

#endif
