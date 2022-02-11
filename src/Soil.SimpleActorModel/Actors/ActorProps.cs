using System;
using Soil.SimpleActorModel.Dispatchers;
using Soil.SimpleActorModel.Mailboxes;

namespace Soil.SimpleActorModel.Actors;

public class ActorProps
{
    private readonly IDispatcherProvider _dispatcherProvider;

    private readonly IMailboxProvider _mailboxProvider;

    private IActorFactory _actorFactory;

    public IDispatcherProvider DispatcherProvider
    {
        get
        {
            return _dispatcherProvider;
        }
    }

    public IMailboxProvider MailboxProvider
    {
        get
        {
            return _mailboxProvider;
        }
    }

    public IActorFactory ActorFactory
    {
        get
        {
            return _actorFactory;
        }
    }

    public ActorProps(IDispatcherProvider dispatcherProvider, IMailboxProvider mailboxProvider)
    {
        _dispatcherProvider = dispatcherProvider;
        _mailboxProvider = mailboxProvider;
        _actorFactory = ActorFactories.None;
    }

    public ActorProps SetActorFactory(IActorFactory actorFactory)
    {
        _actorFactory = actorFactory ?? throw new ArgumentNullException(nameof(actorFactory));
        return this;
    }

    public ActorProps SetActorFactory(Func<AbstractActor> actorFactoryFunc)
    {
        return SetActorFactory(IActorFactory.CreateFactory(actorFactoryFunc));
    }
}
