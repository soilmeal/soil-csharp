using System;
using System.Buffers.Binary;
using System.Runtime.CompilerServices;
using System.Threading;
using Soil.Threading.Atomic;

namespace Soil.Utils.Id;

public abstract class HashCodeBasedIdGenerator<TTarget, TId> : ITargetIdGenerator<TTarget, TId>
    where TTarget : class
    where TId : HashCodeBasedId
{
    private const int BytesLen = sizeof(int) * 2;
    private readonly AtomicInt32 _nextSeq = new();

    public TId Generate(TTarget target)
    {
        byte[] bytes = new byte[BytesLen];
        BinaryPrimitives.WriteInt32BigEndian(bytes, RuntimeHelpers.GetHashCode(target));
        BinaryPrimitives.WriteInt32BigEndian(bytes, _nextSeq.Increment());

        return CreateId(bytes);
    }

    protected abstract TId CreateId(byte[] bytes);
}
