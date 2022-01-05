using System;
using System.Collections.Concurrent;
using System.Threading;
using System.Threading.Tasks;
using Soil.Threading;
using Soil.Threading.Tasks;

namespace Soil.Net.Event;

public class SingleThreadEventLoop : IEventLoop
{
    private readonly AbstractTaskScheduler _taskScheduler;

    private readonly TaskFactory _taskFactory;

    private readonly SynchronizationContext _synchronizationContext;

    private readonly bool _initSynchronizationContextAlways;

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
            return _taskScheduler.HasThread(Environment.CurrentManagedThreadId);
        }
    }

    public SingleThreadEventLoop(IThreadFactory threadFactory)
        : this(threadFactory, false)
    {
    }

    public SingleThreadEventLoop(
        IThreadFactory threadFactory,
        bool initSynchronizationContextAlways)
    {
        _taskScheduler = new AbstractTaskScheduler.Builder()
            .SetMaximumConcurrencyLevel(1)
            .SetThreadFactory(threadFactory)
            .SetQueue(new BlockingCollection<Task>(new ConcurrentQueue<Task>()))
            .Build();
        _taskFactory = new TaskFactory(_taskScheduler);
        _synchronizationContext = new TaskSynchronizationContext(_taskFactory);

        _initSynchronizationContextAlways = initSynchronizationContextAlways;

        _taskFactory.StartNew(SetSynchronizationContext);
    }

    public Task ContinueWhenAll(Task[] tasks, Action<Task[]> continuationAction)
    {
        return _taskFactory.ContinueWhenAll(tasks, Wrap(continuationAction));
    }

    public Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationAction),
            cancellationToken);
    }

    public Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationAction),
            continuationOptions);
    }

    public Task ContinueWhenAll(
        Task[] tasks,
        Action<Task[]> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationAction),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc));
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc),
            cancellationToken);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc),
            continuationOptions);
    }

    public Task<TResult> ContinueWhenAll<TResult>(
        Task[] tasks,
        Func<Task[], TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction)
    {
        return _taskFactory.ContinueWhenAll(tasks, Wrap(continuationAction));
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationAction),
            cancellationToken);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationAction),
            continuationOptions);
    }

    public Task ContinueWhenAll<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>[]> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationAction),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc)
    {
        return _taskFactory.ContinueWhenAll(tasks, Wrap(continuationFunc));
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc),
            cancellationToken);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc),
            continuationOptions);
    }

    public Task<TResult> ContinueWhenAll<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>[], TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAll(
            tasks,
            Wrap(continuationFunc),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task ContinueWhenAny(Task[] tasks, Action<Task> continuationAction)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationAction));
    }

    public Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationAction), cancellationToken);
    }

    public Task ContinueWhenAny(
        Task[] tasks,
        Action<Task> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationAction), continuationOptions);
    }

    public Task ContinueWhenAny(Task[] tasks,
        Action<Task> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(
            tasks,
            Wrap(continuationAction),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationFunc));
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationFunc), cancellationToken);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationFunc), continuationOptions);
    }

    public Task<TResult> ContinueWhenAny<TResult>(
        Task[] tasks,
        Func<Task, TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(
            tasks,
            Wrap(continuationFunc),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationAction));
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationAction), cancellationToken);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationAction), continuationOptions);
    }

    public Task ContinueWhenAny<TAntecedentResult>(
        Task<TAntecedentResult>[] tasks,
        Action<Task<TAntecedentResult>> continuationAction,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {

        return _taskFactory.ContinueWhenAny(
            tasks,
            Wrap(continuationAction),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationFunc));
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        CancellationToken cancellationToken)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationFunc), cancellationToken);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(tasks, Wrap(continuationFunc), continuationOptions);
    }

    public Task<TResult> ContinueWhenAny<TAntecedentResult, TResult>(
        Task<TAntecedentResult>[] tasks,
        Func<Task<TAntecedentResult>, TResult> continuationFunc,
        CancellationToken cancellationToken,
        TaskContinuationOptions continuationOptions)
    {
        return _taskFactory.ContinueWhenAny(
            tasks,
            Wrap(continuationFunc),
            cancellationToken,
            continuationOptions,
            _taskScheduler);
    }

    public Task FromAsync(
        IAsyncResult asyncResult,
        Action<IAsyncResult> endMethod,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(asyncResult, Wrap(endMethod), creationOptions);
    }

    public Task FromAsync(IAsyncResult asyncResult, Action<IAsyncResult> endMethod)
    {
        return _taskFactory.FromAsync(asyncResult, Wrap(endMethod));
    }

    public Task<TResult> FromAsync<TResult>(
        IAsyncResult asyncResult,
        Func<IAsyncResult, TResult> endMethod)
    {
        return _taskFactory.FromAsync(asyncResult, Wrap(endMethod));
    }

    public Task<TResult> FromAsync<TResult>(
        IAsyncResult asyncResult,
        Func<IAsyncResult, TResult> endMethod,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(asyncResult, Wrap(endMethod), creationOptions);
    }

    public Task FromAsync(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), state);
    }

    public Task FromAsync(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), state, creationOptions);
    }

    public Task<TResult> FromAsync<TResult>(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), state);
    }

    public Task<TResult> FromAsync<TResult>(
        Func<AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), state, creationOptions);
    }

    public Task FromAsync<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), arg1, state);
    }

    public Task FromAsync<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(
            Wrap(beginMethod),
            Wrap(endMethod),
            arg1,
            state,
            creationOptions);
    }

    public Task<TResult> FromAsync<TArg1, TResult>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), arg1, state);
    }

    public Task<TResult> FromAsync<TArg1, TResult>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(
            Wrap(beginMethod),
            Wrap(endMethod),
            arg1,
            state,
            creationOptions);
    }

    public Task FromAsync<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), arg1, arg2, state);
    }

    public Task FromAsync<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(
            Wrap(beginMethod),
            Wrap(endMethod),
            arg1,
            arg2,
            state,
            creationOptions);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TResult>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), arg1, arg2, state);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TResult>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.FromAsync(
            Wrap(beginMethod),
            Wrap(endMethod),
            arg1,
            arg2,
            state,
            creationOptions);
    }

    public Task FromAsync<TArg1, TArg2, TArg3>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Action<IAsyncResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), arg1, arg2, arg3, state);
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
        return _taskFactory.FromAsync(
            Wrap(beginMethod),
            Wrap(endMethod),
            arg1,
            arg2,
            arg3,
            state,
            creationOptions);
    }

    public Task<TResult> FromAsync<TArg1, TArg2, TArg3, TResult>(
        Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> beginMethod,
        Func<IAsyncResult, TResult> endMethod,
        TArg1 arg1,
        TArg2 arg2,
        TArg3 arg3,
        object? state)
    {
        return _taskFactory.FromAsync(Wrap(beginMethod), Wrap(endMethod), arg1, arg2, arg3, state);
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
        return _taskFactory.FromAsync(
            Wrap(beginMethod),
            Wrap(endMethod),
            arg1,
            arg2,
            arg3,
            state,
            creationOptions);
    }

    public Task StartNew(Action action)
    {
        return _taskFactory.StartNew(Wrap(action));
    }

    public Task StartNew(Action action, CancellationToken cancellationToken)
    {
        return _taskFactory.StartNew(Wrap(action), cancellationToken);
    }

    public Task StartNew(Action action, TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(Wrap(action), creationOptions);
    }

    public Task StartNew(
        Action action,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(
            Wrap(action),
            cancellationToken,
            creationOptions,
            _taskScheduler);
    }

    public Task StartNew(Action<object?> action, object? state)
    {
        return _taskFactory.StartNew(Wrap(action), state);
    }

    public Task StartNew(Action<object?> action, object? state, CancellationToken cancellationToken)
    {
        return _taskFactory.StartNew(Wrap(action), state, cancellationToken);
    }

    public Task StartNew(Action<object?> action, object? state, TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(Wrap(action), state, creationOptions);
    }

    public Task StartNew(
        Action<object?> action,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(
            Wrap(action),
            state,
            cancellationToken,
            creationOptions,
            _taskScheduler);
    }

    public Task<TResult> StartNew<TResult>(Func<TResult> func)
    {
        return _taskFactory.StartNew(Wrap(func));
    }

    public Task<TResult> StartNew<TResult>(Func<TResult> func, CancellationToken cancellationToken)
    {
        return _taskFactory.StartNew(Wrap(func), cancellationToken);
    }

    public Task<TResult> StartNew<TResult>(Func<TResult> func, TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(Wrap(func), creationOptions);
    }

    public Task<TResult> StartNew<TResult>(
        Func<TResult> func,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(
            Wrap(func),
            cancellationToken,
            creationOptions,
            _taskScheduler);
    }

    public Task<TResult> StartNew<TResult>(Func<object?, TResult> func, object? state)
    {
        return _taskFactory.StartNew(Wrap(func), state);
    }

    public Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken)
    {
        return _taskFactory.StartNew(Wrap(func), state, cancellationToken);
    }

    public Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(Wrap(func), state, creationOptions);
    }

    public Task<TResult> StartNew<TResult>(
        Func<object?, TResult> func,
        object? state,
        CancellationToken cancellationToken,
        TaskCreationOptions creationOptions)
    {
        return _taskFactory.StartNew(
            Wrap(func),
            state,
            cancellationToken,
            creationOptions,
            _taskScheduler);
    }

    private void SetSynchronizationContext()
    {
        SynchronizationContext.SetSynchronizationContext(_synchronizationContext);
    }

    private Action Wrap(Action action)
    {
        if (InitSynchronizationContextAlways)
        {
            return () =>
            {
                SetSynchronizationContext();
                action();
            };
        }

        return action;
    }

    private Action<object?> Wrap(Action<object?> action)
    {
        if (InitSynchronizationContextAlways)
        {
            return (state) =>
            {
                SetSynchronizationContext();
                action(state);
            };
        }

        return action;
    }

    private Action<IAsyncResult> Wrap(Action<IAsyncResult> action)
    {
        if (InitSynchronizationContextAlways)
        {
            return (asyncResult) =>
            {
                SetSynchronizationContext();
                action(asyncResult);
            };
        }

        return action;
    }

    private Action<Task> Wrap(Action<Task> action)
    {
        if (InitSynchronizationContextAlways)
        {
            return (task) =>
            {
                SetSynchronizationContext();
                action(task);
            };
        }

        return action;
    }

    private Action<Task[]> Wrap(Action<Task[]> action)
    {
        if (InitSynchronizationContextAlways)
        {
            return (tasks) =>
            {
                SetSynchronizationContext();
                action(tasks);
            };
        }

        return action;
    }

    private Action<Task<TAntecedentResult>> Wrap<TAntecedentResult>(
        Action<Task<TAntecedentResult>> action)
    {
        if (InitSynchronizationContextAlways)
        {
            return (task) =>
            {
                SetSynchronizationContext();
                action(task);
            };
        }

        return action;
    }

    private Action<Task<TAntecedentResult>[]> Wrap<TAntecedentResult>(
        Action<Task<TAntecedentResult>[]> action)
    {
        if (InitSynchronizationContextAlways)
        {
            return (tasks) =>
            {
                SetSynchronizationContext();
                action(tasks);
            };
        }

        return action;
    }

    private Func<TResult> Wrap<TResult>(Func<TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return () =>
            {
                SetSynchronizationContext();
                return func();
            };
        }

        return func;
    }

    private Func<object?, TResult> Wrap<TResult>(Func<object?, TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (state) =>
            {
                SetSynchronizationContext();
                return func(state);
            };
        }

        return func;
    }

    private Func<IAsyncResult, TResult> Wrap<TResult>(Func<IAsyncResult, TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (asyncResult) =>
            {
                SetSynchronizationContext();
                return func(asyncResult);
            };
        }

        return func;
    }

    private Func<AsyncCallback, object?, IAsyncResult> Wrap(
        Func<AsyncCallback, object?, IAsyncResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (asyncCallback, state) =>
            {
                SetSynchronizationContext();
                return func(asyncCallback, state);
            };
        }

        return func;
    }

    private Func<TArg1, AsyncCallback, object?, IAsyncResult> Wrap<TArg1>(
        Func<TArg1, AsyncCallback, object?, IAsyncResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (arg1, asyncCallback, state) =>
            {
                SetSynchronizationContext();
                return func(arg1, asyncCallback, state);
            };
        }

        return func;
    }

    private Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> Wrap<TArg1, TArg2>(
        Func<TArg1, TArg2, AsyncCallback, object?, IAsyncResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (arg1, arg2, asyncCallback, state) =>
            {
                SetSynchronizationContext();
                return func(arg1, arg2, asyncCallback, state);
            };
        }

        return func;
    }

    private Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> Wrap<TArg1, TArg2, TArg3>(Func<TArg1, TArg2, TArg3, AsyncCallback, object?, IAsyncResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (arg1, arg2, arg3, asyncCallback, state) =>
            {
                SetSynchronizationContext();
                return func(arg1, arg2, arg3, asyncCallback, state);
            };
        }

        return func;
    }

    private Func<Task, TResult> Wrap<TResult>(Func<Task, TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (task) =>
            {
                SetSynchronizationContext();
                return func(task);
            };
        }

        return func;
    }

    private Func<Task[], TResult> Wrap<TResult>(Func<Task[], TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (tasks) =>
            {
                SetSynchronizationContext();
                return func(tasks);
            };
        }

        return func;
    }

    private Func<Task<TAntecedentResult>, TResult> Wrap<TAntecedentResult, TResult>(
        Func<Task<TAntecedentResult>, TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (task) =>
            {
                SetSynchronizationContext();
                return func(task);
            };
        }

        return func;
    }

    private Func<Task<TAntecedentResult>[], TResult> Wrap<TAntecedentResult, TResult>(
        Func<Task<TAntecedentResult>[], TResult> func)
    {
        if (InitSynchronizationContextAlways)
        {
            return (tasks) =>
            {
                SetSynchronizationContext();
                return func(tasks);
            };
        }

        return func;
    }
}
