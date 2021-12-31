#if NET6_0_OR_GREATER

using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;

namespace Soil.Buffers.Helper;

public static partial class BinaryPrimitivesHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadSingleBigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadSingleBigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static float ReadSingleLittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadSingleLittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleBigEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadDoubleBigEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static double ReadDoubleLittleEndian(ReadOnlySpan<byte> source)
    {
        return BinaryPrimitives.ReadDoubleLittleEndian(source);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleBigEndian(Span<byte> dest, float value)
    {
        BinaryPrimitives.WriteSingleBigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteSingleLittleEndian(Span<byte> dest, float value)
    {
        BinaryPrimitives.WriteSingleLittleEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleBigEndian(Span<byte> dest, double value)
    {
        BinaryPrimitives.WriteDoubleBigEndian(dest, value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static void WriteDoubleLittleEndian(Span<byte> dest, double value)
    {
        BinaryPrimitives.WriteDoubleLittleEndian(dest, value);
    }
}

#endif
