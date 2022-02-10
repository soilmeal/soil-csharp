namespace Soil.SimpleActorModel.Actors;

public abstract class AbstractActor
{
    public static readonly Receiver EmptyReceiver = (_) => false;

    public Receiver Receiver
    {
        get
        {
            return Receive;
        }
    }

    public abstract IActorContext Context { get; }

    protected AbstractActor() { }

    public virtual void PreStart()
    {
    }

    public virtual void PostStart()
    {
    }

    public virtual void PreStop()
    {
    }

    public virtual void PostStop()
    {
    }

    protected abstract bool Receive(object message);
}
