using System;

namespace Soil.Core.Event;

public interface IEventHandlerSet<TEnum>
    where TEnum : struct, Enum
{
    void Subscribe(IEventSubscriber<TEnum>? subscriber);

    void Unsubscribe(IEventSubscriber<TEnum>? subscriber);

    void Dispatch(Event<TEnum>? eventData);
}
