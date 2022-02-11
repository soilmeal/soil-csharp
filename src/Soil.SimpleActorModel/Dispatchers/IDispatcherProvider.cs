namespace Soil.SimpleActorModel.Dispatchers;

public interface IDispatcherProvider
{
    DispatcherProps Props { get; }

    IDispatcher Provide();
}
