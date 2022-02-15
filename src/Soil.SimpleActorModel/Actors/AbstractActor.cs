namespace Soil.SimpleActorModel.Actors;

public abstract class AbstractActor
{
    private IActorContext _context;

    public IActorContext Context
    {
        get
        {
            return _context;
        }
        internal set
        {
            _context = value;
        }
    }

    protected AbstractActor()
    {
        _context = ActorContexts.None;
    }

    public virtual void HandleCreate()
    {
        OnCreate();
    }

    public virtual void HandleStart()
    {
        OnStart();
    }

    public virtual void HandleStop()
    {
        OnStop();
    }

    public virtual bool HandleChildStopped(IActorRef child)
    {
        return OnChildStopped(child);
    }

    public abstract void HandleReceive(object? message);

    protected virtual void OnCreate()
    {
    }

    protected virtual void OnStart()
    {
    }

    protected virtual void OnStop()
    {
    }

    protected virtual bool OnChildStopped(IActorRef child)
    {
        return true;
    }
}
