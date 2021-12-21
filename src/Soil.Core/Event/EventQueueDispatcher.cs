using System;
using System.Collections.Generic;

namespace Soil.Core.Event;

internal class EventQueueDispatcher<TEnum> : IEventQueueDispatcher<TEnum>
    where TEnum : struct, Enum
{
    private ulong _queuedCnt = 0;

    private ulong _processedCnt = 0;

    private readonly Queue<Event<TEnum>> _queue = new();

    private readonly EventHandlerSet<TEnum> _handlerSet = new();

    internal EventQueueDispatcher() { }

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

        _queuedCnt += 1;
    }

    public void Process(ulong count)
    {
        if (count <= 0UL)
        {
            return;
        }

        ulong currProcessedCnt = 0UL;
        while (currProcessedCnt < count && _processedCnt + currProcessedCnt < _queuedCnt)
        {
            bool proceed = TryProcess();
            if (!proceed)
            {
                break;
            }

            currProcessedCnt += 1UL;
        }

        _processedCnt += currProcessedCnt;
    }

    public void ProcessAll()
    {
        while (_processedCnt < _queuedCnt)
        {
            bool proceed = TryProcess();
            if (!proceed)
            {
                break;
            }

            _processedCnt += 1UL;
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
