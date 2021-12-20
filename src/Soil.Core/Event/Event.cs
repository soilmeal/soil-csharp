using System;

namespace Soil.Core.Event;

public abstract class Event<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _type;

    public TEnum Type => _type;

    public Event(TEnum type_)
    {
        _type = type_;
    }
}
