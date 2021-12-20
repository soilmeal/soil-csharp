using System;

namespace Soil.Core.Event;

public interface IEventDispatcher<TEnum> : IEventHandlerSet<TEnum>
    where TEnum : struct, Enum
{
}
