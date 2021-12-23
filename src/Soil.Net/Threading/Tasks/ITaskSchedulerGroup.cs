using System;
using System.Diagnostics.CodeAnalysis;
using System.Threading;
using System.Threading.Tasks;
using TaskScheduler = Soil.Core.Threading.Tasks.TaskScheduler;

namespace Soil.Net.Threading.Tasks;

[SuppressMessage("Design", "CA1068: CancellationToken parameters must come last", Justification = "Adjust to constructor signatures of Task")]
public interface ITaskSchedulerGroup
{
    TaskScheduler NextScheduler();

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<object?, TResult> func,
        object? state);

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken);

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<object?, TResult> func,
        object? state,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNewOnNextScheduler<TResult>(Func<TResult> func);

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken);

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNewOnNextScheduler<TResult>(
        Func<TResult> func,
        TaskCreationOptions creationOptions);

    Task StartNewOnNextScheduler(Action<object?> action, object? state);

    Task StartNewOnNextScheduler(
        Action<object?> action,
        object? state,
        CancellationToken cancellationToken);

    Task StartNewOnNextScheduler(
        Action<object?> action,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task StartNewOnNextScheduler(
        Action<object?> action,
        object? state,
        TaskCreationOptions creationOptions);

    Task StartNewOnNextScheduler(Action action);

    Task StartNewOnNextScheduler(Action action, CancellationToken cancellationToken);

    Task StartNewOnNextScheduler(
        Action action,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task StartNewOnNextScheduler(Action action, TaskCreationOptions creationOptions);

    Task<TResult> StartOnNextScheduler<TResult>(Task<TResult> task);

    Task StartOnNextScheduler(Task task);
}
