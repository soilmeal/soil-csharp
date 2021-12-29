using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace Soil.Core.Threading.Tasks;

public class TaskSynchronizationContext : SynchronizationContext
{
    private readonly ILogger<TaskSynchronizationContext> _logger;

    private readonly TaskFactory _taskFactory;

    public TaskSynchronizationContext(TaskFactory taskFactory, ILoggerFactory loggerFactory)
        : this(taskFactory, loggerFactory.CreateLogger<TaskSynchronizationContext>())
    {
    }

    private TaskSynchronizationContext(
        TaskFactory taskFactory,
        ILogger<TaskSynchronizationContext> logger)
    {
        _logger = logger;

        _taskFactory = taskFactory;
    }

    public override void Post(SendOrPostCallback d, object? state)
    {
        _taskFactory.StartNew(d.Invoke, state);
    }

    public override void Send(SendOrPostCallback d, object? state)
    {
        SynchronizationContext? current = Current;
        if (this == current)
        {
            d.Invoke(state);
            return;
        }

        Task task = _taskFactory.StartNew(d.Invoke, state);
        task.GetAwaiter().GetResult();
    }

    public override SynchronizationContext CreateCopy()
    {
        return new TaskSynchronizationContext(_taskFactory, _logger);
    }
}
