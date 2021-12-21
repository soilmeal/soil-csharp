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

        public Builder<TEnum> AddHandler<TEvent>(TEnum type, EventHandler<TEvent>? handler)
            where TEvent : Event<TEnum>
        {
            if (handler == null)
            {
                return this;
            }

            _handlers.TryAdd(type, (evt) =>
            {
                if (evt is not TEvent tEvt)
                {
                    return;
                }


                handler(tEvt);
            });

            return this;
        }

        public Builder<TEnum> AddHandler<TEvent>(TEnum type, Action<TEvent>? handler)
        {
            if (handler == null)
            {
                return this;
            }

            _handlers.TryAdd(type, (evt) =>
            {
                if (evt is not TEvent tEvt)
                {
                    return;
                }

                handler.Invoke(tEvt);
            });

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
    }
}
