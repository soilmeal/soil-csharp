using System.Buffers;

namespace Soil.Buffers;

public interface IByteBufferWriter : IBufferWriter<byte>
{
    public int MaxCapacity { get; }
}
