using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Soil.SimpleActorModel.Message;

public class UnboundedMessageQueue : IMessageQueue
{
    private readonly ConcurrentQueue<Envelope> _envelopes = new();

    public int Count
    {
        get
        {
            return _envelopes.Count;
        }
    }

    public bool IsSynchronized
    {
        get
        {
            return false;
        }
    }

    public object SyncRoot
    {
        get
        {
            throw new NotSupportedException();
        }
    }

    public bool TryAdd(Envelope item)
    {
        _envelopes.Enqueue(item);
        return true;
    }

    public bool TryTake(out Envelope item)
    {
        return _envelopes.TryDequeue(out item);
    }

    public void Clear()
    {
        _envelopes.Clear();
    }

    public void CopyTo(Envelope[] array, int index)
    {
        _envelopes.CopyTo(array, index);
    }

    public void CopyTo(Array array, int index)
    {
        ((ICollection)_envelopes).CopyTo(array, index);
    }

    public IEnumerator<Envelope> GetEnumerator()
    {
        return _envelopes.GetEnumerator();
    }

    public Envelope[] ToArray()
    {
        return _envelopes.ToArray();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return _envelopes.GetEnumerator();
    }
}
