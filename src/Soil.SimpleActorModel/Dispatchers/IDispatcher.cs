using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Mailboxes;

namespace Soil.SimpleActorModel.Dispatchers;

public interface IDispatcher : IExecutor
{
    string Name { get; }

    int ThroughputPerActor { get; }

    void Dispatch(ActorCell actorCell, Envelope envelope);

    bool TryExecuteMailbox(Mailbox mailbox);
}
