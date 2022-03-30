using System;
using System.Diagnostics;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Sources;

namespace Soil.Net.Channel;

public sealed class SocketChannelAsyncEventArgs : SocketAsyncEventArgs, IValueTaskSource, IValueTaskSource<int>, IValueTaskSource<Socket>
{
    private static readonly Action<object?> _completedSentinel = (_) => throw new InvalidOperationException("should no reach here");

    private short _token;

    // private CancellationToken _cancellationToken;

    private Action<object?>? _continuation;

    private ExecutionContext? _executionContext;

    private object? _scheduler;

    public SocketChannelAsyncEventArgs()
        : base()
    {
        _token = 0;
    }

    void IValueTaskSource.GetResult(short token)
    {
        if (token != _token)
        {
            ThrowIncorrectTokenException();
        }

        SocketError error = SocketError;
        // CancellationToken cancellationToken = _cancellationToken;

        Release();

        if (error != SocketError.Success)
        {
            ThrowException(error);
        }
    }

    public ValueTaskSourceStatus GetStatus(short token)
    {
        if (token != _token)
        {
            ThrowIncorrectTokenException();
        }

        if (!ReferenceEquals(_continuation, _completedSentinel))
        {
            return ValueTaskSourceStatus.Pending;
        }

        SocketError error = SocketError;
        return error == SocketError.Success
            ? ValueTaskSourceStatus.Succeeded
            : ValueTaskSourceStatus.Faulted;
    }

    public ValueTask ConnectAsync(Socket socket)
    {
        Debug.Assert(
            Volatile.Read(ref _continuation) == null,
            "Expected null continuation to indicate reserved for use");

        try
        {
            if (socket.ConnectAsync(this))
            {
                return new ValueTask(this, _token);
            }
        }
        catch
        {
            Release();
            throw;
        }

        SocketError error = SocketError;

        Release();

        return error == SocketError.Success
            ? default
            : new ValueTask(Task.FromException(CreateException(error)));
    }

    public ValueTask<Socket> AcceptAsync(Socket socket)
    {
        Debug.Assert(
            Volatile.Read(ref _continuation) == null,
            "Expected null continuation to indicate reserved for use");

        try
        {
            if (socket.AcceptAsync(this))
            {
                return new ValueTask<Socket>(this, _token);
            }
        }
        catch
        {
            Release();
            throw;
        }

        Socket acceptSocket = AcceptSocket!;
        SocketError error = SocketError;

        AcceptSocket = null;

        Release();

        return error == SocketError.Success
            ? new ValueTask<Socket>(acceptSocket)
            : new ValueTask<Socket>(Task.FromException<Socket>(CreateException(error)));
    }

    public ValueTask<int> ReceiveAsync(Socket socket)
    {
        Debug.Assert(
            Volatile.Read(ref _continuation) == null,
            "Expected null continuation to indicate reserved for use");

        try
        {
            if (socket.ReceiveAsync(this))
            {
                return new ValueTask<int>(this, _token);
            }
        }
        catch
        {
            Release();
            throw;
        }

        int bytesTransferred = BytesTransferred;
        SocketError error = SocketError;

        Release();

        return error == SocketError.Success
            ? new ValueTask<int>(bytesTransferred)
            : new ValueTask<int>(Task.FromException<int>(CreateException(error)));
    }

    public ValueTask DisconnectAsync(Socket socket)
    {
        Debug.Assert(
            Volatile.Read(ref _continuation) == null,
            $"Expected null continuation to indicate reserved for use");

        try
        {
            if (socket.DisconnectAsync(this))
            {
                return new ValueTask(this, _token);
            }
        }
        catch
        {
            Release();
            throw;
        }

        SocketError error = SocketError;

        Release();

        return error == SocketError.Success
            ? new ValueTask()
            : new ValueTask(Task.FromException(CreateException(error)));
    }

    public ValueTask<int> SendAsync(Socket socket)
    {
        Debug.Assert(
            Volatile.Read(ref _continuation) == null,
            $"Expected null continuation to indicate reserved for use");

        try
        {
            if (socket.SendAsync(this))
            {
                return new ValueTask<int>(this, _token);
            }
        }
        catch
        {
            Release();
            throw;
        }

        int bytesTransferred = BytesTransferred;
        SocketError error = SocketError;

        Release();

        return error == SocketError.Success
            ? new ValueTask<int>(bytesTransferred)
            : new ValueTask<int>(Task.FromException<int>(CreateException(error)));
    }

    public void OnCompleted(
        Action<object> continuation,
        object? state,
        short token,
        ValueTaskSourceOnCompletedFlags flags)
    {
        if (token != _token)
        {
            ThrowIncorrectTokenException();
        }

        if ((flags & ValueTaskSourceOnCompletedFlags.FlowExecutionContext) != 0)
        {
            _executionContext = ExecutionContext.Capture();
        }

        if ((flags & ValueTaskSourceOnCompletedFlags.UseSchedulingContext) != 0)
        {
            SynchronizationContext? sc = SynchronizationContext.Current;
            if (sc != null && sc.GetType() != typeof(SynchronizationContext))
            {
                _scheduler = sc;
            }
            else
            {
                TaskScheduler ts = TaskScheduler.Current;
                if (ts != TaskScheduler.Default)
                {
                    _scheduler = ts;
                }
            }
        }

        UserToken = state; // Use UserToken to carry the continuation state around
        Action<object>? prevContinuation = Interlocked.CompareExchange(
            ref _continuation!,
            continuation,
            null);
        if (ReferenceEquals(prevContinuation, _completedSentinel))
        {
            // Lost the race condition and the operation has now already completed.
            // We need to invoke the continuation, but it must be asynchronously to
            // avoid a stack dive.  However, since all of the queueing mechanisms flow
            // ExecutionContext, and since we're still in the same context where we
            // captured it, we can just ignore the one we captured.
            bool requiresExecutionContextFlow = _executionContext != null;
            _executionContext = null;
            UserToken = null; // we have the state in "state"; no need for the one in UserToken
            InvokeContinuation(
                continuation!,
                state,
                true,
                requiresExecutionContextFlow);
        }
        else if (prevContinuation != null)
        {
            // Flag errors with the continuation being hooked up multiple times.
            // This is purely to help alert a developer to a bug they need to fix.
            ThrowMultipleContinuationsException();
        }
    }

    int IValueTaskSource<int>.GetResult(short token)
    {
        if (token != _token)
        {
            ThrowIncorrectTokenException();
        }

        SocketError error = SocketError;
        int bytes = BytesTransferred;
        // CancellationToken cancellationToken = _cancellationToken;

        Release();

        if (error != SocketError.Success)
        {
            ThrowException(error);
        }

        return bytes;
    }

    Socket IValueTaskSource<Socket>.GetResult(short token)
    {
        if (token != _token)
        {
            ThrowIncorrectTokenException();
        }

        SocketError error = SocketError;
        Socket acceptSocket = AcceptSocket!;
        // CancellationToken cancellationToken = _cancellationToken;

        AcceptSocket = null;

        Release();

        if (error != SocketError.Success)
        {
            ThrowException(error);
        }

        return acceptSocket;
    }

    protected override void OnCompleted(SocketAsyncEventArgs e)
    {
        Action<object?>? continuation = _continuation;
        if (continuation != null
           || (continuation = Interlocked.CompareExchange(
                ref _continuation,
                _completedSentinel,
                null)) != null)
        {
            Debug.Assert(
                continuation != _completedSentinel,
                "continuation should not have been sentinel");

            object? continuationState = UserToken;
            UserToken = null;
            _continuation = _completedSentinel;

            ExecutionContext? executionContext = _executionContext;
            if (executionContext == null)
            {
                InvokeContinuation(continuation, continuationState, false, false);
            }
            else
            {
                _executionContext = null;
                ExecutionContext.Run(
                    executionContext,
                    runState =>
                    {
                        var tuple = ((SocketChannelAsyncEventArgs, Action<object?>, object))runState!;
                        tuple.Item1.InvokeContinuation(
                            tuple.Item2,
                            tuple.Item3,
                            false,
                            false);
                    },
                    (this, continuation, continuationState));
            }
        }
    }

    private void Release()
    {
        // _cancellationToken = default;
        _token++;
        _continuation = null;
    }

    private void InvokeContinuation(
        Action<object?> continuation,
        object? state,
        bool forceAsync,
        bool requiresExecutionContextFlow)
    {
        object? scheduler = _scheduler;
        _scheduler = null;

        if (scheduler != null)
        {
            if (scheduler is SynchronizationContext sc)
            {
                sc.Post(static s =>
                {
                    var t = ((Action<object>, object))s!;
                    t.Item1(t.Item2);
                }, (continuation, state));
            }
            else
            {
                Debug.Assert(
                    scheduler is TaskScheduler,
                    $"Expected TaskScheduler, got {scheduler}");
                Task.Factory.StartNew(
                    continuation,
                    state,
                    CancellationToken.None,
                    TaskCreationOptions.DenyChildAttach,
                    (TaskScheduler)scheduler);
            }
        }
        else if (forceAsync)
        {
            if (requiresExecutionContextFlow)
            {
                ThreadPool.QueueUserWorkItem(continuation, state, preferLocal: true);
            }
            else
            {
                ThreadPool.UnsafeQueueUserWorkItem(continuation.Invoke, state);
            }
        }
        else
        {
            continuation(state);
        }
    }

    private void ThrowIncorrectTokenException()
    {
        throw new InvalidOperationException("incorret token");
    }

    private void ThrowException(SocketError error)
    {
        throw CreateException(error);
    }

    private void ThrowMultipleContinuationsException()
    {
        throw new InvalidOperationException("multiple continuation detected");
    }

    private Exception CreateException(SocketError error)
    {
        return new SocketException((int)error);
    }
}
