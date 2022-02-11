using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Mailboxes;
using Soil.SimpleActorModel.Messages;

namespace Soil.SimpleActorModel.Dispatchers;

public interface IDispatcher : IExecutor
{
    int ThroughputPerActor { get; }

    void Dispatch(ActorCell actorCell, Envelope envelope);

    bool TryExecuteMailbox(Mailbox mailbox);
}
