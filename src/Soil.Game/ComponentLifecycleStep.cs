using System;
using System.Runtime.CompilerServices;

namespace Soil.Game;

public enum ComponentLifecycleStep
{
    Start,

    OnEnable,

    FixedUpdate,

    Update,

    LateUpdate,

    OnDisable,

    OnDestroy,
}

public static class ComponentLifecycleStepExtensions
{
    private static readonly ComponentLifecycleStep[] _values = {
        ComponentLifecycleStep.Start,
        ComponentLifecycleStep.OnEnable,
        ComponentLifecycleStep.FixedUpdate,
        ComponentLifecycleStep.Update,
        ComponentLifecycleStep.LateUpdate,
        ComponentLifecycleStep.OnDisable,
        ComponentLifecycleStep.OnDestroy,
    };

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static ComponentLifecycleStep[] FastGetValues()
    {
        return _values;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static string FastToString(this ComponentLifecycleStep value)
    {
        return value switch
        {
            ComponentLifecycleStep.Start => nameof(ComponentLifecycleStep.Start),
            ComponentLifecycleStep.OnEnable => nameof(ComponentLifecycleStep.OnEnable),
            ComponentLifecycleStep.FixedUpdate => nameof(ComponentLifecycleStep.FixedUpdate),
            ComponentLifecycleStep.Update => nameof(ComponentLifecycleStep.Update),
            ComponentLifecycleStep.LateUpdate => nameof(ComponentLifecycleStep.LateUpdate),
            ComponentLifecycleStep.OnDisable => nameof(ComponentLifecycleStep.OnDisable),
            ComponentLifecycleStep.OnDestroy => nameof(ComponentLifecycleStep.OnDestroy),
            _ => throw new ArgumentOutOfRangeException(nameof(value), value, null),
        };
    }
}
