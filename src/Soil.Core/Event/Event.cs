using System;

namespace Soil.Core.Event;

public abstract class Event<TEnum>
    where TEnum : struct, Enum
{
    private readonly TEnum _type;

    public TEnum Type
    {
        get
        {
            return _type;
        }
    }

    protected Event(TEnum type)
    {
        _type = type;
    }
}
