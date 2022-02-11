using System;

namespace Soil.SimpleActorModel.Actors;

public interface IActorFactory
{
    AbstractActor Create();

    internal static IActorFactory CreateFactory(Func<AbstractActor> actorFactoryFunc)
    {
        return new FuncActorFactory(actorFactoryFunc);
    }

    private class FuncActorFactory : IActorFactory
    {
        private readonly Func<AbstractActor> _factoryFunc;

        public FuncActorFactory(Func<AbstractActor> factoryFunc)
        {
            _factoryFunc = factoryFunc;
        }

        public AbstractActor Create()
        {
            return _factoryFunc();
        }
    }
}
