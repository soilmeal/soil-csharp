using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Soil.Collections.Generics;
using Soil.SimpleActorModel.Dispatchers;
using Soil.SimpleActorModel.Mailboxes;

namespace Soil.SimpleActorModel.Actors;

public class ActorCell : IActorContext, IActorRef
{
    private readonly AbstractActor _actor;

    private readonly IActorRef _parent;

    private readonly IDispatcher _dispatcher;

    private readonly Mailbox _mailbox;

    private readonly CopyOnWriteList<IActorRef> _children = new();

    private IActorRef _sender = IActorRef.NoSender;

    public IActorRef Parent
    {
        get
        {
            return _parent;
        }
    }

    public IActorRef Self
    {
        get
        {
            return this;
        }
    }

    public IActorRef Sender
    {
        get
        {
            return _sender;
        }
    }

    public IEnumerable<IActorRef> Children
    {
        get
        {
            return _children;
        }
    }

    public IDispatcher Dispatcher
    {
        get
        {
            return _dispatcher;
        }
    }

    public Mailbox Mailbox
    {
        get
        {
            return _mailbox;
        }
    }

    public ActorCell(
        AbstractActor actor,
        IActorRef parent,
        IDispatcher dispatcher,
        Mailbox mailbox)
    {
        _actor = actor;
        _parent = parent;
        _dispatcher = dispatcher;
        _mailbox = mailbox;
    }

    public void Invoke(Envelope envelope)
    {
        _sender = envelope.Sender;
        throw new NotImplementedException();
    }

    public IActorRef Create(AbstractActor actor)
    {
        throw new NotImplementedException();
    }

    public void Start()
    {
        throw new NotImplementedException();
    }

    public void Stop()
    {
        throw new NotImplementedException();
    }

    public void Stop(int millisecondsTimeout)
    {
        throw new NotImplementedException();
    }

    public void Stop(TimeSpan timeout)
    {
        throw new NotImplementedException();
    }

    public bool Send(object message)
    {
        throw new NotImplementedException();
    }

    public bool Send(object message, IActorRef sender)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendAsync(object message)
    {
        throw new NotImplementedException();
    }

    public Task<bool> SendAsync(object message, IActorRef sender)
    {
        throw new NotImplementedException();
    }

    public bool Equals(IActorRef other)
    {
        throw new NotImplementedException();
    }
}
