using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : ICanTell, IEquatable<IActorRef>
{
    ActorRefState State { get; }

    AbstractActor Actor();

    T? Actor<T>()
        where T : AbstractActor;

    bool CanReceiveMessage();
}
