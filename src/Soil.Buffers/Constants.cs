using Soil.Buffers.Helper;

namespace Soil.Buffers;

internal class Constants
{
    public static readonly int MaxCapacity = (int)BitOperationsHelper.RoundUpToPowerOf2((uint)int.MaxValue >> 1);

    public static readonly int DefaultCapacityIncrements = 1024;
}
