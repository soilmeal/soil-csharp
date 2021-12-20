using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Soil.Core.Event;

internal class ReadOnlyEventSubscriber<TEnum> : IEventSubscriber<TEnum>
    where TEnum : struct, Enum
{
    private readonly ReadOnlyDictionary<TEnum, EventHandler<Event<TEnum>>> _handlers;

    public IEnumerable<KeyValuePair<TEnum, EventHandler<Event<TEnum>>>> Handlers
    {
        get { return _handlers; }
    }

    internal ReadOnlyEventSubscriber(Dictionary<TEnum, EventHandler<Event<TEnum>>> handlers_)
    {
        _handlers = new ReadOnlyDictionary<TEnum, EventHandler<Event<TEnum>>>(handlers_);
    }
}
