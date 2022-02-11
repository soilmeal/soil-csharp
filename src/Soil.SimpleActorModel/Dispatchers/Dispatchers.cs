using System;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Mailboxes;
using Soil.SimpleActorModel.Messages;

namespace Soil.SimpleActorModel.Dispatchers;

public static class Dispatchers
{
    public static readonly IDispatcher None = new NoneDispatcher();

    private class NoneDispatcher : IDispatcher
    {
        public int ThroughputPerActor
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public void Dispatch(ActorCell actorCell, Envelope envelope)
        {
            throw new NotSupportedException();
        }

        public void Dispose()
        {
            throw new NotSupportedException();
        }

        public Task Execute(Action action)
        {
            throw new NotSupportedException();
        }

        public void JoinAll()
        {
            throw new NotSupportedException();
        }

        public void JoinAll(TimeSpan timeout)
        {
            throw new NotSupportedException();
        }

        public void JoinAll(int millisecondsTimeout)
        {
            throw new NotSupportedException();
        }

        public bool TryExecuteMailbox(Mailbox mailbox)
        {
            throw new NotSupportedException();
        }
    }
}
