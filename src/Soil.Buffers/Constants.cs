using System;
using System.Collections.Generic;
using Soil.Buffers.Helper;

namespace Soil.Buffers;

internal class Constants
{
    public static readonly byte[] EmptyBuffer = Array.Empty<byte>();

    public static readonly List<ArraySegment<byte>> EmptySegments = new();

    public static readonly int DefaultMaxCapacity = (int)BitOperationsHelper.RoundUpToPowerOf2((uint)int.MaxValue >> 1);

    public const int DefaultCapacity = 1024;

    public const int DefaultCapacityIncrements = 1024;

    public const int CompositionByteBufferCapacityHint = 16;
}
