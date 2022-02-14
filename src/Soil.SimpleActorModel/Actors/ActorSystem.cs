using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;
using Soil.SimpleActorModel.Message.System;

namespace Soil.SimpleActorModel.Actors;

public class ActorSystem : IActorContext, IActorRef, IEquatable<ActorSystem>
{
    private readonly IDispatcherFactory _dispatcherFactory;

    private readonly IMailboxFactory _mailboxFactory;

    private readonly ConcurrentDictionary<string, IDispatcher> _dispatchers = new();

    private readonly ActorRoot _actorRoot;

    public ActorRefState State
    {
        get
        {
            return _actorRoot.State;
        }
    }

    public IActorRef Parent
    {
        get
        {
            return ActorRefs.None;
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
            return ActorRefs.NoSender;
        }
    }

    public IEnumerable<IActorRef> Children
    {
        get
        {
            return _actorRoot.Children;
        }
    }

    public IDispatcher Dispatcher
    {
        get
        {
            return _actorRoot.Dispatcher;
        }
    }

    private ActorSystem(
        IDispatcherFactory dispatcherFactory,
        IMailboxFactory mailboxFactory)
    {
        _dispatcherFactory = dispatcherFactory;
        _mailboxFactory = mailboxFactory;
        _actorRoot = new ActorRoot(this);
    }

    public IDispatcher GetOrCreateDispatcher(DispatcherProps props)
    {
        return _dispatchers.GetOrAdd(props.Id, (_) => _dispatcherFactory.Create(props));
    }

    public Mailbox CreateMailbox(IActorContext context, MailboxProps props)
    {
        return _mailboxFactory.Create(context, props);
    }

    public AbstractActor GetActor()
    {
        throw new NotSupportedException();
    }

    public T GetActor<T>()
        where T : AbstractActor
    {
        throw new NotSupportedException();
    }

    public bool CanReceiveMessage()
    {
        throw new NotSupportedException();
    }

    public void Start()
    {
        _actorRoot.Start();
    }

    public Task StartAsync()
    {
        return _actorRoot.StartAsync();
    }

    public void Stop(bool waitChildren)
    {
        _actorRoot.Stop(waitChildren);
    }

    public Task StopAsync(bool waitChildren)
    {
        return _actorRoot.StopAsync(waitChildren);
    }

    public void Send(object message)
    {
        throw new NotSupportedException();
    }

    public void Send(object message, IActorRef sender)
    {
        throw new NotSupportedException();
    }

    public IActorRef Create(ActorProps props)
    {
        return _actorRoot.Create(props);
    }

    public void Invoke(Envelope envelope)
    {
        throw new NotSupportedException();
    }

    public void InvokeSystem(SystemMessage message)
    {
        throw new NotSupportedException();
    }

    public bool Equals(ActorSystem? other)
    {
        return ReferenceEquals(this, other);
    }

    public bool Equals(IActorRef? other)
    {
        return other is ActorSystem system && Equals(system);
    }

    public override bool Equals(object? obj)
    {
        return obj is ActorSystem system && Equals(system);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string? ToString()
    {
        return base.ToString();
    }

    public class Builder
    {
        private IDispatcherFactory? _dispatcherFactory;

        private IMailboxFactory? _mailboxFactory;

        public IDispatcherFactory? DispatcherFactory
        {
            get
            {
                return _dispatcherFactory;
            }
        }

        public IMailboxFactory? MailboxFactory
        {
            get
            {
                return _mailboxFactory;
            }
        }

        public Builder()
        {
        }

        public Builder SetDispatcherFactory(IDispatcherFactory dispatcherFactory)
        {
            _dispatcherFactory = dispatcherFactory ?? throw new ArgumentNullException(nameof(dispatcherFactory));

            return this;
        }

        public Builder SetMailboxFactory(IMailboxFactory mailboxFactory)
        {
            _mailboxFactory = mailboxFactory ?? throw new ArgumentNullException(nameof(mailboxFactory));

            return this;
        }

        public ActorSystem Build()
        {
            return new ActorSystem(
                GetOrDefaultDispatcherFactory(),
                GetOrDefaultMailboxFactory());
        }

        private IDispatcherFactory GetOrDefaultDispatcherFactory()
        {
            return _dispatcherFactory ?? new DefaultDispatcherFactory();
        }

        private IMailboxFactory GetOrDefaultMailboxFactory()
        {
            return _mailboxFactory ?? new DefaultMailboxFactory();
        }
    }
}
