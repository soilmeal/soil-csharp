using System;
using System.Runtime.CompilerServices;
using Soil.Buffers.Helper;

namespace Soil.Buffers;

internal static class BufferUtilities
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComputeActualCapacity(int capacityHint)
    {
        return (int)BitOperationsHelper.RoundUpToPowerOf2((uint)capacityHint);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int ComputeNextCapacity(int capacity)
    {
        return BitOperationsHelper.Log2((uint)capacity);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(T[] src)
    {
        return src.AsSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(T[] src, int length)
    {
        return SpanSlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(T[] src, int index, int length)
    {
        return SpanSlice(src.AsSpan(), index, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(Memory<T> src)
    {
        return src.Span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(Memory<T> src, int length)
    {
        return SpanSlice(src, 0, length);
    }

    public static Span<T> SpanSlice<T>(Memory<T> src, int index, int length)
    {
        return src.Span[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(Span<T> src, int length)
    {
        return SpanSlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Span<T> SpanSlice<T>(Span<T> src, int index, int length)
    {
        return src[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(T[] src)
    {
        return src.AsSpan();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(T[] src, int length)
    {
        return ReadOnlySpanSlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(T[] src, int index, int length)
    {
        return ReadOnlySpanSlice((ReadOnlySpan<T>)src.AsSpan(), index, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(ReadOnlyMemory<T> src)
    {
        return src.Span;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(ReadOnlyMemory<T> src, int length)
    {
        return ReadOnlySpanSlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(ReadOnlyMemory<T> src, int index, int length)
    {
        return src.Span[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(ReadOnlySpan<T> src, int length)
    {
        return ReadOnlySpanSlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlySpan<T> ReadOnlySpanSlice<T>(ReadOnlySpan<T> src, int index, int length)
    {
        return src[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> MemorySlice<T>(T[] src)
    {
        return src.AsMemory();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> MemorySlice<T>(T[] src, int length)
    {
        return MemorySlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> MemorySlice<T>(T[] src, int index, int length)
    {
        return src.AsMemory()[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> MemorySlice<T>(Memory<T> src, int length)
    {
        return MemorySlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Memory<T> MemorySlice<T>(Memory<T> src, int index, int length)
    {
        return src[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> ReadOnlyMemorySlice<T>(T[] src)
    {
        return src.AsMemory();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> ReadOnlyMemorySlice<T>(T[] src, int length)
    {
        return ReadOnlyMemorySlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> ReadOnlyMemorySlice<T>(T[] src, int index, int length)
    {
        return src.AsMemory()[index..(index + length)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> ReadOnlyMemorySlice<T>(ReadOnlyMemory<T> src, int length)
    {
        return ReadOnlyMemorySlice(src, 0, length);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ReadOnlyMemory<T> ReadOnlyMemorySlice<T>(ReadOnlyMemory<T> src, int index, int length)
    {
        return src[index..(index + length)];
    }
}
