using System.Collections.Generic;

namespace Soil.Game;

public class GameObject
{
    private readonly World _world;

    private GameObject? _parent;

    private bool _active;

    private readonly Transform _transform;

    private readonly List<GameObject> _children = new();

    private readonly List<Component> _components = new();

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
        _active = false;
    }

    public void SetActive(bool active)
    {
        _active = active;
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

    public T GetComponent<T>()
        where T : Component
    {
        return (_components.Find((component) => component is T) as T)!;
    }

    public GameObject CreateChild(bool isActive = true, List<Component>? components = null)
    {
        GameObject child = _world.CreateObject(isActive, components);
        child.Parent = this;

        return child;
    }

    public void Destroy()
    {
    }

    internal void ReserveDestroyComponent(Component component)
    {
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
