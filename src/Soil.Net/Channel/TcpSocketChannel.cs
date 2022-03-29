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
    private readonly ChannelId _id;
    private readonly AtomicInt32 _status = new((int)ChannelStatus.None);

    private readonly Socket _socket;

    private readonly IEventLoop _eventLoop;

    private readonly ChannelHandlerContext _ctx;

    private IChannelHandlerSet _handlerSet = DefaultChannelHandlerSet.Instance;

    private readonly ChannelConfiguration _configuration;

    private readonly SocketChannelConfigurationSection _socketConfSection;


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

    public IChannelHandlerSet HandlerSet
    {
        get
        {
            return _handlerSet;
        }
        set
        {
            _handlerSet = value ?? throw new ArgumentNullException(nameof(value));
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
        _id = configuration.IdGenerator.Generate(this);
        _socket = socket;
        _eventLoop = configuration.EventLoopGroup.Next();

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
        return RunBindAsync(endPoint);
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

    public Task StartAsync(EndPoint endPoint)
    {
        return RunConnectAsync(endPoint);
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

    public void RequestRead(IByteBuffer? byteBuffer = null)
    {
#pragma warning disable CS4014
        RunReceiveAsync(byteBuffer);
#pragma warning restore CS4014
    }

    public Task<int> WriteAsync(object message)
    {
        return RunSendAsync(message);
    }

    public Task CloseAsync()
    {
        return RunCloseAsync(ChannelInactiveReason.ByLocal, null);
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

    private void ReturnSocketChannelAsyncEventArgs(SocketChannelAsyncEventArgs args)
    {
        args.AcceptSocket = null;
        args.SetBuffer((Memory<byte>)null);
        _socketConfSection.SocketChannelEventArgsPool.Return(args);
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
    private async Task RunConnectAsync(EndPoint endPoint)
    {
        if (_eventLoop.IsInEventLoop)
        {
            await DoConnectAsync(endPoint);
            return;
        }

        Task task = await _eventLoop.StartNew(() => DoConnectAsync(endPoint));
        await task;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private void TryRunReceiveAsync(IByteBuffer? byteBuffer = null)
    {
        if (!_configuration.AutoRequest)
        {
            return;
        }

#pragma warning disable CS4014
        RunReceiveAsync(byteBuffer);
#pragma warning restore CS4014
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunReceiveAsync(IByteBuffer? byteBuffer = null)
    {
        if (_eventLoop.IsInEventLoop)
        {
            await DoReceiveAsync(byteBuffer);
        }

        Task task = await _eventLoop.StartNew(() => DoReceiveAsync(byteBuffer));
        await task;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task<int> RunSendAsync(object message)
    {
        if (_eventLoop.IsInEventLoop)
        {
            return await DoSendAsync(message);
        }


        Task<int> task = await _eventLoop.StartNew(() => DoSendAsync(message));
        return await task;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    private async Task RunCloseAsync(ChannelInactiveReason reason, Exception? cause)
    {
        if (_eventLoop.IsInEventLoop)
        {
            DoClose(reason, cause);
            return;
        }

        await _eventLoop.StartNew(() => DoClose(reason, cause));
    }

    private void DoBind(EndPoint endPoint)
    {
        try
        {
            _socket.Bind(endPoint);
        }
        catch
        {
            throw;
        }
    }

    private async Task DoConnectAsync(EndPoint endPoint)
    {
        if (!TryChangeStatusToStarting())
        {
            return;
        }

        SocketChannelAsyncEventArgs args = _socketConfSection.SocketChannelEventArgsPool.Get();
        // SocketAsyncEventArgs args = new SocketAsyncEventArgs();
        try
        {
            args.RemoteEndPoint = endPoint;

            await args.ConnectAsync(_socket);

            ChangeStatusToRunning();

            _handlerSet.HandleChannelActive(this);
        }
        catch (Exception ex)
        {
            ChangeStatusToNone();

            _handlerSet.HandleException(_ctx, ex);

            throw;
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);

            if (Status == ChannelStatus.Running)
            {
                TryRunReceiveAsync();
            }
        }
    }

    private async Task DoReceiveAsync(IByteBuffer? byteBuffer = null)
    {
        if (Status != ChannelStatus.Running)
        {
            return;
        }

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

            int recvBytes = await args.ReceiveAsync(_socket);
            if (recvBytes <= 0)
            {
#pragma warning disable CS4014
                RunCloseAsync(ChannelInactiveReason.ByRemote, null);
#pragma warning restore CS4014
                return;
            }

            byteBuffer.Unsafe.SetWriteIndex(byteBuffer.WriteIndex + recvBytes);

            Unit? result = await _handlerSet.HandleReadAsync(_ctx, byteBuffer);
            if (result == null)
            {
                TryRunReceiveAsync(byteBuffer);
                return;
            }

            IByteBuffer prevByteBuffer = byteBuffer;
            if (prevByteBuffer.ReadableBytes > 0)
            {
                byteBuffer = Allocator.Allocate(prevByteBuffer.ReadableBytes);
                byteBuffer.ReadBytes(prevByteBuffer);
            }
            else
            {
                byteBuffer = null;
            }

            prevByteBuffer.Release();

            TryRunReceiveAsync(byteBuffer);
        }
        catch (SocketException ex)
            when (_socketConfSection.ContainsInCloseError(ex.SocketErrorCode))
        {
            if (_socketConfSection.InvokeHandlerWhenCloseErrorCaught)
            {
                _handlerSet.HandleException(_ctx, ex);
            }

#pragma warning disable CS4014
            RunCloseAsync(ChannelInactiveReason.ByException, ex);
#pragma warning restore CS4014
        }
        catch (Exception ex)
        {
            _handlerSet.HandleException(_ctx, ex);
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);
        }
    }

    private async Task<int> DoSendAsync(object message)
    {
        if (Status != ChannelStatus.Running)
        {
            return 0;
        }

        if (message == null)
        {
            throw new ArgumentNullException(nameof(message));
        }

        SocketChannelAsyncEventArgs args = _socketConfSection.SocketChannelEventArgsPool.Get();
        IByteBuffer? byteBuffer = null;
        try
        {
            byteBuffer = await _handlerSet.HandleWriteAsync(_ctx, message);
            if (byteBuffer == null)
            {
                throw new InvalidOperationException($"outbound pipe failed");
            }

            args.SetBuffer(byteBuffer.Unsafe.AsMemoryToSend());

            int sendBytes = await args.SendAsync(_socket);

            byteBuffer.Unsafe.SetReadIndex(byteBuffer.ReadIndex + sendBytes);

            return sendBytes;
        }
        catch (SocketException ex)
            when (_socketConfSection.ContainsInCloseError(ex.SocketErrorCode))
        {
            if (_socketConfSection.InvokeHandlerWhenCloseErrorCaught)
            {
                _handlerSet.HandleException(_ctx, ex);
            }

#pragma warning disable CS4014
            RunCloseAsync(ChannelInactiveReason.ByException, ex);
#pragma warning restore CS4014

            throw;
        }
        catch (Exception ex)
        {
            _handlerSet.HandleException(_ctx, ex);

            throw;
        }
        finally
        {
            ReturnSocketChannelAsyncEventArgs(args);

            byteBuffer?.Release();
        }
    }

    private void DoClose(ChannelInactiveReason reason, Exception? cause)
    {
        if (!TryChangeStatusToClosing())
        {
            return;
        }

        try
        {
            _socket.Shutdown(_socketConfSection.ShutdownHow);
        }
        catch (Exception ex)
        {
            _handlerSet.HandleException(_ctx, ex);

            throw;
        }
        finally
        {
            _socket.Disconnect(true);

            ChangeStatusToNone();

            _handlerSet.HandleChannelInactive(this, reason, cause);
        }
    }

    [DoesNotReturn]
    private void ThrowNotSupportedException()
    {
        throw new NotSupportedException("Not supported operation by socket channel.");
    }
}
