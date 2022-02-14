using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : IEquatable<IActorRef>
{

    ActorRefState State { get; }

    AbstractActor Actor();

    T Actor<T>()
        where T : AbstractActor;

    bool CanReceiveMessage();

    void Start();

    Task StartAsync();

    void Stop(bool waitChildren);

    Task StopAsync(bool waitChildren);

    void Tell(object? message);

    void Tell(object? message, IActorRef sender);

    Task<object?> Ask(object? message);

    Task<T?> Ask<T>(object? message);
}
