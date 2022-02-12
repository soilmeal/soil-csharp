using System;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Actors;
using Soil.SimpleActorModel.Message;

namespace Soil.SimpleActorModel.Dispatcher;

public static class Dispatchers
{
    public const string DefaulId = "default-task-scheduler-dispatcher";

    public const string DefaultType = DispatcherType.TaskScheduler;

    public const int DefaultThroughput = 100;

    public const string NoneId = "none-dispatcher";

    public static readonly IDispatcher None = new NoneDispatcher();

    internal static readonly string ActorRootDispatcherId = "actor-root-dispatcher";

    private class NoneDispatcher : IDispatcher
    {
        public string Id
        {
            get
            {
                return NoneId;
            }
        }

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
