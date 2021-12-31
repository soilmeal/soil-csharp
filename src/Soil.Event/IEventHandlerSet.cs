using System;

namespace Soil.Event;

public interface IEventHandlerSet<TEnum>
    where TEnum : struct, Enum
{
    void Subscribe(EventSubscriber<TEnum>? subscriber);

    void Unsubscribe(EventSubscriber<TEnum>? subscriber);

    void Dispatch(Event<TEnum>? eventData);
}
