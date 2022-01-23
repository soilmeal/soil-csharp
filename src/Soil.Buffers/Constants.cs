using System;
using Soil.Buffers.Helper;

namespace Soil.Buffers;

internal class Constants
{
    public static readonly byte[] DefaultBuffer = Array.Empty<byte>();

    public static readonly int MaxCapacity = (int)BitOperationsHelper.RoundUpToPowerOf2((uint)int.MaxValue >> 1);

    public const int DefaultCapacityIncrements = 1024;

    public const int CompositionByteBufferCapacityHint = 16;
}
