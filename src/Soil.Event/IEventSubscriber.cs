using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Soil.Event;

public class EventSubscriber<TEnum>
    where TEnum : struct, Enum
{
    private readonly IReadOnlyDictionary<TEnum, EventHandler<Event<TEnum>>> _handlers;

    public IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> Handlers
    {
        get { return _handlers; }
    }

    internal EventSubscriber(Dictionary<TEnum, EventHandler<Event<TEnum>>> handlers)
    {
        _handlers = handlers;
    }
}

public class EventSubscriber
{
    public class Builder<TEnum>
        where TEnum : struct, Enum
    {
        private readonly Dictionary<TEnum, EventHandler<Event<TEnum>>> _handlers = new();

        public Builder<TEnum> AddHandler<TEvent>(TEnum type, EventHandler<TEvent> handler)
            where TEvent : Event<TEnum>
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.TryAdd(type, WrapHandler<TEvent>(handler.Invoke));

            return this;
        }

        public EventSubscriber<TEnum> Build()
        {
            return new EventSubscriber<TEnum>(_handlers);
        }

        private static EventHandler<Event<TEnum>> WrapHandler<TEvent>(EventHandler<TEvent> handler)
            where TEvent : Event<TEnum>
        {
            return (evt) =>
            {
                if (evt is not TEvent tEvt)
                {
                    return;
                }

                handler(tEvt);
            };
        }
    }
}
