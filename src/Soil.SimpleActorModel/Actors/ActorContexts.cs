using System;
using System.Collections.Generic;
using Soil.SimpleActorModel.Dispatchers;
using Soil.SimpleActorModel.Messages;
using Soil.SimpleActorModel.Messages.System;

namespace Soil.SimpleActorModel.Actors;

public static class ActorContexts
{
    public static readonly IActorContext None = new NoneActorContext();

    private class NoneActorContext : IActorContext
    {
        public IActorRef Parent
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public IActorRef Self
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public IActorRef Sender
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public IEnumerable<IActorRef> Children
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public IDispatcher Dispatcher
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public IActorRef Create(ActorProps props)
        {
            throw new NotSupportedException();
        }

        public void Invoke(Envelope envelope)
        {
            throw new NotSupportedException();
        }

        public void InvokeSystem(SystemMessage message)
        {
            throw new NotSupportedException();
        }

        public void Send(object message)
        {
            throw new NotSupportedException();
        }

        public void Send(object message, IActorRef sender)
        {
            throw new NotSupportedException();
        }

        public void Start()
        {
            throw new NotSupportedException();
        }

        public void Stop()
        {
            throw new NotSupportedException();
        }

        public bool Equals(IActorRef? other)
        {
            throw new NotSupportedException();
        }
    }
}
