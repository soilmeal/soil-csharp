using System;
using System.Threading.Tasks;

namespace Soil.SimpleActorModel.Actors;

public interface IActorRef : IEquatable<IActorRef>
{
    public static readonly IActorRef None = new NoneActorRef();

    public static readonly IActorRef NoSender = new NoSenderActorRef();

    void Start();

    void Stop();

    void Stop(int millisecondsTimeout);

    void Stop(TimeSpan timeout);

    bool Send(object message);

    bool Send(object message, IActorRef sender);

    Task<bool> SendAsync(object message);

    Task<bool> SendAsync(object message, IActorRef sender);

    private class NoneActorRef : IActorRef, IEquatable<NoneActorRef>
    {
        public void Start()
        {
            throw new NotSupportedException();
        }

        public void Stop()
        {
            throw new NotSupportedException();
        }

        public void Stop(int millisecondsTimeout)
        {
            throw new NotSupportedException();
        }

        public void Stop(TimeSpan timeout)
        {
            throw new NotSupportedException();
        }

        public bool Send(object message)
        {
            throw new NotSupportedException();
        }

        public bool Send(object message, IActorRef sender)
        {
            throw new NotSupportedException();
        }

        public Task<bool> SendAsync(object message)
        {
            throw new NotSupportedException();
        }

        public Task<bool> SendAsync(object message, IActorRef sender)
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
        public void Start()
        {
            throw new NotSupportedException();
        }

        public void Stop()
        {
            throw new NotSupportedException();
        }

        public void Stop(int millisecondsTimeout)
        {
            throw new NotSupportedException();
        }

        public void Stop(TimeSpan timeout)
        {
            throw new NotSupportedException();
        }

        public bool Send(object message)
        {
            throw new NotSupportedException();
        }

        public bool Send(object message, IActorRef sender)
        {
            throw new NotSupportedException();
        }

        public Task<bool> SendAsync(object message)
        {
            throw new NotSupportedException();
        }

        public Task<bool> SendAsync(object message, IActorRef sender)
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
