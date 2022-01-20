using System;
using System.Collections.Generic;
using Soil.Math;

namespace Soil.Game;

public class World
{
    private readonly CoordinateSystem _coordinateSystem;

    private readonly ComponentLifecycleInfoRegistry _registry;

    private readonly Timer _time;

    private readonly List<GameObject> _gameObjects = new(1024);

    private readonly List<GameObject> _destroyReserved = new(1024);

    public CoordinateSystem CoordinateSystem
    {
        get
        {
            return _coordinateSystem;
        }
    }

    public ITimer Time
    {
        get
        {
            return _time;
        }
    }

    public ComponentLifecycleInfoRegistry Registry
    {
        get
        {
            return _registry;
        }
    }

    public World(
        CoordinateSystem coordinateSystem,
        ComponentLifecycleInfoRegistry registry,
        TimeSpan fixedTimeStep)
    {
        _coordinateSystem = coordinateSystem;
        _registry = registry;
        _time = new Timer(fixedTimeStep);
        _time.OnUpdate += HandleTimeUpdate;
        _time.OnFixedUpdate += HandleTimeFixedUpdate;
    }

    public GameObject CreateObject(bool active = true, List<Component>? components = null)
    {
        var gameObject = new GameObject(this);
        if (components == null)
        {
            _gameObjects.Add(gameObject);

            gameObject.SetActive(active);

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

        _gameObjects.Add(gameObject);

        gameObject.SetActive(active);

        return gameObject;
    }

    public void Update()
    {
        TimeSpan currTime = DateTime.UtcNow.TimeOfDay;

        foreach (var gameObject in _gameObjects)
        {
            gameObject.PreUpdate();
        }

        _time.Update(currTime);

        foreach (var gameObject in _gameObjects)
        {
            gameObject.LateUpdate();
        }

        foreach (var gameObject in _destroyReserved)
        {
            gameObject.HandleDestroy(destroyed => _gameObjects.Remove(destroyed));

            _gameObjects.Remove(gameObject);
        }

        _destroyReserved.Clear();
    }

    public void Reset()
    {
        _gameObjects.Clear();
        _destroyReserved.Clear();
        _time.Reset();
    }

    internal void Destroy(GameObject gameObject)
    {
        if (!_gameObjects.Contains(gameObject))
        {
            return;
        }

        _destroyReserved.Add(gameObject);
    }

    private void HandleTimeUpdate()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.Update();
        }
    }

    private void HandleTimeFixedUpdate()
    {
        foreach (var gameObject in _gameObjects)
        {
            gameObject.FixedUpdate();
        }
    }
}
