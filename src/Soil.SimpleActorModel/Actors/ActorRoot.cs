using Soil.SimpleActorModel.Dispatchers;
using Soil.SimpleActorModel.Mailboxes;

namespace Soil.SimpleActorModel.Actors;

public class ActorRoot : ActorCell
{
    private static readonly IDispatcherProvider _rootDispatcherProvider = new DefaultDispatcherProvider(new DispatcherProps(DefaultDispatcherType.CurrentThreadDispatcherType, int.MaxValue));

    private static readonly IMailboxProvider _rootMailboxProvider = new DefaultMailboxProvider(new MailboxProps(DefaultMailboxType.UnboundedMailboxType));

    private static readonly ActorProps _rootProps = new ActorProps(
        _rootDispatcherProvider,
        _rootMailboxProvider)
        .SetActorFactory(new ActorRootActorFactory());
    public ActorRoot()
        : base(ActorRefs.None, _rootProps)
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
