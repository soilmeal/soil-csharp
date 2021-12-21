using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Soil.Core.Event;

internal class ConcurrentEventHandlerSet<TEnum> : IEventDispatcher<TEnum>
    where TEnum : struct, Enum
{
    private readonly ConcurrentDictionary<TEnum, ImmutableList<EventHandler<Event<TEnum>>>> _handlersOfType = new();

    internal ConcurrentEventHandlerSet() { }

    public void Subscribe(IEventSubscriber<TEnum>? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> targetHandlers = subscriber.Handlers;

        foreach (var (type, handler) in targetHandlers)
        {
            _handlersOfType.AddOrUpdate(type,
                (_) => ImmutableList.Create(handler),
                (_, handlers) => handlers.Add(handler));
        }
    }

    public void Unsubscribe(IEventSubscriber<TEnum>? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> targetHandlers = subscriber.Handlers;

        foreach (var (type, handler) in targetHandlers)
        {
            ImmutableList<EventHandler<Event<TEnum>>>? handlers;
            if (!_handlersOfType.TryGetValue(type, out handlers))
            {
                continue;
            }

            handlers.Remove(handler);
        }
    }

    public void Dispatch(Event<TEnum>? eventData)
    {
        if (eventData == null)
        {
            return;
        }

        ImmutableList<EventHandler<Event<TEnum>>>? handlers;
        if (!_handlersOfType.TryGetValue(eventData.Type, out handlers))
        {
            return;
        }

        foreach (var handler in handlers)
        {
            handler(eventData);
        }
    }
}
