using System;

namespace Soil.SimpleActorModel.Dispatcher;

public class DefaultDispatcherFactory : IDispatcherFactory
{
    public IDispatcher Create(DispatcherProps props)
    {
        if (props == null)
        {
            throw new ArgumentNullException(nameof(props));
        }

        switch (props.Type)
        {
            case DispatcherType.TaskScheduler:
            {
                return new TaskSchedulerDispatcher(props.Id, props.ThroughputPerActor);
            }
            case DispatcherType.CurrentThread:
            {
                return new CurrentThreadDispatcher(props.Id);
            }
            default:
            {
                throw new ArgumentException("invalid dispatcher type", props.Type);
            }
        }
    }
}
