using System;
using System.Collections;
using System.Collections.Generic;
using Soil.SimpleActorModel.Message;

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
                throw new NotSupportedException();
            }
        }

        public bool IsSynchronized
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public object SyncRoot
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public void Clear()
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Envelope[] array, int index)
        {
            throw new NotSupportedException();
        }

        public void CopyTo(Array array, int index)
        {
            throw new NotSupportedException();
        }

        public IEnumerator<Envelope> GetEnumerator()
        {
            throw new NotSupportedException();
        }

        public Envelope[] ToArray()
        {
            throw new NotSupportedException();
        }

        public bool TryAdd(Envelope item)
        {
            throw new NotSupportedException();
        }

        public bool TryTake(out Envelope item)
        {
            throw new NotSupportedException();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            throw new NotSupportedException();
        }
    }
}
