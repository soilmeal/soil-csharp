#if !NETCOREAPP3_1_OR_GREATER

using System.Runtime.CompilerServices;

namespace Soil.Buffers.Helper;

public static partial class BitOperationsHelper
{
    private static readonly byte[] _log2DeBruijn = new byte[32] {
        00, 09, 01, 10, 13, 21, 02, 29,
        11, 14, 16, 18, 22, 25, 03, 30,
        08, 12, 20, 28, 15, 17, 24, 07,
        19, 27, 23, 06, 26, 05, 04, 31
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(uint value)
    {
        value |= value >> 01;
        value |= value >> 02;
        value |= value >> 04;
        value |= value >> 08;
        value |= value >> 16;

        return _log2DeBruijn[(int)((value * 0x07C4ACDDu) >> 27)];
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static int Log2(long value)
    {
        value |= 1;
        uint hi = (uint)(value >> 32);
        return hi == 0
            ? Log2((uint)value)
            : 32 + Log2(hi);
    }
}

#endif

