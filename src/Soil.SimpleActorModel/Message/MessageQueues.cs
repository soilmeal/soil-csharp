using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

namespace Soil.SimpleActorModel.Message;

public static class MessageQueues
{
    public static readonly IMessageQueue None = new NoneMessageQueue();

    private class NoneMessageQueue : IMessageQueue
    {
        public int Count
        {
            get
            {
                return 0;
            }
        }

        public bool IsSynchronized
        {
            get
            {
                return false;
            }
        }

#pragma warning disable CS8766
        [MaybeNull]
        public object SyncRoot
        {
            get
            {
                return null;
            }
        }
#pragma warning restore CS8766

        public void Clear()
        {
        }

        public void CopyTo(Envelope[] array, int index)
        {
        }

        public void CopyTo(Array array, int index)
        {
        }

        public IEnumerator<Envelope> GetEnumerator()
        {
            return Enumerable.Empty<Envelope>()
                .GetEnumerator();
        }

        public Envelope[] ToArray()
        {
            return Array.Empty<Envelope>();
        }

        public bool TryAdd(Envelope item)
        {
            return false;
        }

        public bool TryTake(out Envelope item)
        {
            item = default;
            return false;
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return Enumerable.Empty<Envelope>()
                .GetEnumerator();
        }
    }
}
