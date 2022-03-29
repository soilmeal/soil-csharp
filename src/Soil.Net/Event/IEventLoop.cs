using System;
using System.Threading;
using System.Threading.Tasks;

namespace Soil.Net.Event;

public interface IEventLoop
{
    bool InitSynchronizationContextAlways { get; }

    bool IsInEventLoop { get; }

    Task StartNew(Action action);

    Task StartNew(
        Action action,
        CancellationToken cancellationToken);

    Task StartNew(
        Action action,
        TaskCreationOptions creationOptions);

    Task StartNew(
        Action action,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task StartNew(Action<object?> action, object? state);

    Task StartNew(
        Action<object?> action,
        object? state,
        CancellationToken cancellationToken);

    Task StartNew(
        Action<object?> action,
        object? state,
        TaskCreationOptions creationOptions);

    Task StartNew(
        Action<object?> action,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNew<TResult>(Func<TResult> func);

    Task<TResult> StartNew<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken);

    Task<TResult> StartNew<TResult>(
        Func<TResult> func,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNew<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state);

    Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken);

    Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        TaskCreationOptions creationOptions);

    Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions);

    Task FromAsync(
        IAsyncResult asyncResult,
        Action<IAsyncResult> endMethod);

    Task FromAsync(
        IAsyncResult asyncResult,
        Action<IAsyncResult> endMethod,
        TaskCreationOptions creationOptions);

    Task<TResult> FromAsync<TResult>(
        IAsyncResult asyncResult,
        Func<IAsyncResult, TResult> endMethod);

    Task<TResult> FromAsync<TResult>(
        IAsyncResult asyncResult,
        Func<IAsyncResult, TResult> endMethod,
        TaskCreationOptions creationOptions);

    Task FromAsync(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        object? state);

    Task FromAsync(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        object? state,
        TaskCreationOptions creationOptions);

    Task<TResult> FromAsync<TResult>(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        object? state);

    Task<TResult> FromAsync<TResult>(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        object? state,
        TaskCreationOptions creationOptions);

    Task FromAsync<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        object? state);

    Task FromAsync<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        object? state,
        TaskCreationOptions creationOptions);

    Task<TResult> FromAsync<TArg1, TResult>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        object? state);

    Task<TResult> FromAsync<TArg1, TResult>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        object? state,
        TaskCreationOptions creationOptions);

    Task FromAsync<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state);

    Task FromAsync<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state,
        TaskCreationOptions creationOptions);

    Task<TResult> FromAsync<TArg1, TArg2, TResult>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state);

    Task<TResult> FromAsync<TArg1, TArg2, TResult>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state,
        TaskCreationOptions creationOptions);

    Task FromAsync<TArg1, TArg2, TArg3>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state);

    Task FromAsync<TArg1, TArg2, TArg3>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state,
        TaskCreationOptions creationOptions);

    Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state);

    Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state,
        TaskCreationOptions creationOptions);

    Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction);

    Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        CancellationToken cancellationToken);

    Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc);

    Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        CancellationToken cancellationToken);

    Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction);

    Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        CancellationToken cancellationToken);

    Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc);

    Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        CancellationToken cancellationToken);

    Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction);

    Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        CancellationToken cancellationToken);

    Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc);

    Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        CancellationToken cancellationToken);

    Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction);

    Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        CancellationToken cancellationToken);

    Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        TaskContinuationOptions continuationOptions);

    Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc);

    Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        CancellationToken cancellationToken);

    Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        TaskContinuationOptions continuationOptions);

    Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions);
}
