using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;

namespace Soil.SimpleActorModel.Actors;

public class ActorRoot : ActorCell
{
    private static readonly ActorProps _rootProps = new ActorProps()
        .WithDispatcher(
            Dispatchers.ActorRootDispatcherId,
            DispatcherType.CurrentThread,
            int.MaxValue)
        .WithMailbox(MailboxType.UnboundedMailboxType)
        .WithActorFactory(new ActorRootActorFactory());

    public ActorRoot(ActorSystem system)
        : base(system, system, _rootProps)
    {
    }

    private class ActorRootActorFactory : IActorFactory
    {
        public AbstractActor Create()
        {
            return new ActorRootActor();
        }
    }

    private class ActorRootActor : Actor
    {
        protected override void OnReceive(object message)
        {
        }
    }
}
