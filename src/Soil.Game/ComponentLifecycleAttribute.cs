using System;

namespace Soil.Game;

[AttributeUsage(AttributeTargets.Method)]
public class ComponentLifecycleAttribute : Attribute
{
    public ComponentLifecycleStep Step { get; }

    public ComponentLifecycleAttribute(ComponentLifecycleStep step)
    {
        Step = step;
    }
}
