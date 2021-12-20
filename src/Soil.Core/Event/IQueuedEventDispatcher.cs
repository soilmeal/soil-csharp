using System;

namespace Soil.Core.Event;

public interface IEventQueueDispatcher<TEnum> : IEventDispatcher<TEnum>
    where TEnum : struct, Enum
{
    void Process()
    {
        Process(1);
    }

    void Process(ulong count);

    void ProcessAll();
}

public interface IEventQueueDispatcher
{
    static IEventQueueDispatcher<TEnum> Create<TEnum>()
        where TEnum : struct, Enum
    {
        return new EventQueueDispatcher<TEnum>();
    }

    static IEventQueueDispatcher<TEnum> CreateConcurrent<TEnum>()
        where TEnum : struct, Enum
    {
        return new ConcurrentEventQueueDispatcher<TEnum>();
    }
}
