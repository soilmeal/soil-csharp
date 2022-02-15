using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Message.System;

public class ChildStopped : SystemMessage
{
    private readonly IActorContext _child;

    public IActorContext Child
    {
        get
        {
            return _child;
        }
    }

    public ChildStopped(IActorContext child)
    {
        _child = child;
    }
}
