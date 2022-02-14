using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public static class ActorRefs
{
    public static readonly IActorRef None = new NoneActorRef();

    public static readonly IActorRef NoSender = new NoSenderActorRef();

    private class NoneActorRef : IActorRef, IEquatable<NoneActorRef>
    {
        public ActorRefState State
        {
            get
            {
                throw new NotSupportedException();
            }
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
            throw new NotSupportedException();
        }

        public void Stop(bool waitChildren)
        {
            throw new NotSupportedException();
        }

        public Task StopAsync(bool waitChildren)
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

        public override bool Equals(object? other)
        {
            return other is NoneActorRef none && Equals(none);
        }

        public bool Equals(IActorRef? other)
        {
            return other is NoneActorRef none && Equals(none);
        }

        public bool Equals(NoneActorRef? other)
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

    private class NoSenderActorRef : IActorRef, IEquatable<NoSenderActorRef>
    {
        public ActorRefState State
        {
            get
            {
                throw new NotSupportedException();
            }
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
            throw new NotSupportedException();
        }

        public void Stop(bool waitChildren)
        {
            throw new NotSupportedException();
        }

        public Task StopAsync(bool waitChildren)
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

        public override bool Equals(object? other)
        {
            return other is NoSenderActorRef noSender && Equals(noSender);
        }

        public bool Equals(IActorRef? other)
        {
            return other is NoSenderActorRef noSender && Equals(noSender);
        }

        public bool Equals(NoSenderActorRef? other)
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
}
