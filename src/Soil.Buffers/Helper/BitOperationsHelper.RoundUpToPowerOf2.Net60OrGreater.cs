#if NET6_0_OR_GREATER

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soil.Buffers.Helper;

public static partial class BitOperationsHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static uint RoundUpToPowerOf2(uint value)
    {
        return BitOperations.RoundUpToPowerOf2(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ulong RoundUpToPowerOf2(ulong value)
    {
        return BitOperations.RoundUpToPowerOf2(value);
    }
}

#endif
