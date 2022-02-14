namespace Soil.SimpleActorModel.Actors;

public static class Actors
{
    public static readonly IActorFactory NoneFactory = new NoneActorFactory();

    public static readonly AbstractActor None = new NoneActor();

    private class NoneActorFactory : IActorFactory
    {
        public AbstractActor Create()
        {
            return None;
        }
    }

    private class NoneActor : AbstractActor
    {
        public override void HandleReceive(object message)
        {
        }
    }
}
