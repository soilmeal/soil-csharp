using System;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;

namespace Soil.SimpleActorModel.Actors;

public class ActorProps
{
    private readonly DispatcherProps _dispatcherProps = new();

    private readonly MailboxProps _mailboxProps = new();

    private IActorFactory _actorFactory;

    private bool _autoStart = false;

    public IActorFactory ActorFactory
    {
        get
        {
            return _actorFactory;
        }
    }

    public DispatcherProps DispatcherProps
    {
        get
        {
            return _dispatcherProps;
        }
    }

    public MailboxProps MailboxProps
    {
        get
        {
            return _mailboxProps;
        }
    }

    public bool AutoStart
    {
        get
        {
            return _autoStart;
        }
    }

    public ActorProps()
    {
        _actorFactory = Actors.NoneFactory;
    }

    public ActorProps WithDispatcher(string id)
    {
        _dispatcherProps.SetId(id);

        return this;
    }

    public ActorProps WithDispatcher(string id, string type)
    {
        _dispatcherProps.SetId(id)
            .SetType(type);

        return this;
    }

    public ActorProps WithDispatcher(string id, string type, int throughputPerActor)
    {
        _dispatcherProps.SetId(id)
            .SetType(type)
            .SetThroughputPerActor(throughputPerActor);

        return this;
    }

    public ActorProps WithMailbox(string type)
    {
        _mailboxProps.SetType(type);

        return this;
    }

    public ActorProps WithActorFactory(IActorFactory actorFactory)
    {
        _actorFactory = actorFactory ?? throw new ArgumentNullException(nameof(actorFactory));

        return this;
    }

    public ActorProps WithActorFactory(Func<AbstractActor> actorFactoryFunc)
    {
        return WithActorFactory(IActorFactory.CreateFactory(actorFactoryFunc));
    }

    public ActorProps WithAutoStart(bool autoStart)
    {
        _autoStart = autoStart;

        return this;
    }
}
