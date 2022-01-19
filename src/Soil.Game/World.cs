using System;
using System.Collections.Generic;
using System.Threading;
using Soil.Math;
using Soil.Threading;

namespace Soil.Game;

public class World
{
    private readonly CoordinateSystem _coordinateSystem;

    private readonly ComponentLifecycleInfoRegistry _registry;

    private readonly TimeSpan _frameMillis;

    private readonly IThreadFactory _threadFactory;

    private readonly Thread _thread;

    public CoordinateSystem CoordinateSystem
    {
        get
        {
            return _coordinateSystem;
        }
    }

    internal ComponentLifecycleInfoRegistry Registry
    {
        get
        {
            return _registry;
        }
    }

    public World(
        CoordinateSystem coordinateSystem,
        ComponentLifecycleInfoRegistry registry,
        double fps)
    {
        _coordinateSystem = coordinateSystem;
        _registry = registry;
        _frameMillis = TimeSpan.FromMilliseconds(fps / 1000);
    }

    public GameObject CreateObject(bool active = true, List<Component>? components = null)
    {
        var gameObject = new GameObject(this);
        gameObject.SetActive(active);

        if (components == null)
        {
            return gameObject;
        }

        foreach (var component in components)
        {
            if (component == null)
            {
                continue;
            }

            gameObject.AddComponent(component);
        }
        return gameObject;
    }

    public void Update(float delta)
    {
    }
}
