using System;

namespace Soil.SimpleActorModel.Actors;

public abstract class Actor : AbstractActor
{
    public override void HandleReceive(object message)
    {
        throw new NotImplementedException();
    }

    protected abstract void OnReceive(object message);
}
