namespace Soil.SimpleActorModel.Dispatchers;

public class DefaultDispatcherProvider : IDispatcherProvider
{
    private readonly DispatcherProps _props;

    private readonly DefaultDispatcherFactory _factory = new();

    public DispatcherProps Props
    {
        get
        {
            return _props;
        }
    }

    public DefaultDispatcherProvider(DispatcherProps props)
    {
        _props = props;
    }

    public IDispatcher Provide()
    {
        return _factory.Create(_props);
    }
}
