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
                return ActorRefState.Closed;
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

        public void Tell(object? message)
        {
        }

        public void Tell(object? message, IActorRef sender)
        {
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
                return ActorRefState.Closed;
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

        public void Tell(object? message)
        {
        }

        public void Tell(object? message, IActorRef sender)
        {
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
