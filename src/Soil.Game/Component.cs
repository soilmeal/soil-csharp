using System;
using System.Runtime.CompilerServices;

namespace Soil.Game;

public class Component : Object
{
    private GameObject? _gameObject;

    private bool _enabled;

    private bool _started;

    private ComponentLifecycleInfo? _componentLifecycleInfo;

    public GameObject? GameObject
    {
        get
        {
            return _gameObject;
        }
        internal set
        {
            _gameObject = value;
            if (_gameObject == null)
            {
                return;
            }

            _gameObject.World.Registry.TryGetValue(GetType(), out _componentLifecycleInfo);
        }
    }

    public Transform? Transform
    {
        get
        {
            return _gameObject?.Transform;
        }
    }

    public ITimer? Time
    {
        get
        {
            return _gameObject?.Time;
        }
    }

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
            if (_enabled == value)
            {
                return;
            }

            bool prevActiveAndEnabled = IsActiveAndEnabled;
            _enabled = value;
            if (value)
            {
                if (prevActiveAndEnabled)
                {
                    return;
                }

                InvokeOnEnableIfHas();
            }
            else if (!prevActiveAndEnabled)
            {
                return;
            }

            InvokeOnDisableIfHas();
        }
    }

    public bool IsActiveAndEnabled
    {
        get
        {
            bool active = _gameObject?.ActiveInHierarchy ?? true;
            return active && _enabled;
        }
    }

    internal bool Started
    {
        get
        {
            return _started;
        }
        set
        {
            _started = true;
        }
    }

    public Component()
    {
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeStartIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.OnEnable, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnEnableIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.OnEnable, this);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeFixedUpdateIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.FixedUpdate, this);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeUpdateIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.Update, this);
    }


    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeLateUpdateIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.LateUpdate, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnDisableIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.OnDisable, this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void InvokeOnDestroyIfHas()
    {
        _componentLifecycleInfo?.TryInvoke(ComponentLifecycleStep.OnDestroy, this);
    }
}
