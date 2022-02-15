using System;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Message;

namespace Soil.SimpleActorModel.Dispatcher;

public class CurrentThreadDispatcher : IDispatcher
{
    private readonly string _id;

    public string Id
    {
        get
        {
            return _id;
        }
    }

    public int ThroughputPerActor
    {
        get
        {
            return int.MaxValue;
        }
    }

    public CurrentThreadDispatcher(string id)
    {
        _id = id;
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

    public Task<T> Execute<T>(Func<T> func)
    {
        if (func == null)
        {
            throw new ArgumentNullException(nameof(func));
        }

        return Task.FromResult(func());
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
        if (!mailbox.HasAnyMessage())
        {
            return false;
        }

        if (!mailbox.TrySetScheduled())
        {
            return false;
        }

        Execute(mailbox.Process);
        return true;
    }
}
