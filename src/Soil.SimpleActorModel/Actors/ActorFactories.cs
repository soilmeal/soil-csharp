namespace Soil.SimpleActorModel.Actors;

public class ActorFactories
{
    public static readonly IActorFactory None = new NoneActorFactory();

    private class NoneActorFactory : IActorFactory
    {
        public AbstractActor Create()
        {
            return Actors.None;
        }
    }
}
