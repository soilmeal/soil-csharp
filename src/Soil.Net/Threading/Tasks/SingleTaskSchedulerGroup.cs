using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soil.Core.Threading;
using TaskScheduler = Soil.Core.Threading.Tasks.TaskScheduler;

namespace Soil.Net.Threading.Tasks;

public class SingleTaskSchedulerGroup : ITaskSchedulerGroup
{
    private readonly TaskScheduler _taskScheduler;

    public SingleTaskSchedulerGroup()
        : this(ThreadFactoryBuilder.BuildDefault())
    {
    }

    public SingleTaskSchedulerGroup(IThreadFactory threadFactory)
    {
        _taskScheduler = new TaskScheduler.Builder()
            .SetMaximumConcurrencyLevel(1)
            .SetQueue(new ConcurrentQueue<Task>())
            .SetThreadFactory(threadFactory)
            .Build();
    }

    public TaskScheduler NextScheduler()
    {
        return _taskScheduler;
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<object?, TResult> func, object? state)
    {
        var task = new Task<TResult>(func, state);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<object?, TResult> func, object? state, CancellationToken cancellationToken)
    {
        var task = new Task<TResult>(func, state, cancellationToken);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<object?, TResult> func, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
    {
        var task = new Task<TResult>(func, state, cancellationToken, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<object?, TResult> func, object? state, TaskCreationOptions creationOptions)
    {
        var task = new Task<TResult>(func, state, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<TResult> func)
    {
        var task = new Task<TResult>(func);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<TResult> func, CancellationToken cancellationToken)
    {
        var task = new Task<TResult>(func, cancellationToken);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<TResult> func, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
    {
        var task = new Task<TResult>(func, cancellationToken, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartNewOnNextScheduler<TResult>(Func<TResult> func, TaskCreationOptions creationOptions)
    {
        var task = new Task<TResult>(func, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action<object?> action, object? state)
    {
        var task = new Task(action, state);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action<object?> action, object? state, CancellationToken cancellationToken)
    {
        var task = new Task(action, state, cancellationToken);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action<object?> action, object? state, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
    {
        var task = new Task(action, state, cancellationToken, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action<object?> action, object? state, TaskCreationOptions creationOptions)
    {
        var task = new Task(action, state, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action action)
    {
        var task = new Task(action);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action action, CancellationToken cancellationToken)
    {
        var task = new Task(action, cancellationToken);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action action, CancellationToken cancellationToken, TaskCreationOptions creationOptions)
    {
        var task = new Task(action, cancellationToken, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task StartNewOnNextScheduler(Action action, TaskCreationOptions creationOptions)
    {
        var task = new Task(action, creationOptions);
        return StartOnNextScheduler(task);
    }

    public Task<TResult> StartOnNextScheduler<TResult>(Task<TResult> task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        task.Start(NextScheduler());

        return task;
    }

    public Task StartOnNextScheduler(Task task)
    {
        if (task == null)
        {
            throw new ArgumentNullException(nameof(task));
        }

        task.Start(NextScheduler());

        return task;
    }
}
