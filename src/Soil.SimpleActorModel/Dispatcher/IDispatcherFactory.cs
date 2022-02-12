namespace Soil.SimpleActorModel.Dispatcher;

public interface IDispatcherFactory
{
    public IDispatcher Create(DispatcherProps props);
}
