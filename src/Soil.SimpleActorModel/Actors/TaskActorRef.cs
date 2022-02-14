using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

internal class TaskActorRef<T> : IActorRef, IEquatable<TaskActorRef<T>>
{
    private TaskCompletionSource<T>? _taskCompletionSource;

    public ActorRefState State
    {
        get
        {
            return _taskCompletionSource != null ? ActorRefState.Running : ActorRefState.Closed;
        }
    }

    public TaskActorRef()
    {
        _taskCompletionSource = null;
    }

    public void Initialize(TaskCompletionSource<T> taskCompletionSource)
    {
        _taskCompletionSource = taskCompletionSource;
    }

    public void Reset()
    {
        _taskCompletionSource = null;
    }

    public AbstractActor Actor()
    {
        return Actors.None;
    }

    public T1 Actor<T1>() where T1 : AbstractActor
    {
        return (T1)Actor();
    }

    public Task<object?> Ask(object? message)
    {
        return Ask<object?>(message);
    }

    public Task<T1?> Ask<T1>(object? message)
    {
        return Task.FromResult<T1?>(default);
    }

    public bool CanReceiveMessage()
    {
        return _taskCompletionSource != null;
    }

    public bool Equals(IActorRef? other)
    {
        return other is TaskActorRef<T> taskActorRef && Equals(taskActorRef);
    }

    public bool Equals(TaskActorRef<T>? other)
    {
        return ReferenceEquals(this, other);
    }

    public void Start()
    {
    }

    public Task StartAsync()
    {
        return Task.CompletedTask;
    }

    public void Stop(bool waitChildren)
    {
    }

    public Task StopAsync(bool waitChildren)
    {
        return Task.CompletedTask;
    }

    public void Tell(object? message)
    {
        if (!CanReceiveMessage())
        {
            throw new InvalidOperationException("call Initialize() first!");
        }

        switch (message)
        {
            case T t:
            {
                _taskCompletionSource!.TrySetResult(t);
                break;
            }
            case null:
            {
                _taskCompletionSource!.TrySetException(new InvalidOperationException($"cannot cast as {nameof(T)} - typename=null"));
                break;
            }
            default:
            {
                _taskCompletionSource!.TrySetException(new InvalidOperationException($"cannot cast as {nameof(T)} - typename={message.GetType().Name}"));
                break;
            }
        }
    }

    public void Tell(object? message, IActorRef sender)
    {
        Tell(message);
    }
}
