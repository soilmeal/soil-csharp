using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;

namespace Soil.Game;

public class ComponentLifecycleInfo
{
    private readonly Dictionary<ComponentLifecycleType, Action<object>> _actions;

    internal ComponentLifecycleInfo(Dictionary<ComponentLifecycleType, Action<object>> actions)
    {
        _actions = actions;
    }

    public bool TryGetValue(
        ComponentLifecycleType type,
        [NotNullWhen(true)] out Action<object>? action)
    {
        return _actions.TryGetValue(type, out action);
    }
}
