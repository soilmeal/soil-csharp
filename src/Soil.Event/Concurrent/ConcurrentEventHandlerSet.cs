using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using Soil.Collections.Generics;

namespace Soil.Event.Concurrent;

internal class ConcurrentEventHandlerSet<TEnum> : IEventHandlerSet<TEnum>
    where TEnum : struct, Enum
{

    private readonly ConcurrentDictionary<TEnum, CopyOnWriteList<EventHandler<Event<TEnum>>>> _handlersOfType = new();

    internal ConcurrentEventHandlerSet() { }

    public void Subscribe(EventSubscriber<TEnum>? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> targetHandlers = subscriber.Handlers;

        foreach (var (type, handler) in targetHandlers)
        {
            _handlersOfType.AddOrUpdate(type,
                (a) => new CopyOnWriteList<EventHandler<Event<TEnum>>>(),
                (_, handlers) =>
                {
                    handlers.Add(handler);
                    return handlers;
                });
        }
    }

    public void Unsubscribe(EventSubscriber<TEnum>? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> targetHandlers = subscriber.Handlers;

        foreach (var (type, handler) in targetHandlers)
        {
            CopyOnWriteList<EventHandler<Event<TEnum>>>? handlers;
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

        CopyOnWriteList<EventHandler<Event<TEnum>>>? handlers;
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
