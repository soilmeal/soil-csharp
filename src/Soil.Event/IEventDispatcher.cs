using System;

namespace Soil.Event;

public interface IEventDispatcher<TEnum> : IEventHandlerSet<TEnum>
    where TEnum : struct, Enum
{
}
