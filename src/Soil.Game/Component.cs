using System;
using System.Linq;
using System.Reflection;

namespace Soil.Game;

public class Component
{
    private GameObject? _gameObject;

    private bool _enabled;

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

    public bool Enabled
    {
        get
        {
            return _enabled;
        }
        set
        {
        }
    }

    public bool IsActiveAndEnabled
    {
        get
        {
            return (_gameObject?.Active ?? true) && _enabled;
        }
    }

    public Component()
    {
    }
}
