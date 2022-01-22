using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Soil.Game;

public class GameObject : Object
{
    private readonly World _world;

    private readonly Transform _transform;

    private GameObject? _parent;

    private bool _active;

    private readonly List<GameObject> _children = new(32);

    private readonly List<Component> _components = new(8);

    private readonly List<Component> _destroyReserved = new(8);

    public bool Active
    {
        get
        {
            return _active;
        }
    }

    public bool ActiveInHierarchy
    {
        get
        {
            bool parentActive = _parent?._active ?? true;
            return parentActive && _active;
        }
    }

    public World World
    {
        get
        {
            return _world;
        }
    }

    public ITimer Time
    {
        get
        {
            return World.Time;
        }
    }

    public GameObject? Parent
    {
        get
        {
            return _parent;
        }
        set
        {
            _parent?.RemoveChild(this);

            if (value == null)
            {
                return;
            }

            value.AddChild(this);
            _parent = value;
        }
    }

    public Transform Transform
    {
        get
        {
            return _transform;
        }
    }

    internal GameObject(World world)
    {
        _world = world;
        _transform = AddComponent<Transform>();
        _transform.CoordinateSystem = _world.CoordinateSystem;
        _active = false;
    }

    public void SetActive(bool active)
    {
        if (_active == active)
        {
            return;
        }

        bool prevActiveInHierarchy = ActiveInHierarchy;
        _active = active;

        if (active)
        {
            if (prevActiveInHierarchy)
            {
                return;
            }

            HandleActive();
        }
        else if (!prevActiveInHierarchy)
        {
            return;
        }

        HandleDeactive();
    }

    public T AddComponent<T>()
        where T : Component, new()
    {
        T component = new();
        component.GameObject = this;

        _components.Add(new T());
        return component;
    }

    public Component AddComponent(Component component)
    {
        component.GameObject = this;

        _components.Add(component);
        return component;
    }

    public T? GetComponent<T>()
        where T : Component
    {
        return _components.Find((component) => component is T) as T;
    }

    public GameObject CreateChild(bool active = true, List<Component>? components = null)
    {
        GameObject child = _world.CreateObject(active, components);
        child.Parent = this;

        return child;
    }

    internal void Destroy(Component component)
    {
        if (!_components.Contains(component))
        {
            return;
        }

        _destroyReserved.Add(component);
    }

    internal void HandleDestroy(Action<GameObject> action)
    {
        foreach (var child in _children)
        {
            child.HandleDestroy(action);
        }

        if (!ActiveInHierarchy)
        {
            ClearAll();
            action(this);
            return;
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                continue;
            }

            component.InvokeOnDisableIfHas();

            component.InvokeOnDestroyIfHas();
        }

        ClearAll();
        action(this);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PreUpdate()
    {
        if (!ActiveInHierarchy)
        {
            return;
        }

        if (_components.Count <= 0)
        {
            return;
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                return;
            }

            if (component.Started)
            {
                return;
            }

            component.InvokeStartIfHas();

            component.Started = true;
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void FixedUpdate()
    {
        if (!ActiveInHierarchy)
        {
            return;
        }

        if (_components.Count <= 0)
        {
            return;
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                continue;
            }

            component.InvokeFixedUpdateIfHas();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void Update()
    {
        if (!ActiveInHierarchy)
        {
            return;
        }

        if (_components.Count <= 0)
        {
            return;
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                continue;
            }

            component.InvokeUpdateIfHas();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void LateUpdate()
    {
        if (!ActiveInHierarchy)
        {
            return;
        }

        if (_components.Count <= 0)
        {
            return;
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                continue;
            }

            component.InvokeLateUpdateIfHas();
        }
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal void PostUpdate()
    {
        if (_destroyReserved.Count <= 0)
        {
            return;
        }

        foreach (var component in _destroyReserved)
        {
            component.InvokeOnDestroyIfHas();

            _components.Remove(component);
        }

        _destroyReserved.Clear();
    }

    private void HandleActive()
    {
        foreach (var child in _children)
        {
            if (!child._active)
            {
                continue;
            }

            child.HandleActive();
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                continue;
            }

            component.InvokeOnEnableIfHas();
        }
    }

    private void HandleDeactive()
    {
        foreach (var child in _children)
        {
            if (!child._active)
            {
                continue;
            }

            child.HandleDeactive();
        }

        foreach (var component in _components)
        {
            if (!component.Enabled)
            {
                continue;
            }

            component.InvokeOnDisableIfHas();
        }
    }

    private void ClearAll()
    {
        Parent = null;
        _components.Clear();
        _children.Clear();
    }

    private void AddChild(GameObject child)
    {
        _children.Add(child);
    }

    private bool RemoveChild(GameObject child)
    {
        return _children.Remove(child);
    }
}
