using System;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : IEquatable<IActorRef>
{
    ActorRefState State { get; }


    AbstractActor GetActor();

    T GetActor<T>()
        where T : AbstractActor;

    bool CanReceiveMessage();

    void Start();

    void Stop();

    void Send(object message);

    void Send(object message, IActorRef sender);
}
