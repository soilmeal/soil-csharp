using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : IEquatable<IActorRef>
{
    ActorRefState State { get; }


    AbstractActor GetActor();

    T GetActor<T>()
        where T : AbstractActor;

    bool CanReceiveMessage();

    void Start();

    Task StartAsync();

    void Stop(bool waitChildren);

    Task StopAsync(bool waitChildren);

    void Send(object message);

    void Send(object message, IActorRef sender);
}
