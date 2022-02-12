using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Message;

namespace Soil.SimpleActorModel.Dispatcher;

public interface IDispatcher : IExecutor
{
    string Id { get; }

    int ThroughputPerActor { get; }

    void Dispatch(ActorCell actorCell, Envelope envelope);

    bool TryExecuteMailbox(Mailbox mailbox);
}
