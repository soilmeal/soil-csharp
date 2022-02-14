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
                return 0;
            }
        }

        public void Dispatch(ActorCell actorCell, Envelope envelope)
        {
        }

        public void Dispose()
        {
        }

        public Task Execute(Action action)
        {
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
            return false;
        }
    }
}
