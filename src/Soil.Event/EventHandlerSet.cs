using System;
using System.Collections.Generic;

namespace Soil.Event;

internal class EventHandlerSet<TEnum> : IEventHandlerSet<TEnum>
    where TEnum : struct, Enum
{
    private readonly Dictionary<TEnum, List<EventHandler<Event<TEnum>>>> _handlersOfType = new();

    internal EventHandlerSet() { }

    public void Subscribe(EventSubscriber<TEnum>? subscriber)
    {
        if (subscriber == null)
        {
            return;
        }

        IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> targetHandlers = subscriber.Handlers;

        foreach (var (type, handler) in targetHandlers)
        {
            List<EventHandler<Event<TEnum>>>? handlers;
            if (!_handlersOfType.TryGetValue(type, out handlers))
            {
                handlers = new List<EventHandler<Event<TEnum>>>(16);
                _handlersOfType.Add(type, handlers);
            }

            handlers.Add(handler);
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
            List<EventHandler<Event<TEnum>>>? handlers;
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

        List<EventHandler<Event<TEnum>>>? handlers;
        if (!_handlersOfType.TryGetValue(eventData.Type, out handlers))
        {
            return;
        }

        foreach (var handler in handlers)
        {
            handler.Invoke(eventData);
        }
    }
}
