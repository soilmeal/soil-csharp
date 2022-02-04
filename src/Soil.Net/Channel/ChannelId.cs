using System;
using System.Runtime.CompilerServices;
using System.Threading;
using Soil.Buffers.Helper;
using Soil.Threading.Atomic;

namespace Soil.Net.Channel;

public class ChannelId
{
    private const int BytesLen = sizeof(long) + (sizeof(int) * 3);

    private static readonly AtomicInt32 _nextSeq = new();

    private static readonly ThreadLocal<Random> _random = new(() => new Random(Environment.TickCount));

    private readonly byte[] _bytes;

    private readonly int _hashCode;

    private readonly string _text;

    public ChannelId(IChannel channel)
    {
        long currentTicks = DateTime.UtcNow.Ticks;
        int seq = _nextSeq.Increment();
        int random = _random.Value!.Next();
        int hashCode = RuntimeHelpers.GetHashCode(channel);

        _bytes = new byte[BytesLen];
        BinaryPrimitivesHelper.WriteInt64BigEndian(_bytes, currentTicks);
        BinaryPrimitivesHelper.WriteInt32BigEndian(_bytes, seq);
        BinaryPrimitivesHelper.WriteInt32BigEndian(_bytes, random);
        BinaryPrimitivesHelper.WriteInt32BigEndian(_bytes, hashCode);

        _hashCode = HashCode.Combine(currentTicks, seq, random, hashCode);
        _text = BitConverter.ToString(_bytes)
            .Replace("-", "");
    }

    public override int GetHashCode()
    {
        return _hashCode;
    }

    public override string ToString()
    {
        return _text;
    }
}
