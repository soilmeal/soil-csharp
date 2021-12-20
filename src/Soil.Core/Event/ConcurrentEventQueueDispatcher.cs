using System;
using System.Collections.Concurrent;
using Soil.Core.Threading.Atomic;

namespace Soil.Core.Event;

internal class ConcurrentEventQueueDispatcher<TEnum> : IEventQueueDispatcher<TEnum>
    where TEnum : struct, Enum
{
    private readonly AtomicUInt64 _queuedCnt = new AtomicUInt64(0UL);

    private readonly AtomicUInt64 _processedCnt = new AtomicUInt64(0UL);

    private readonly ConcurrentQueue<Event<TEnum>> _queue = new ConcurrentQueue<Event<TEnum>>();

    private readonly ConcurrentEventHandlerSet<TEnum> _handlerSet = new ConcurrentEventHandlerSet<TEnum>();

    internal ConcurrentEventQueueDispatcher() { }

    public void Subscribe(IEventSubscriber<TEnum>? subscriber)
    {
        _handlerSet.Subscribe(subscriber);
    }

    public void Unsubscribe(IEventSubscriber<TEnum>? subscriber)
    {
        _handlerSet.Unsubscribe(subscriber);
    }

    public void Dispatch(Event<TEnum>? eventData)
    {
        if (eventData == null)
        {
            return;
        }

        _queue.Enqueue(eventData);

        _queuedCnt.Increment();
    }

    public void Process(ulong count)
    {
        if (count <= 0UL)
        {
            return;
        }

        ulong currProcessedCnt = 0UL;
        while (currProcessedCnt < count && _processedCnt.Read() + currProcessedCnt < _queuedCnt.Read())
        {
            bool processed = TryProcess();
            if (!processed)
            {
                break;
            }

            currProcessedCnt += 1UL;
        }

        _processedCnt.Add(currProcessedCnt);
    }

    public void ProcessAll()
    {
        while (_processedCnt.Read() < _queuedCnt.Read())
        {
            bool proceed = TryProcess();
            if (!proceed)
            {
                break;
            }

            _processedCnt.Increment();
        }
    }

    private bool TryProcess()
    {
        Event<TEnum>? eventData;
        if (!_queue.TryDequeue(out eventData))
        {
            return false;
        }

        _handlerSet.Dispatch(eventData);
        return true;
    }
}
