using System;

namespace Soil.Game;

[AttributeUsage(AttributeTargets.Method)]
public class ComponentLifecycleAttribute : Attribute
{
    public ComponentLifecycleStep Step { get; }

    public ComponentLifecycleAttribute(ComponentLifecycleStep Step)
    {
        this.Step = Step;
    }
}
