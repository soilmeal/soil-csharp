using System;

namespace Soil.Game;

[AttributeUsage(AttributeTargets.Method)]
public class ComponentLifecycleAttribute : Attribute
{
    public ComponentLifecycleType Value { get; }

    public ComponentLifecycleAttribute(ComponentLifecycleType value)
    {
        Value = value;
    }
}
