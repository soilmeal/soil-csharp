using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Soil.SimpleActorModel.Actors;

public class ReceiveActor : Actor
{
    public static readonly Action<object?> DefaultHandler = (_) => { };

    private bool _handlerLocked = false;

    private readonly Dictionary<Type, Action<object>> _handlers = new();

    private Action<object?> _defaultHandler = DefaultHandler;

    public ReceiveActor Receive<T>(Action<T> action)
    {
        ThrowIfHandlerLocked();

        _handlers[typeof(T)] = action != null
            ? (val) => action((T)val)
            : throw new ArgumentNullException(nameof(action));

        return this;
    }

    public ReceiveActor Default(Action<object?> action)
    {
        ThrowIfHandlerLocked();

        _defaultHandler = action ?? throw new ArgumentNullException(nameof(action));

        return this;
    }

    public override void HandleCreate()
    {
        OnCreate();

        _handlerLocked = true;
    }

    protected sealed override void OnReceive(object? message)
    {
        if (message == null)
        {
            _defaultHandler(message);
            return;
        }

        if (!_handlers.TryGetValue(message.GetType(), out Action<object>? handler))
        {
            _defaultHandler(message);
            return;
        }

        handler(message);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ThrowIfHandlerLocked()
    {
        if (_handlerLocked)
        {
            throw new InvalidOperationException("Default() only available on constructor or OnCreate()");
        }
    }
}
