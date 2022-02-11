using System;
using System.Collections.Generic;
using Soil.Collections.Generics;
using Soil.SimpleActorModel.Dispatchers;
using Soil.SimpleActorModel.Mailboxes;
using Soil.SimpleActorModel.Messages;
using Soil.SimpleActorModel.Messages.System;

namespace Soil.SimpleActorModel.Actors;

public class ActorCell : IActorContext, IEquatable<ActorCell>
{
    private readonly AbstractActor _actor;

    private readonly IActorRef _parent;

    private readonly IDispatcher _dispatcher;

    private readonly Mailbox _mailbox;

    private readonly CopyOnWriteList<IActorRef> _children = new();

    private IActorRef _sender = ActorRefs.NoSender;

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

    public ActorCell(IActorRef parent, ActorProps props)
    {
        _actor = props.ActorFactory.Create();
        _actor.Context = this;

        _parent = parent;
        _dispatcher = props.DispatcherProvider.Provide();
        _mailbox = props.MailboxProvider.Provide(this);
    }

    public IActorRef Create(ActorProps props)
    {
        var child = new ActorCell(this, props);
        _children.Add(child);

        child.Mailbox.TryAddSystemMessage(Messages.System.Create.Instance);
        child.Dispatcher.TryExecuteMailbox(child.Mailbox);

        return child;
    }

    public void Start()
    {
        _mailbox.TryAddSystemMessage(Messages.System.Start.Instance);
        _dispatcher.TryExecuteMailbox(_mailbox);
    }

    public void Stop()
    {
        foreach (var child in _children)
        {
            child.Stop();
        }

        _mailbox.TryAddSystemMessage(Messages.System.Stop.Instance);
        _dispatcher.TryExecuteMailbox(_mailbox);
    }

    public void Send(object message)
    {
        Send(message, ActorRefs.NoSender);
    }

    public void Send(object message, IActorRef sender)
    {
        _dispatcher.Dispatch(this, new Envelope(message, sender));
    }

    public void Invoke(Envelope envelope)
    {
        _sender = envelope.Sender;
        _actor.HandleReceive(envelope.Message);
    }

    public void InvokeSystem(SystemMessage message)
    {
        _sender = this;

        switch (message)
        {
            case Messages.System.Create:
            {
                _actor.HandleCreate();
                break;
            }
            case Messages.System.Start:
            {
                _actor.HandleStart();
                if (_mailbox.Count > 0)
                {
                    _dispatcher.TryExecuteMailbox(_mailbox);
                }
                break;
            }
            case Messages.System.Stop:
            {
                _actor.HandleStop();
                _mailbox.Close();
                break;
            }
        }
    }

    public bool Equals(IActorRef? other)
    {
        return other is ActorCell actorCell && Equals(actorCell);
    }

    public override bool Equals(object? obj)
    {
        return obj is ActorCell actorCell && Equals(actorCell);
    }

    public bool Equals(ActorCell? other)
    {
        return ReferenceEquals(this, other);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }
}
