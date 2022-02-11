using System;

namespace Soil.SimpleActorModel.Actors;

public static class Actors
{
    public static readonly AbstractActor None = new NoneActor();

    private class NoneActor : AbstractActor
    {
        public override void HandleReceive(object message)
        {
            throw new NotSupportedException();
        }
    }
}
