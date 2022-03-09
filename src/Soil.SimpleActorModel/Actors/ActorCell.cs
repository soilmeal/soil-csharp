using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
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

    private readonly IActorContext _parent;

    private readonly IDispatcher _dispatcher;

    private readonly Mailbox _mailbox;

    private readonly bool _autoStart;

    private readonly CopyOnWriteList<IActorContext> _children = new();

    private readonly AtomicInt32 _state = (int)ActorRefState.Created;

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

    public ActorCell(ActorSystem system, IActorContext parent, ActorProps props)
    {
        _system = system;

        _actor = props.ActorFactory.Create();
        _actor.Context = this;

        _parent = parent;
        _dispatcher = system.GetOrCreateDispatcher(props.DispatcherProps);
        _mailbox = system.CreateMailbox(this, props.MailboxProps);
        _autoStart = props.AutoStart;
    }

    public AbstractActor Actor()
    {
        return _actor;
    }

    public T? Actor<T>()
        where T : AbstractActor
    {
        return _actor as T;
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

    public IActorContext Create(ActorProps props)
    {
        var child = new ActorCell(_system, this, props);
        _children.Add(child);

        child.Mailbox.TryAddSystemMessage(Message.System.Create.Instance);
        child.Dispatcher.TryExecuteMailbox(child.Mailbox);

        return child;
    }

    public void Start()
    {
        StartAsync()
            .Wait();
    }

    public Task StartAsync()
    {
        if (!CanStart())
        {
            return Task.FromException(new InvalidOperationException("start parent first!"));
        }

        if (_autoStart)
        {
            return Task.CompletedTask;
        }

        return EnqueueStartMessage();
    }

    public void Stop(bool waitChildren)
    {
        ActorRefState oldState = CompareExchangeState(
            ActorRefState.Closing,
            ActorRefState.Running);
        if (oldState != ActorRefState.Running)
        {
            return;
        }

        StopChildren(waitChildren).Wait();

        Stop stop = Message.System.Stop.Create();
        Tell(stop);

        stop.Task.Wait();
    }

    public async Task StopAsync(bool waitChildren)
    {
        ActorRefState oldState = CompareExchangeState(
            ActorRefState.Closing,
            ActorRefState.Running);
        if (oldState != ActorRefState.Running)
        {
            return;
        }

        await StopChildren(waitChildren);

        Stop stop = Message.System.Stop.Create();
        Tell(stop);

        await stop.Task;
    }

    public void Tell(object? message)
    {
        Tell(message, ActorRefs.NoSender);
    }

    public void Tell(object? message, IActorRef sender)
    {
        _dispatcher.Dispatch(this, new Envelope(message, sender));
    }

    public void Invoke(Envelope envelope)
    {
        switch (envelope.Message)
        {
            case Stop stop:
            {
                InvokeSystem(stop);
                break;
            }
            default:
            {
                _sender = envelope.Sender;
                _actor.HandleReceive(envelope.Message);
                break;
            }
        }
    }

    public void InvokeSystem(SystemMessage message)
    {
        switch (message)
        {
            case Message.System.Create:
            {
                _actor.HandleCreate();

                if (_autoStart)
                {
                    EnqueueStartMessage();
                }
                break;
            }
            case Start start:
            {
                try
                {
                    _actor.HandleStart();

                    start.TaskCompletionSource.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    start.TaskCompletionSource.TrySetException(ex);
                }
                finally
                {
                    ExchangeState(ActorRefState.Running);
                }
                break;
            }
            case Stop stop:
            {
                try
                {
                    _actor.HandleStop();

                    stop.TaskCompletionSource.TrySetResult(true);
                }
                catch (Exception ex)
                {
                    stop.TaskCompletionSource.TrySetException(ex);
                }
                finally
                {
                    ExchangeState(ActorRefState.Closed);

                    _mailbox.Close();

                    _parent.Mailbox.TryAddSystemMessage(new ChildStopped(this));
                    _parent.Dispatcher.TryExecuteMailbox(_parent.Mailbox);
                }
                break;
            }
            case ChildStopped childStopped:
            {
                IActorContext child = childStopped.Child;
                try
                {
                    bool removeChild = _actor.HandleChildStopped(child);
                    if (!removeChild)
                    {
                        return;
                    }
                }
                catch
                {
                }

                _children.Remove(child);
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

    public bool Equals(IActorContext? other)
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

    private bool CanStart()
    {
        if (this is ActorRoot)
        {
            return true;
        }

        return _parent.State != ActorRefState.Closed;
    }

    private Task EnqueueStartMessage()
    {
        ActorRefState oldState = CompareExchangeState(
            ActorRefState.Starting,
            ActorRefState.Created);
        if (oldState != ActorRefState.Created)
        {
            return Task.CompletedTask;
        }

        Start start = Message.System.Start.Create();
        _mailbox.TryAddSystemMessage(start);
        _dispatcher.TryExecuteMailbox(_mailbox);
        return start.Task;
    }

    private Task StopChildren(bool waitChildren)
    {
        if (_children.Count <= 0)
        {
            return Task.CompletedTask;
        }

        Task task = Task.WhenAll(_children.Select(child => child.StopAsync(waitChildren)));
        return waitChildren ? task : Task.CompletedTask;
    }
}
