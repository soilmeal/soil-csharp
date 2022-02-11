namespace Soil.SimpleActorModel.Dispatchers;

public class DefaultDispatcherFactory : IDispatcherFactory
{
    public IDispatcher Create(DispatcherProps props)
    {
        switch (props.Type)
        {
            case DefaultDispatcherType.TaskSchedulerDispatcherType:
            {
                return new TaskSchedulerDispatcher(props.ThroughputPerActor);
            }
            case DefaultDispatcherType.CurrentThreadDispatcherType:
            {
                return new CurrentThreadDispatcher();
            }
            default:
            {
                return Dispatchers.None;
            }
        }
    }
}
