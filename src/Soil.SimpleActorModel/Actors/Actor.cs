using System;

namespace Soil.SimpleActorModel.Actors;

public abstract class Actor : AbstractActor
{
    public override void HandleReceive(object message)
    {
        OnReceive(message);
    }

    protected abstract void OnReceive(object message);
}
