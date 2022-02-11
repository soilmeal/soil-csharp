using System;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Mailboxes;
using Soil.SimpleActorModel.Messages;

namespace Soil.SimpleActorModel.Dispatchers;

public class CurrentThreadDispatcher : IDispatcher
{

    public int ThroughputPerActor
    {
        get
        {
            return int.MaxValue;
        }
    }

    public CurrentThreadDispatcher()
    {
    }

    public void Dispatch(ActorCell actorCell, Envelope envelope)
    {
        if (!actorCell.Mailbox.TryAdd(envelope))
        {
            return;
        }

        TryExecuteMailbox(actorCell.Mailbox);
    }

    public void Dispose()
    {
    }

    public Task Execute(Action action)
    {
        if (action == null)
        {
            throw new ArgumentNullException(nameof(action));
        }

        action();
        return Task.CompletedTask;
    }

    public void JoinAll()
    {
    }

    public void JoinAll(TimeSpan timeout)
    {
    }

    public void JoinAll(int millisecondsTimeout)
    {
    }

    public bool TryExecuteMailbox(Mailbox mailbox)
    {
        if (!mailbox.TrySetScheduled())
        {
            return false;
        }

        Execute(mailbox.Process);
        return true;
    }
}
