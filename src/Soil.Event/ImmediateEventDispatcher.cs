using System;
using Soil.Event.Concurrent;

namespace Soil.Event;

public class ImmediateEventDispatcher<TEnum> : IEventDispatcher<TEnum>
    where TEnum : struct, Enum
{
    private readonly IEventHandlerSet<TEnum> _handlerSet;

    private ImmediateEventDispatcher(IEventHandlerSet<TEnum> handlerSets)
    {
        _handlerSet = handlerSets;
    }

    public void Subscribe(EventSubscriber<TEnum>? subscriber)
    {
        _handlerSet.Subscribe(subscriber);
    }

    public void Unsubscribe(EventSubscriber<TEnum>? subscriber)
    {
        _handlerSet.Unsubscribe(subscriber);
    }

    public void Dispatch(Event<TEnum>? eventData)
    {
        _handlerSet.Dispatch(eventData);
    }

    public static ImmediateEventDispatcher<TEnum> Create()
    {
        return new ImmediateEventDispatcher<TEnum>(new EventHandlerSet<TEnum>());
    }

    public static ImmediateEventDispatcher<TEnum> CreateConcurrent()
    {
        return new ImmediateEventDispatcher<TEnum>(new ConcurrentEventHandlerSet<TEnum>());
    }
}
