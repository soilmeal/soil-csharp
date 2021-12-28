using System;
using System.Threading.Tasks;
using Soil.Core.Event;
using Soil.Core.Threading.Atomic;

namespace Soil.Net.Event;

public class EventSource : IEventSource
{
    private readonly AtomicBool _isRegistered = new();
    public bool IsRegistered
    {
        get
        {
            return _isRegistered.Read();
        }
    }

    private IEventLoop? _eventLoop;
    public IEventLoop? EventLoop
    {
        get
        {
            return _eventLoop;
        }
    }

    public EventSource()
    {
    }


    public void Register(IEventLoop eventLoop, Action? action = default)
    {
        RegisterAsync(eventLoop)
            .GetAwaiter()
            .GetResult();

        action?.Invoke();
    }

    public void Register(IEventLoopGroup eventLoopGroup, Action? action = default)
    {
        RegisterAsync(eventLoopGroup)
            .GetAwaiter()
            .GetResult();

        action?.Invoke();
    }

    public Task RegisterAsync(IEventLoop eventLoop)
    {
        if (eventLoop == null)
        {
            throw new ArgumentNullException(nameof(eventLoop));
        }

        if (_isRegistered.Read())
        {
            throw new InvalidOperationException("Already registered");
        }

        TaskFactory taskFactory = eventLoop.TaskFactory;
        return taskFactory.StartNew(() =>
        {
            if (_isRegistered.CompareExchange(true, false))
            {
                throw new InvalidOperationException("Already registered");
            }

            _eventLoop = eventLoop;
        });
    }

    public Task RegisterAsync(IEventLoopGroup eventLoopGroup)
    {
        if (eventLoopGroup == null)
        {
            throw new ArgumentNullException(nameof(eventLoopGroup));
        }

        IEventLoop eventLoop = eventLoopGroup.Next();
        return RegisterAsync(eventLoop);
    }

    public void Deregister(Action? action = null)
    {
        DeregisterAsync()
            .GetAwaiter()
            .GetResult();

        action?.Invoke();
    }


    public Task DeregisterAsync()
    {
        if (!_isRegistered.Read())
        {
            throw new InvalidOperationException("Already deregistered");
        }

        IEventLoop? eventLoop = _eventLoop;
        if (eventLoop == null)
        {
            throw new InvalidOperationException("Already deregistered");
        }

        TaskFactory taskFactory = eventLoop.TaskFactory;
        return taskFactory.StartNew(() =>
        {
            if (!_isRegistered.CompareExchange(false, true))
            {
                throw new InvalidOperationException("Already deregistered");
            }

            _eventLoop = null;
        });
    }
}
