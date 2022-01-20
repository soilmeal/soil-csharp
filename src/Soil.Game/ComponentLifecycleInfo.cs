using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Soil.Game;

public class ComponentLifecycleInfo
{

    private readonly Action<object>? _onEnable;

    private readonly Action<object>? _start;

    private readonly Action<object>? _fixedUpdate;

    private readonly Action<object>? _update;

    private readonly Action<object>? _lateUpdate;

    private readonly Action<object>? _onDisable;

    private readonly Action<object>? _onDestroy;

    internal ComponentLifecycleInfo(Dictionary<ComponentLifecycleStep, Action<object>> actions)
    {
        actions.TryGetValue(ComponentLifecycleStep.OnEnable, out _onEnable);
        actions.TryGetValue(ComponentLifecycleStep.Start, out _start);
        actions.TryGetValue(ComponentLifecycleStep.FixedUpdate, out _fixedUpdate);
        actions.TryGetValue(ComponentLifecycleStep.Update, out _update);
        actions.TryGetValue(ComponentLifecycleStep.LateUpdate, out _lateUpdate);
        actions.TryGetValue(ComponentLifecycleStep.OnDisable, out _onDisable);
        actions.TryGetValue(ComponentLifecycleStep.OnDestroy, out _onDestroy);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void TryInvoke(ComponentLifecycleStep step, object target)
    {
        if (!TryGetAction(step, out Action<object>? action))
        {
            return;
        }

        action.Invoke(target);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public bool TryGetAction(
        ComponentLifecycleStep lifecycleType,
        [NotNullWhen(true)] out Action<object>? action)
    {
        action = lifecycleType switch
        {
            ComponentLifecycleStep.OnEnable => _onEnable,
            ComponentLifecycleStep.Start => _start,
            ComponentLifecycleStep.FixedUpdate => _fixedUpdate,
            ComponentLifecycleStep.Update => _update,
            ComponentLifecycleStep.LateUpdate => _lateUpdate,
            ComponentLifecycleStep.OnDisable => _onDisable,
            ComponentLifecycleStep.OnDestroy => _onDestroy,
            _ => throw new ArgumentOutOfRangeException(nameof(lifecycleType), lifecycleType, null),
        };

        return action != null;
    }
}
