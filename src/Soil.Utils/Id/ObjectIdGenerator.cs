using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Threading;
using Soil.Threading.Atomic;

namespace Soil.Utils.Id;

public abstract class ObjectIdGenerator<TTarget, TId> : IBytesIdGenerator<TTarget, TId>
    where TTarget : class
    where TId : IBytesId
{
    private const int BytesLen = sizeof(long) + (sizeof(int) * 3);

    private readonly AtomicInt32 _nextSeq = new();

    private readonly ThreadLocal<Random> _random = new(() => new Random(Environment.TickCount));

    public TId Generate(TTarget target)
    {
        long currentTicks = DateTime.UtcNow.Ticks;
        int seq = _nextSeq.Increment();
        int random = _random.Value!.Next();
        int hashCode = RuntimeHelpers.GetHashCode(target);

        byte[] bytes = new byte[BytesLen];
        BinaryPrimitives.WriteInt64LittleEndian(bytes, currentTicks);
        BinaryPrimitives.WriteInt32BigEndian(bytes, seq);
        BinaryPrimitives.WriteInt32BigEndian(bytes, random);
        BinaryPrimitives.WriteInt32BigEndian(bytes, hashCode);

        return CreateId(bytes);
    }

    protected abstract TId CreateId(byte[] bytes);
}
