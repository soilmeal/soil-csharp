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
        return RunBindAsync(endPoint);
    }

    public Task StartAsync()
    {
        return StartAsync(Constants.DefaultBacklog);
    }

    public Task StartAsync(int backlog)
    {
        return RunStartAsync(backlog);
    }

    public Task StartAsync(EndPoint endPoint)
    {
        return StartAsync(endPoint, Constants.DefaultBacklog);
    }

    public Task StartAsync(EndPoint endPoint, int backlog)
    {
        return RunStartAsync(endPoint, backlog);
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

        await RunAcceptAsync();
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
        return RunCloseAsync();
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

    private void ReturnSocketChannelAsyncEventArgs(SocketChannelAsyncEventArgs? args)
    {
        if (args == null)
        {
            return;
        }

        args.AcceptSocket = null;
        args.BufferList = null;
        args.SetBuffer(null, 0, 0);
        _socketConfSection.SocketChannelEventArgsPool.Return(args);
    }

    private void HandleStartSucceed()
    {
        ChangeStatusToRunning();

        try
        {
            _handlerSet.HandleChannelActive(this);
        }
        catch (Exception ex)
        {
            _handlerSet.HandleException(_ctx, ex);
        }

        if (!_configuration.AutoRequest)
        {
            return;
        }

        TryRunAcceptAsync();
    }

    private void HandleStartFailed(Exception ex)
    {
        ChangeStatusToNone();

        _handlerSet.HandleException(_ctx, ex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunBindAsync(EndPoint endPoint)
    {
        if (_eventLoop.IsInEventLoop)
        {
            DoBind(endPoint);
            return;
        }

        await _eventLoop.StartNew(() => DoBind(endPoint));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunStartAsync(int backlog)
    {
        if (_eventLoop.IsInEventLoop)
        {
            DoStart(backlog);
            return;
        }

        await _eventLoop.StartNew(() => DoStart(backlog));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunStartAsync(EndPoint endPoint, int backlog)
    {
        if (_eventLoop.IsInEventLoop)
        {
            DoStart(endPoint, backlog);
            return;
        }

        await _eventLoop.StartNew(() => DoStart(endPoint, backlog));
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TryRunAcceptAsync()
    {
        if (!_configuration.AutoRequest)
        {
            return;
        }

#pragma warning disable CS4014
        RunAcceptAsync();
#pragma warning restore CS4014
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunAcceptAsync()
    {
        if (_eventLoop.IsInEventLoop)
        {
            await DoAcceptAsync();
            return;
        }

        Task task = await _eventLoop.StartNew(() => DoAcceptAsync());
        await task;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunCloseAsync()
    {
        if (_eventLoop.IsInEventLoop)
        {
            DoClose();
            return;
        }

        await _eventLoop.StartNew(() => DoClose());
    }

    private void DoBind(EndPoint endPoint)
    {
        try
        {
            _socket.Bind(endPoint);
        }
        catch (Exception ex)
        {
            _handlerSet.HandleException(_ctx, ex);

            throw;
        }
    }

    private void DoStart(int backlog)
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

    private void DoStart(EndPoint endPoint, int backlog)
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

    private async Task DoAcceptAsync()
    {
        SocketChannelAsyncEventArgs? args = null;
        Socket? acceptedSocket = null;
        try
        {
            args = _socketConfSection.SocketChannelEventArgsPool.Get();
            acceptedSocket = await args.AcceptAsync(_socket);

            var child = new TcpSocketChannel(
                acceptedSocket,
                _childConfiguration,
                ChannelStatus.Running);

            InitHandler.InitChannel(child);

#pragma warning disable CS4014
            child.EventLoop.StartNew(() =>
            {
                child.HandlerSet.HandleChannelActive(child);

                if (!_childConfiguration.AutoRequest)
                {
                    return;
                }

                child.RequestRead();
            });
#pragma warning restore CS4014
        }
        catch (Exception ex)
        {
            acceptedSocket?.Close();

            _handlerSet.HandleException(_ctx, ex);
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);

            TryRunAcceptAsync();
        }
    }

    private void DoClose()
    {
        if (!TryChangeStatusToClosing())
        {
            return;
        }

        try
        {
            _socket.Close();
        }
        catch (Exception ex)
        {
            _handlerSet.HandleException(_ctx, ex);

            throw;
        }
        finally
        {
            ChangeStatusToNone();

            try
            {
                _handlerSet.HandleChannelInactive(this, ChannelInactiveReason.ByLocal, null);
            }
            catch (Exception ex)
            {
                _handlerSet.HandleException(_ctx, ex);

                throw;
            }
        }
    }
}
