using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : IEquatable<IActorRef>
{
    ActorRefState State { get; }

    AbstractActor Actor();

    T Actor<T>()
        where T : AbstractActor;

    bool CanReceiveMessage();

    void Tell(object? message);

    void Tell(object? message, IActorRef sender);

    Task<object?> Ask(object? message);

    Task<T?> Ask<T>(object? message);
}
