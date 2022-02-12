using System;
using System.Collections.Generic;
using Soil.Collections.Generics;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;
using Soil.SimpleActorModel.Message.System;

namespace Soil.SimpleActorModel.Actors;

public class ActorCell : IActorContext, IEquatable<ActorCell>
{
    private readonly ActorSystem _system;

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

    public ActorCell(ActorSystem system, IActorRef parent, ActorProps props)
    {
        _system = system;

        _actor = props.ActorFactory.Create();
        _actor.Context = this;

        _parent = parent;
        _dispatcher = system.GetOrCreateDispatcher(props.DispatcherProps);
        _mailbox = system.CreateMailbox(this, props.MailboxProps);
    }

    public IActorRef Create(ActorProps props)
    {
        var child = new ActorCell(_system, this, props);
        _children.Add(child);

        child.Mailbox.TryAddSystemMessage(Message.System.Create.Instance);
        child.Dispatcher.TryExecuteMailbox(child.Mailbox);

        return child;
    }

    public void Start()
    {
        _mailbox.TryAddSystemMessage(Message.System.Start.Instance);
        _dispatcher.TryExecuteMailbox(_mailbox);
    }

    public void Stop()
    {
        foreach (var child in _children)
        {
            child.Stop();
        }

        _mailbox.TryAddSystemMessage(Message.System.Stop.Instance);
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
            case Message.System.Create:
            {
                _actor.HandleCreate();
                break;
            }
            case Message.System.Start:
            {
                _actor.HandleStart();
                if (_mailbox.Count > 0)
                {
                    _dispatcher.TryExecuteMailbox(_mailbox);
                }
                break;
            }
            case Message.System.Stop:
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
