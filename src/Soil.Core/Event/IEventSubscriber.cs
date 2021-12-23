using System;
using System.Collections.Generic;

namespace Soil.Core.Event;

public interface IEventSubscriber<TEnum>
    where TEnum : struct, Enum
{
    IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> Handlers { get; }
}

public interface IEventSubscriber
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

        public Builder<TEnum> AddHandler<TEvent>(TEnum type, Action<TEvent> handler)
        {
            if (handler == null)
            {
                throw new ArgumentNullException(nameof(handler));
            }

            _handlers.TryAdd(type, WrapHandler(handler));

            return this;
        }

        public IEventSubscriber<TEnum> Build()
        {
#if !NETFRAMEWORK
            return new ImmutableEventSubscriber<TEnum>(_handlers);
#else
            return new ReadOnlyEventSubscriber<TEnum>(_handlers);
#endif
        }

        private static EventHandler<Event<TEnum>> WrapHandler<TEvent>(Action<TEvent> handler)
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
