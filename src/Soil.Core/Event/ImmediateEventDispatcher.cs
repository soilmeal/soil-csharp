using System;

namespace Soil.Core.Event;

public class ImmediateEventDispatcher<TEnum> : IEventDispatcher<TEnum>
    where TEnum : struct, Enum
{
    private readonly IEventHandlerSet<TEnum> _handlerSet;

    private ImmediateEventDispatcher(IEventHandlerSet<TEnum> handlerSets_)
    {
        _handlerSet = handlerSets_;
    }

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
