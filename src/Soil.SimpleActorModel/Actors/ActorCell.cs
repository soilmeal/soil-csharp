using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using System.Threading;
using Soil.Collections.Generics;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;
using Soil.SimpleActorModel.Message.System;
using Soil.Threading.Atomic;

namespace Soil.SimpleActorModel.Actors;

public class ActorCell : IActorContext, IEquatable<ActorCell>
{
    private readonly ActorSystem _system;

    private readonly AbstractActor _actor;

    private readonly IActorRef _parent;

    private readonly IDispatcher _dispatcher;

    private readonly Mailbox _mailbox;

    private readonly CopyOnWriteList<IActorRef> _children = new();

    private readonly AtomicInt32 _state = (int)ActorRefState.Created;

    private readonly ManualResetEventSlim _stopSync = new(false);

    private IActorRef _sender = ActorRefs.NoSender;

    public ActorRefState State
    {
        get
        {
            return GetState();
        }
    }

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

    public AbstractActor GetActor()
    {
        return _actor;
    }

    public T GetActor<T>() where T : AbstractActor
    {
        return (T)_actor;
    }

    public bool CanReceiveMessage()
    {
        return GetState() switch
        {
            ActorRefState.Running
            or ActorRefState.Closing => true,
            _ => false,
        };
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
        ActorRefState oldState = CompareExchangeState(
            ActorRefState.Starting,
            ActorRefState.Created);
        if (oldState != ActorRefState.Created)
        {
            return;
        }

        _mailbox.TryAddSystemMessage(Message.System.Start.Instance);
        _dispatcher.TryExecuteMailbox(_mailbox);
    }

    public void Stop()
    {
        ActorRefState oldState = CompareExchangeState(
            ActorRefState.Closing,
            ActorRefState.Running);
        if (oldState != ActorRefState.Running)
        {
            return;
        }

        foreach (var child in _children)
        {
            child.Stop();
        }

        _mailbox.TryAddSystemMessage(Message.System.Stop.Instance);
        _dispatcher.TryExecuteMailbox(_mailbox);

        _stopSync.Wait();
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

                ExchangeState(ActorRefState.Running);
                break;
            }
            case Message.System.Stop:
            {
                _actor.HandleStop();

                ExchangeState(ActorRefState.Closed);

                _stopSync.Set();

                _mailbox.Close();
                break;
            }
            default:
            {
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

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ActorRefState GetState()
    {
        return (ActorRefState)_state.Read();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ActorRefState ExchangeState(ActorRefState state)
    {
        return (ActorRefState)_state.Exchange((int)state);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ActorRefState CompareExchangeState(ActorRefState state, ActorRefState comparandState)
    {
        return (ActorRefState)_state.CompareExchange((int)state, (int)comparandState);
    }
}
