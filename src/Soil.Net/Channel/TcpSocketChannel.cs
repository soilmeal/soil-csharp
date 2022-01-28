using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;
using Soil.Threading.Atomic;
using Soil.Types;

namespace Soil.Net.Channel;

public class TcpSocketChannel : ISocketChannel, IDisposable
{
    private readonly AtomicInt32 _status = new((int)ChannelStatus.None);

    private readonly Socket _socket;

    private readonly IEventLoop _eventLoop;

    private readonly ChannelHandlerContext _ctx;

    private IChannelPipeline _pipeline;

    private readonly ChannelConfiguration _configuration;

    private readonly SocketChannelConfigurationSection _socketConfSection;

    public AddressFamily AddressFamily
    {
        get
        {
            return _socket.AddressFamily;
        }
    }

    public SocketType SocketType
    {
        get
        {
            return _socket.SocketType;
        }
    }

    public ProtocolType ProtocolType
    {
        get
        {
            return _socket.ProtocolType;
        }
    }

    public EndPoint? LocalEndPoint
    {
        get
        {
            return _socket.LocalEndPoint;
        }
    }

    public EndPoint? RemoteEndPoint
    {
        get
        {
            return _socket.RemoteEndPoint;
        }
    }

    public bool IsBound
    {
        get
        {
            return _socket.IsBound;
        }
    }

    public bool Connected
    {
        get
        {
            return _socket.Connected;
        }
    }

    public ChannelStatus Status
    {
        get
        {
            return (ChannelStatus)_status.Read();
        }
    }

    public Socket Socket
    {
        get
        {
            return _socket;
        }
    }

    public SocketShutdown ShutdownHow
    {
        get
        {
            return _socketConfSection.ShutdownHow;
        }
    }

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _configuration.Allocator;
        }
    }

    public IEventLoop EventLoop
    {
        get
        {
            return _eventLoop;
        }
    }

    public IChannelLifecycleHandler LifecycleHandler
    {
        get
        {
            return _configuration.LifecycleHandler;
        }
    }

    public IChannelExceptionHandler ExceptionHandler
    {
        get
        {
            return _configuration.ExceptionHandler;
        }
    }

    public IChannelPipeline Pipeline
    {
        get
        {
            return _pipeline;
        }
        set
        {
            _pipeline = value ?? throw new ArgumentNullException(nameof(value));
        }
    }

    public ChannelConfiguration Configuration
    {
        get
        {
            return _configuration;
        }
    }

    public TcpSocketChannel(
        AddressFamily addressFamily,
        ChannelConfiguration configuration)
        : this(new Socket(
            addressFamily,
            SocketType.Stream,
            ProtocolType.Tcp), configuration)
    {
    }

    public TcpSocketChannel(
        Socket socket,
        ChannelConfiguration configuration)
    {
        _socket = socket;
        _eventLoop = configuration.EventLoopGroup.Next();
        _pipeline = configuration.Pipeline;

        _configuration = configuration;
        _socketConfSection = configuration.GetSection<SocketChannelConfigurationSection>();

        _ctx = new ChannelHandlerContext(this);

        _socketConfSection.ApplyAllSocketOption(socket);
    }

    public TcpSocketChannel(
        Socket socket,
        ChannelConfiguration configuration,
        ChannelStatus status)
            : this(
                  socket,
                  configuration)
    {
        _status.Exchange((int)status);
    }

    ~TcpSocketChannel()
    {
        Dispose(false);
    }

    public Task BindAsync(EndPoint endPoint)
    {
        return ((IChannel)this).BindAsync(endPoint);
    }

    Task IChannel.BindAsync(EndPoint endPoint)
    {
        if (!_eventLoop.IsInEventLoop)
        {
            return _eventLoop.StartNew(() => _socket.Bind(endPoint));
        }

        try
        {
            _socket.Bind(endPoint);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task StartAsync()
    {
        ThrowNotSupportedException();

        return Task.CompletedTask;
    }

    public Task StartAsync(int backlog)
    {
        ThrowNotSupportedException();

        return Task.CompletedTask;
    }

    public async Task StartAsync(EndPoint endPoint)
    {
        if (!TryChangeStatusToStarting())
        {
            return;
        }

        SocketChannelAsyncEventArgs args = _socketConfSection.SocketChannelEventArgsPool.Get();
        try
        {
            args.RemoteEndPoint = endPoint;
            await args.ConnectAsync(_socket)
                .ConfigureAwait(false);

            HandleStartSucceed();
        }
        catch (Exception ex)
        {
            // TODO: log

            HandleStartFailed(ex);

            throw;
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);
        }
    }

    public Task StartAsync(EndPoint endPoint, int backlog)
    {
        ThrowNotSupportedException();

        return Task.CompletedTask;
    }

    public Task StartAsync(IPAddress address, int port)
    {
        return address != null
            ? StartAsync(new IPEndPoint(address, port))
            : throw new ArgumentNullException(nameof(address));
    }

    public Task StartAsync(IPAddress address, int port, int backlog)
    {
        ThrowNotSupportedException();

        return Task.CompletedTask;
    }

    public Task StartAsync(string host, int port)
    {
        IPAddress? address;
        EndPoint endPoint = IPAddress.TryParse(host, out address)
            ? new IPEndPoint(address, port)
            : new DnsEndPoint(host, port);

        return StartAsync(endPoint);
    }

    public Task StartAsync(string host, int port, int backlog)
    {
        ThrowNotSupportedException();

        return Task.CompletedTask;
    }

    public async void RequestRead(IByteBuffer? byteBuffer = null)
    {
        if (byteBuffer == null)
        {
            byteBuffer = Allocator.Allocate(1024);
        }
        else
        {
            byteBuffer.EnsureCapacity();
        }

        SocketChannelAsyncEventArgs args = _socketConfSection.SocketChannelEventArgsPool.Get();
        try
        {
            args.SetBuffer(byteBuffer.Unsafe.AsMemoryToRecv());
            int recvBytes = await args.ReceiveAsync(_socket)
                .ConfigureAwait(false);
            if (recvBytes <= 0)
            {
#pragma warning disable CS4014
                CloseAsync();
#pragma warning restore CS4014
                return;
            }

            HandleReadCompleted(recvBytes, byteBuffer);

            RunInboundPipe(byteBuffer);
        }
        catch (SocketException ex)
            when (_socketConfSection.ContainsInCloseError(ex.SocketErrorCode))
        {
            // TODO: log
            if (_socketConfSection.InvokeHandlerWhenCloseErrorCaught)
            {
                RunExceptionHandler(ex);
            }

#pragma warning disable CS4014
            CloseAsync();
#pragma warning restore CS4014
        }
        catch (Exception ex)
        {
            // TODO: log

            RunExceptionHandler(ex);
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);
        }
    }


    public async Task<int> WriteAsync(object message)
    {
        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        ChannelStatus status = Status;
        if (status != ChannelStatus.Running)
        {
            return 0;
        }

        SocketChannelAsyncEventArgs args = _socketConfSection.SocketChannelEventArgsPool.Get();
        IByteBuffer? byteBuffer = null;
        try
        {
            var result = await RunOutboundPipe(message)
                .ConfigureAwait(false);
            switch (result.Type)
            {
                case ChannelPipeResultType.None:
                case ChannelPipeResultType.ContinueIO:
                {
                    throw new InvalidOperationException($"outbound pipe failed - type={ result.Type.FastToString()}");
                }
                default:
                {
                    break;
                }
            }
            if (!result.HasValue())
            {
                throw new InvalidOperationException($"outbound pipe failed - hasValue={result.HasValue()}");
            }

            byteBuffer = result.Value!;
            args.SetBuffer(byteBuffer.Unsafe.AsMemoryToSend());
            int sendBytes = await args.SendAsync(_socket)
                .ConfigureAwait(false);
            if (sendBytes <= 0)
            {
#pragma warning disable CS4014
                CloseAsync();
#pragma warning restore CS4014
                return sendBytes;
            }

            HandleWriteCompleted(sendBytes, byteBuffer);
            return sendBytes;
        }
        catch (SocketException ex)
            when (_socketConfSection.ContainsInCloseError(ex.SocketErrorCode))
        {
            // TODO: log
            if (_socketConfSection.InvokeHandlerWhenCloseErrorCaught)
            {
                RunExceptionHandler(ex);
            }

#pragma warning disable CS4014
            CloseAsync();
#pragma warning restore CS4014

            throw;
        }
        catch (Exception ex)
        {
            // TODO: log

            RunExceptionHandler(ex);

            throw;
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);

            byteBuffer?.Release();
        }
    }

    public async Task CloseAsync()
    {
        if (!TryChangeStatusToClosing())
        {
            return;
        }

        try
        {
            await RunCloseAsync()
                   .ConfigureAwait(false);
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);

            throw;
        }
        finally
        {
            HandleClose();
        }
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        ChangeStatusToNone();

        _socket.Dispose();
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ChangeStatus(ChannelStatus status)
    {
        _status.Exchange((int)status);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private ChannelStatus TryChangeStatus(ChannelStatus status, ChannelStatus comparandStatus)
    {
        return (ChannelStatus)_status.CompareExchange(
            (int)status,
            (int)comparandStatus);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ChangeStatusToNone()
    {
        ChangeStatus(ChannelStatus.None);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void ChangeStatusToRunning()
    {
        ChangeStatus(ChannelStatus.Running);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryChangeStatusToStarting()
    {
        ChannelStatus oldStatus = TryChangeStatus(ChannelStatus.Starting, ChannelStatus.None);
        return oldStatus == ChannelStatus.None;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private bool TryChangeStatusToClosing()
    {
        ChannelStatus oldStatus = TryChangeStatus(ChannelStatus.Closing, ChannelStatus.Running);
        return oldStatus == ChannelStatus.Running;
    }

    private void Close()
    {
        try
        {
            _socket.Shutdown(_socketConfSection.ShutdownHow);
        }
        finally
        {
            _socket.Disconnect(false);
        }
    }

    private void ReturnSocketChannelAsyncEventArgs(SocketChannelAsyncEventArgs args)
    {
        args.AcceptSocket = null;
        args.BufferList = null;
        args.SetBuffer(null, 0, 0);
        _socketConfSection.SocketChannelEventArgsPool.Return(args);
    }

    private Task RunCloseAsync()
    {
        if (!_eventLoop.IsInEventLoop)
        {
            return _eventLoop.StartNew(Close);
        }

        try
        {
            Close();
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    private void TryRequestRead()
    {
        if (!_configuration.AutoRequest)
        {
            return;
        }

        RequestRead();
    }

    private void HandleStartSucceed()
    {
        ChangeStatusToRunning();

        RunLifecycleHandlerActive();

        TryRequestRead();
    }

    private void HandleStartFailed(Exception ex)
    {
        ChangeStatusToNone();

        RunExceptionHandler(ex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleReadCompleted(int recvBytes, IByteBuffer byteBuffer)
    {
        byteBuffer.Unsafe.SetWriteIndex(byteBuffer.WriteIndex + recvBytes);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void HandleWriteCompleted(int sendBytes, IByteBuffer byteBuffer)
    {
        byteBuffer.Unsafe.SetReadIndex(byteBuffer.ReadIndex + sendBytes);
    }

    private void HandleClose()
    {
        ChangeStatusToNone();

        RunLifecycleHandlerInactive();
    }

    private void RunLifecycleHandlerActive()
    {
        if (_eventLoop.IsInEventLoop)
        {
            InvokeLifecycleActive();
            return;
        }

        _eventLoop.StartNew(() => InvokeLifecycleActive());
    }

    private void RunLifecycleHandlerInactive()
    {
        if (_eventLoop.IsInEventLoop)
        {
            InvokeLifecycleInactive();
            return;
        }

        _eventLoop.StartNew(() => InvokeLifecycleInactive());
    }

    private void RunInboundPipe(IByteBuffer byteBuffer)
    {
        if (_eventLoop.IsInEventLoop)
        {
            InvokeInboundPipe(byteBuffer);
            return;
        }

        _eventLoop.StartNew(() => InvokeInboundPipe(byteBuffer));
    }

    private Task<Result<ChannelPipeResultType, IByteBuffer>> RunOutboundPipe(object message)
    {
        if (!_eventLoop.IsInEventLoop)
        {
            return _eventLoop.StartNew(() => _pipeline.HandleWrite(_ctx, message));
        }

        try
        {
            return Task.FromResult(_pipeline.HandleWrite(_ctx, message));
        }
        catch (Exception ex)
        {
            return Task.FromException<Result<ChannelPipeResultType, IByteBuffer>>(ex);
        }
    }

    private void RunExceptionHandler(Exception ex)
    {
        if (_eventLoop.IsInEventLoop)
        {
            ExceptionHandler.HandleException(_ctx, ex);
            return;
        }

        _eventLoop.StartNew(() => ExceptionHandler.HandleException(_ctx, ex));
    }

    private void InvokeLifecycleActive()
    {
        try
        {
            LifecycleHandler.HandleChannelActive(this);
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);
        }
    }

    private void InvokeLifecycleInactive()
    {
        try
        {
            LifecycleHandler.HandleChannelInactive(this);
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);
        }
    }

    private void InvokeInboundPipe(IByteBuffer byteBuffer)
    {
        try
        {
            var result = _pipeline.HandleRead(_ctx, byteBuffer);

            switch (result.Type)
            {
                case ChannelPipeResultType.CallNext:
                {
                    if (byteBuffer.IsInitialized)
                    {
                        byteBuffer.Release();
                    }

                    TryRequestRead();
                    break;
                }
                case ChannelPipeResultType.ContinueIO:
                {
                    RequestRead(byteBuffer);
                    break;
                }
                default:
                {
                    throw new ArgumentOutOfRangeException(nameof(result.Type), result.Type, "should not be reached here");
                }
            }
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);
        }
    }

    [DoesNotReturn]
    private void ThrowNotSupportedException()
    {
        throw new NotSupportedException("Not supported operation by socket channel.");
    }
}
