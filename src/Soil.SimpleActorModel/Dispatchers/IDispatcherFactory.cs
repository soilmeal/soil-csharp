using Soil.SimpleActorModel.Actors;

namespace Soil.SimpleActorModel.Dispatchers;

public interface IDispatcherFactory
{
    IDispatcher Create(DispatcherProps props);
}
