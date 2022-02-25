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

    public T1? Actor<T1>()
        where T1 : AbstractActor
    {
        return Actor() as T1;
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
            default:
            {
                string typename = message?.GetType().Name ?? "null";
                _taskCompletionSource!.TrySetException(new InvalidCastException($"cannot cast as {nameof(T)} - typename={typename}"));
                break;
            }
        }
    }

    public void Tell(object? message, IActorRef sender)
    {
        Tell(message);
    }
}
