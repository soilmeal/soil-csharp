using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;
using Soil.Threading.Atomic;

namespace Soil.Net.Channel;

public class TcpSocketServerChannel : ISocketServerChannel, IDisposable
{
    private readonly ChannelId _id;

    private readonly AtomicInt32 _status = new((int)ChannelStatus.None);

    private readonly Socket _socket;

    private readonly IEventLoop _eventLoop;

    private readonly ChannelHandlerContext _ctx;

    private IChannelHandlerSet _handlerSet = DefaultChannelHandlerSet.Instance;

    private readonly ChannelConfiguration _configuration;

    private readonly SocketChannelConfigurationSection _socketConfSection;

    private readonly ChannelConfiguration _childConfiguration;

    public ChannelId Id
    {
        get
        {
            return _id;
        }
    }

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

    public IChannelInitHandler InitHandler
    {
        get
        {
            return _configuration.InitHandler!;
        }
    }

    public IChannelHandlerSet HandlerSet
    {
        get
        {
            return _handlerSet;
        }
        set
        {
            _handlerSet = value;
        }
    }

    public ChannelConfiguration Configuration
    {
        get
        {
            return _configuration;
        }
    }

    public ChannelConfiguration ChildConfiguration
    {
        get
        {
            return _childConfiguration;
        }
    }

    public TcpSocketServerChannel(
        AddressFamily addressFamily,
        ChannelConfiguration configuration,
        ChannelConfiguration childConfiguration)
    {
        _id = configuration.IdGenerator.Generate(this);
        _socket = new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp);
        _eventLoop = configuration.EventLoopGroup;

        _configuration = configuration;
        _socketConfSection = configuration.GetSection<SocketChannelConfigurationSection>();

        _childConfiguration = childConfiguration;

        _ctx = new ChannelHandlerContext(this);

        _socketConfSection.ApplyAllSocketOption(_socket);
    }

    ~TcpSocketServerChannel()
    {
        Dispose(false);
    }

    public Task BindAsync(EndPoint endPoint)
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
        return StartAsync(Constants.DefaultBacklog);
    }

    public Task StartAsync(int backlog)
    {
        if (!_eventLoop.IsInEventLoop)
        {
            return _eventLoop.StartNew(() => Start(backlog));
        }

        try
        {
            Start(backlog);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);

            return Task.FromException(ex);
        }
    }

    public Task StartAsync(EndPoint endPoint)
    {
        return StartAsync(endPoint, Constants.DefaultBacklog);
    }

    public Task StartAsync(EndPoint endPoint, int backlog)
    {
        if (!_eventLoop.IsInEventLoop)
        {
            return _eventLoop.StartNew(() => Start(endPoint, backlog));
        }

        try
        {
            Start(endPoint, backlog);
            return Task.CompletedTask;
        }
        catch (Exception ex)
        {
            return Task.FromException(ex);
        }
    }

    public Task StartAsync(IPAddress address, int port)
    {
        return StartAsync(address, port, Constants.DefaultBacklog);
    }

    public Task StartAsync(IPAddress address, int port, int backlog)
    {
        return StartAsync(new IPEndPoint(address, port), backlog);
    }

    public Task StartAsync(string host, int port)
    {
        return StartAsync(host, port, Constants.DefaultBacklog);
    }

    public Task StartAsync(string host, int port, int backlog)
    {

        IPAddress? address;
        EndPoint endPoint = IPAddress.TryParse(host, out address)
            ? new IPEndPoint(address, port)
            : new DnsEndPoint(host, port);

        return StartAsync(endPoint, backlog);
    }

    public async void RequestAccept()
    {
        if (Status != ChannelStatus.Running)
        {
            return;
        }

        SocketChannelAsyncEventArgs args = _socketConfSection.SocketChannelEventArgsPool.Get();
        try
        {
            Socket acceptedSocket = await args.AcceptAsync(_socket);

            RunInitHandler(acceptedSocket);
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);

            TryRequestAccept();
        }
    }

    public void RequestRead(IByteBuffer? byteBuffer = null)
    {
        throw new NotSupportedException();
    }

    public Task<int> WriteAsync(object message)
    {
        throw new NotSupportedException();
    }

    public Task CloseAsync()
    {
        return CloseAsync(ChannelInactiveReason.ByLocal, null);
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

    private void Start(EndPoint endPoint, int backlog)
    {
        if (IsBound)
        {
            throw new InvalidOperationException("already bound");
        }

        if (!TryChangeStatusToStarting())
        {
            return;
        }

        try
        {
            _socket.Bind(endPoint);
            _socket.Listen(backlog);

            HandleStartSucceed();
        }
        catch (Exception ex)
        {
            HandleStartFailed(ex);

            throw;
        }
    }

    private void Start(int backlog)
    {
        if (!TryChangeStatusToStarting())
        {
            return;
        }

        try
        {
            _socket.Listen(backlog);

            HandleStartSucceed();
        }
        catch (Exception ex)
        {
            HandleStartFailed(ex);

            throw;
        }
    }

    private async Task CloseAsync(ChannelInactiveReason reason, Exception? cause)
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
            HandleClose(reason, cause);
        }
    }

    private void Close()
    {
        _socket.Close();
    }

    private void ReturnSocketChannelAsyncEventArgs(SocketChannelAsyncEventArgs args)
    {
        args.AcceptSocket = null;
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

    private void TryRequestAccept()
    {
        if (!_configuration.AutoRequest)
        {
            return;
        }

        RequestAccept();
    }

    private void HandleStartSucceed()
    {
        ChangeStatusToRunning();

        RunLifecycleHandlerActive();

        if (!_configuration.AutoRequest)
        {
            return;
        }

        TryRequestAccept();
    }

    private void HandleStartFailed(Exception ex)
    {
        ChangeStatusToNone();

        RunExceptionHandler(ex);
    }

    private void HandleClose(ChannelInactiveReason reason, Exception? cause)
    {
        ChangeStatusToNone();

        RunLifecycleHandlerInactive(reason, cause);
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

    private void RunInitHandler(Socket socket)
    {
        if (_eventLoop.IsInEventLoop)
        {
            InvokeInitHandler(socket);
            return;
        }

        _eventLoop.StartNew(() => InvokeInitHandler(socket));
    }

    private void RunLifecycleHandlerInactive(ChannelInactiveReason reason, Exception? cause)
    {
        if (_eventLoop.IsInEventLoop)
        {
            InvokeLifecycleInactive(reason, cause);
            return;
        }

        _eventLoop.StartNew(() => InvokeLifecycleInactive(reason, cause));
    }

    private void RunExceptionHandler(Exception ex)
    {
        if (_eventLoop.IsInEventLoop)
        {
            _handlerSet.HandleException(_ctx, ex);
            return;
        }

        _eventLoop.StartNew(() => _handlerSet.HandleException(_ctx, ex));
    }

    private void InvokeInitHandler(Socket socket)
    {
        var child = new TcpSocketChannel(
            socket,
            _childConfiguration,
            ChannelStatus.Running);
        try
        {
            InitHandler.InitChannel(child);

            child.EventLoop.StartNew(() =>
            {
                child.HandlerSet.HandleChannelActive(child);

                if (!_childConfiguration.AutoRequest)
                {
                    return;
                }

                child.RequestRead();
            });
        }
        catch (Exception ex)
        {
            socket.Close();

            RunExceptionHandler(ex);
        }
    }

    private void InvokeLifecycleActive()
    {
        try
        {
            _handlerSet.HandleChannelActive(this);
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);
        }
    }

    private void InvokeLifecycleInactive(ChannelInactiveReason reason, Exception? cause)
    {
        try
        {
            _handlerSet.HandleChannelInactive(this, reason, cause);
        }
        catch (Exception ex)
        {
            RunExceptionHandler(ex);
        }
    }
}
