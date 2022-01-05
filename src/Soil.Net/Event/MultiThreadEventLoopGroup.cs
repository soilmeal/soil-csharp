using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Soil.Threading;
using Soil.Threading.Atomic;

namespace Soil.Net.Event;

public class MultiThreadEventLoopGroup : IEventLoopGroup
{
    private readonly int _maximumConcurrencyLevel;

    private readonly AtomicInt32 _inc = new(-1);

    private readonly SingleThreadEventLoop[] _eventLoops;

    private readonly int _eventLoopsLen;

    private readonly bool _initSynchronizationContextAlways;

    public int MaximumConcurrencyLevel
    {
        get
        {
            return _maximumConcurrencyLevel;
        }
    }

    public bool InitSynchronizationContextAlways
    {
        get
        {
            return _initSynchronizationContextAlways;
        }
    }

    public bool IsInEventLoop
    {
        get
        {
            return _eventLoops.Any(eventLoop => eventLoop.IsInEventLoop);
        }
    }

    public MultiThreadEventLoopGroup(IThreadFactory threadFactory)
        : this(Environment.ProcessorCount, threadFactory)
    {
    }

    public MultiThreadEventLoopGroup(int maxConcurrencyLevel, IThreadFactory threadFactory)
        : this(maxConcurrencyLevel, threadFactory, false)
    {

    }

    public MultiThreadEventLoopGroup(
        int maxConcurrencyLevel,
        IThreadFactory threadFactory,
        bool initSynchronizationContextAlways)
    {
        if (maxConcurrencyLevel <= 0 || maxConcurrencyLevel >= int.MaxValue)
        {
            maxConcurrencyLevel = 1;
        }

        _maximumConcurrencyLevel = maxConcurrencyLevel;

        _eventLoops = Enumerable.Range(0, maxConcurrencyLevel)
            .Select((_) => new SingleThreadEventLoop(
                threadFactory, initSynchronizationContextAlways))
            .ToArray();

        _initSynchronizationContextAlways = initSynchronizationContextAlways;

        _eventLoopsLen = _eventLoops.Length;
    }

    public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction);
    }

    public Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction, cancellationToken);
    }

    public Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction, continuationOptions);
    }

    public Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction, cancellationToken, continuationOptions);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc, cancellationToken);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc, continuationOptions);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc, cancellationToken, continuationOptions);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction, cancellationToken);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction, continuationOptions);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationAction, cancellationToken, continuationOptions);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc, cancellationToken);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc, continuationOptions);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAll(tasks, continuationFunc, cancellationToken, continuationOptions);
    }

    public Task ContinueWhenAny(Task[] tasks, Action<Task> continuationAction)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction);
    }

    public Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction, cancellationToken);
    }

    public Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction, continuationOptions);
    }

    public Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction, cancellationToken, continuationOptions);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc, cancellationToken);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc, continuationOptions);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc, cancellationToken, continuationOptions);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction, cancellationToken);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction, continuationOptions);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationAction, cancellationToken, continuationOptions);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc, cancellationToken);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc, continuationOptions);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return Next()
            .ContinueWhenAny(tasks, continuationFunc, cancellationToken, continuationOptions);
    }

    public Task FromAsync(
        IAsyncResult asyncResult,
        Action<IAsyncResult> endMethod,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(asyncResult, endMethod, creationOptions);
    }

    public Task FromAsync(IAsyncResult asyncResult, Action<IAsyncResult> endMethod)
    {
        return Next()
            .FromAsync(asyncResult, endMethod);
    }

    public Task<TResult> FromAsync<TResult>(
        IAsyncResult asyncResult,
        Func<IAsyncResult, TResult> endMethod)
    {
        return Next()
            .FromAsync(asyncResult, endMethod);
    }

    public Task<TResult> FromAsync<TResult>(
        IAsyncResult asyncResult,
        Func<IAsyncResult, TResult> endMethod,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(asyncResult, endMethod);
    }

    public Task FromAsync(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, state);
    }

    public Task FromAsync(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, state, creationOptions);
    }

    public Task<TResult> FromAsync<TResult>(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, state);
    }

    public Task<TResult> FromAsync<TResult>(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, state, creationOptions);
    }

    public Task FromAsync<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, state);
    }

    public Task FromAsync<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, state, creationOptions);
    }

    public Task<TResult> FromAsync<TArg1, TResult>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, state);
    }

    public Task<TResult> FromAsync<TArg1, TResult>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, state, creationOptions);
    }

    public Task FromAsync<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, state);
    }

    public Task FromAsync<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, state, creationOptions);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TResult>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, state);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TResult>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, state, creationOptions);
    }

    public Task FromAsync<TArg1, TArg2, TArg3>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, arg3, state);
    }

    public Task FromAsync<TArg1, TArg2, TArg3>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, arg3, state, creationOptions);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, arg3, state);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .FromAsync(beginMethod, endMethod, arg1, arg2, arg3, state, creationOptions);
    }

    public IEventLoop Next()
    {
        return _eventLoops[_inc.Increment() % _eventLoopsLen];
    }

    public Task StartNew(Action action)
    {
        return Next()
            .StartNew(action);
    }

    public Task StartNew(Action action, CancellationToken cancellationToken)
    {
        return Next()
            .StartNew(action, cancellationToken);
    }

    public Task StartNew(Action action, TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(action, creationOptions);
    }

    public Task StartNew(
        Action action,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(action, cancellationToken, creationOptions);
    }

    public Task StartNew(Action<object?> action, object? state)
    {
        return Next()
            .StartNew(action, state);
    }

    public Task StartNew(Action<object?> action, object? state, CancellationToken cancellationToken)
    {
        return Next()
            .StartNew(action, state, cancellationToken);
    }

    public Task StartNew(Action<object?> action, object? state, TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(action, state, creationOptions);
    }

    public Task StartNew(
        Action<object?> action,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(action, state, cancellationToken, creationOptions);
    }

    public Task<TResult> StartNew<TResult>(Func<TResult> func)
    {
        return Next()
            .StartNew(func);
    }

    public Task<TResult> StartNew<TResult>(Func<TResult> func, CancellationToken cancellationToken)
    {
        return Next()
            .StartNew(func, cancellationToken);
    }

    public Task<TResult> StartNew<TResult>(Func<TResult> func, TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(func, creationOptions);
    }

    public Task<TResult> StartNew<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(func, cancellationToken, creationOptions);
    }

    public Task<TResult> StartNew<TResult>(Func<object?, TResult> func, object? state)
    {
        return Next()
            .StartNew(func, state);
    }

    public Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken)
    {
        return Next()
            .StartNew(func, state, cancellationToken);
    }

    public Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(func, state, creationOptions);
    }

    public Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return Next()
            .StartNew(func, state, cancellationToken, creationOptions);
    }
}
