#if NETCOREAPP3_1_OR_GREATER

using System.Numerics;
using System.Runtime.CompilerServices;

namespace Soil.Buffers.Helper;

public static partial class BitOperationsHelper
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(uint value)
    {
        return BitOperations.Log2(value);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(ulong value)
    {
        return BitOperations.Log2(value);
    }
}

#endif
