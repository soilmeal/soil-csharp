using System;

namespace Soil.Game;

public enum ComponentLifecycleType
{
    Awake,

    OnEnable,

    Start,

    FixedUpdate,

    Update,

    LateUpdate,

    OnDisable,

    OnDestroy,
}

public static class ComponentLifecycleTypeExtensions
{
    public static string FastToString(this ComponentLifecycleType type)
    {
        return type switch
        {
            ComponentLifecycleType.Awake => nameof(ComponentLifecycleType.Awake),
            ComponentLifecycleType.OnEnable => nameof(ComponentLifecycleType.OnEnable),
            ComponentLifecycleType.Start => nameof(ComponentLifecycleType.Start),
            ComponentLifecycleType.FixedUpdate => nameof(ComponentLifecycleType.FixedUpdate),
            ComponentLifecycleType.Update => nameof(ComponentLifecycleType.Update),
            ComponentLifecycleType.LateUpdate => nameof(ComponentLifecycleType.LateUpdate),
            ComponentLifecycleType.OnDisable => nameof(ComponentLifecycleType.OnDisable),
            ComponentLifecycleType.OnDestroy => nameof(ComponentLifecycleType.OnDestroy),
            _ => throw new ArgumentOutOfRangeException(nameof(type), type, null),
        };
    }
}
