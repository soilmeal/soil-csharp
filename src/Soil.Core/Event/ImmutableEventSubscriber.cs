#if !NETFRAMEWORK

using System;
using System.Collections.Generic;
using System.Collections.Immutable;

namespace Soil.Core.Event;

internal class ImmutableEventSubscriber<TEnum> : IEventSubscriber<TEnum>
    where TEnum : struct, Enum
{
    private readonly ImmutableDictionary<TEnum, EventHandler<Event<TEnum>>> _handlers;

    public IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> Handlers
    {
        get { return _handlers; }
    }

    internal ImmutableEventSubscriber(Dictionary<TEnum, EventHandler<Event<TEnum>>> handlers_)
    {
        _handlers = ImmutableDictionary.CreateRange(handlers_);
    }
}

#endif
