using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Soil.SimpleActorModel.Dispatcher;
using Soil.SimpleActorModel.Message;
using Soil.SimpleActorModel.Message.System;

namespace Soil.SimpleActorModel.Actors;

public static class ActorContexts
{
    public static readonly IActorContext None = new NoneActorContext();

    private class NoneActorContext : IActorContext, IEquatable<NoneActorContext>
    {
        public ActorRefState State
        {
            get
            {
                return ActorRefState.Closed;
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
                return ActorRefs.None;
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
                return Enumerable.Empty<IActorRef>();
            }
        }

        public IDispatcher Dispatcher
        {
            get
            {
                return Dispatchers.None;
            }
        }

        public Mailbox Mailbox
        {
            get
            {
                return Mailboxes.None;
            }
        }

        public AbstractActor Actor()
        {
            return Actors.None;
        }

        public T? Actor<T>()
            where T : AbstractActor
        {
            return Actors.None as T;
        }

        public bool CanReceiveMessage()
        {
            return false;
        }

        public IActorContext Create(ActorProps props)
        {
            return this;
        }

        public void Invoke(Envelope envelope)
        {
        }

        public void InvokeSystem(SystemMessage message)
        {
        }

        public void Tell(object? message)
        {
        }

        public void Tell(object? message, IActorRef sender)
        {
        }

        public Task<object> Ask(object? message)
        {
            return Task.FromResult<object>(null!);
        }

        public Task<T> Ask<T>(object? message)
        {
            return Task.FromResult<T>(default!);
        }

        public void Start()
        {
        }

        public Task StartAsync()
        {
            return Task.CompletedTask;
        }

        public void Stop(bool waitChildren)
        {
        }

        public Task StopAsync(bool waitChildren)
        {
            return Task.CompletedTask;
        }

        public bool Equals(IActorRef? other)
        {
            return other is NoneActorContext context && Equals(context);
        }

        public bool Equals(IActorContext? other)
        {
            return other is NoneActorContext context && Equals(context);
        }

        public bool Equals(NoneActorContext? other)
        {
            return ReferenceEquals(this, other);
        }
    }
}
