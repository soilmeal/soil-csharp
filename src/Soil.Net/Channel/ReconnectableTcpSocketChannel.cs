using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;
using Soil.Types;

namespace Soil.Net.Channel;

public class ReconnectableTcpSocketChannel : IReconnectableChannel
{
    private readonly TcpSocketChannel _channel;

    private readonly ChannelHandlerContext _ctx;

    private IChannelHandlerSet _handlerSet = DefaultChannelHandlerSet.Instance;

    private readonly ChannelConfiguration _configuration;

    private readonly SocketChannelConfigurationSection _socketConfSection;

    private bool _started = false;

    private EndPoint? _endPoint;

    public ReconnectableTcpSocketChannel(
        AddressFamily addressFamily,
        ChannelConfiguration configuration)
        : this(new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp), configuration)
    {
    }

    public ReconnectableTcpSocketChannel(
        Socket socket,
        ChannelConfiguration configuration)
        : this(socket, configuration, ChannelStatus.None)
    {
    }

    public ReconnectableTcpSocketChannel(
        Socket socket,
        ChannelConfiguration configuration,
        ChannelStatus status)
    {
        _configuration = configuration;
        _socketConfSection = configuration.GetSection<SocketChannelConfigurationSection>();

        var innerConfigurationBuilder = new ChannelConfiguration.Builder()
            .SetAllocator(configuration.Allocator)
            .SetEventLoopGroup(configuration.EventLoopGroup)
            .SetAutoRequest(configuration.AutoRequest);
        innerConfigurationBuilder.AddSection(_socketConfSection);

        _channel = new TcpSocketChannel(socket, innerConfigurationBuilder.Build(), status)
        {
            HandlerSet = new IChannelHandlerSet.Builder<object>()
                .SetLifecycleHandler(new InnerLifecycleHandler(this))
                .SetExceptionHandler(new InnerExceptionHandler(this))
                .SetInboundPipe(new InnerInboundPipe(this))
                .SetOutboundPipe(new InnerOutboundPipe(this))
                .Build(),
        };

        _ctx = new ChannelHandlerContext(this);
    }

    public ChannelId Id
    {
        get
        {
            return _channel.Id;
        }
    }

    public AddressFamily AddressFamily
    {
        get
        {
            return _channel.AddressFamily;
        }
    }

    public SocketType SocketType
    {
        get
        {
            return _channel.SocketType;
        }
    }

    public ProtocolType ProtocolType
    {
        get
        {
            return _channel.ProtocolType;
        }
    }

    public EndPoint? LocalEndPoint
    {
        get
        {
            return _channel.LocalEndPoint;
        }
    }

    public EndPoint? RemoteEndPoint
    {
        get
        {
            return _channel.RemoteEndPoint;
        }
    }

    public bool IsBound
    {
        get
        {
            return _channel.IsBound;
        }
    }

    public bool Connected
    {
        get
        {
            return _channel.Connected;
        }
    }

    public ChannelStatus Status
    {
        get
        {
            return _channel.Status;
        }
    }

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _channel.Allocator;
        }
    }

    public IEventLoop EventLoop
    {
        get
        {
            return _channel.EventLoop;
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

    public IChannelReconnectStrategy ReconnectStrategy
    {
        get
        {
            return _configuration.ReconnectStrategy!;
        }
    }

    public IChannelReconnectHandler ReconnectHandler
    {
        get
        {
            return _configuration.ReconnectHandler!;
        }
    }

    public ChannelConfiguration Configuration
    {
        get
        {
            return _configuration;
        }
    }

    public Task BindAsync(EndPoint endPoint)
    {
        return RunBindAsync(endPoint);
    }

    public Task CloseAsync()
    {
        return _channel.CloseAsync();
    }

    public void RequestRead()
    {
        _channel.RequestRead();
    }

    public Task StartAsync()
    {
        return _channel.StartAsync();
    }

    public Task StartAsync(int backlog)
    {
        return _channel.StartAsync(backlog);
    }

    public Task StartAsync(EndPoint endPoint)
    {
        return RunStartAsync(endPoint);
    }

    public Task StartAsync(EndPoint endPoint, int backlog)
    {
        return _channel.StartAsync(endPoint, backlog);
    }

    public Task StartAsync(IPAddress address, int port)
    {
        return address != null
            ? StartAsync(new IPEndPoint(address, port))
            : throw new ArgumentNullException(nameof(address));
    }

    public Task StartAsync(IPAddress address, int port, int backlog)
    {
        return _channel.StartAsync(address, port, backlog);
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
        return _channel.StartAsync(host, port, backlog);
    }

    public Task<int> WriteAsync(object message)
    {
        return _channel.WriteAsync(message);
    }

    private async Task RunBindAsync(EndPoint endPoint)
    {
        if (EventLoop.IsInEventLoop)
        {
            await DoBindAsync(endPoint);
            return;
        }

        Task task = EventLoop.StartNew(() => DoBindAsync(endPoint));
        await task;
    }

    private async Task RunStartAsync(EndPoint endPoint)
    {
        if (EventLoop.IsInEventLoop)
        {
            await DoStartAsync(endPoint);
            return;
        }

        Task task = EventLoop.StartNew(() => DoStartAsync(endPoint));
        await task;
    }

    private async Task RunReconnectAsync(
        EndPoint endPoint,
        ChannelReconnectReason reason,
        Exception? cause)
    {
        if (EventLoop.IsInEventLoop)
        {
            await DoReconnectAsync(endPoint, reason, cause);
            return;
        }

        Task task = EventLoop.StartNew(() => DoReconnectAsync(endPoint, reason, cause));
        await task;
    }

    private async Task DoBindAsync(EndPoint endPoint)
    {
        try
        {
            await _channel.BindAsync(endPoint);

            _endPoint = endPoint;
        }
        catch
        {
            throw;
        }
    }

    private async Task DoStartAsync(EndPoint endPoint)
    {
        try
        {
            await _channel.StartAsync(endPoint);

            _endPoint = endPoint;
        }
        catch (Exception startEx)
        {
            try
            {
                await RunReconnectAsync(endPoint, ChannelReconnectReason.ThrownWhenStart, startEx);

                _endPoint = endPoint;
            }
            catch
            {
                throw;
            }
        }
    }

    private async Task DoReconnectAsync(
        EndPoint endPoint,
        ChannelReconnectReason reason,
        Exception? cause)
    {
        if (endPoint == null)
        {
            throw new InvalidOperationException("Call Bind() or Start() first");
        }

        IChannelReconnectStrategy reconnectStrategy = ReconnectStrategy;
        if (reconnectStrategy == null)
        {
            throw new InvalidOperationException("ReconnectStrategy is null");
        }

        IChannelReconnectHandler reconnectHandler = ReconnectHandler;
        if (reconnectHandler == null)
        {
            throw new InvalidOperationException("ReconnectHandler is null");
        }

        int currentReconnectCount = 0;

        Exception? lastException = null;
        bool reconnectStartCalled = false;
        while (true)
        {
            double waitMilliseconds;
            try
            {
                waitMilliseconds = reconnectStrategy.TryReconnect(
                    ++currentReconnectCount,
                    reason,
                    cause);
                if (waitMilliseconds <= 0.0)
                {
                    break;
                }
            }
            catch (Exception ex)
            {
                HandlerSet.HandleException(_ctx, ex);
                lastException = ex;
                break;
            }


            if (!reconnectStartCalled)
            {
                reconnectStartCalled = true;

                ReconnectHandler.HandleReconnectStart(_ctx);
            }

            try
            {
                await Task.Delay(TimeSpan.FromMilliseconds(waitMilliseconds));

                await _channel.StartAsync(endPoint);

                ReconnectHandler.HandleReconnectEnd(_ctx, true);

                return;
            }
            catch (Exception ex)
            {
                lastException = ex;
            }
        }

        ReconnectHandler.HandleReconnectEnd(_ctx, false);

        throw new InvalidOperationException("retry failed", lastException);
    }

    private class InnerLifecycleHandler : IChannelLifecycleHandler
    {
        private readonly ReconnectableTcpSocketChannel _parent;

        public InnerLifecycleHandler(ReconnectableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public void HandleChannelActive(IChannel channel)
        {
            if (_parent._started)
            {
                return;
            }

            _parent._started = true;
            _parent._handlerSet.HandleChannelActive(_parent);
        }

        public void HandleChannelInactive(
            IChannel channel,
            ChannelInactiveReason reason,
            Exception? cause)
        {
            ChannelReconnectReason reconnectReason = reason.ConvertToReconnectReason();
            _parent.RunReconnectAsync(_parent._endPoint!, reconnectReason, cause)
                .ContinueWith((task) =>
                {
                    if (!task.IsFaulted)
                    {
                        return;
                    }

                    _parent._handlerSet.HandleChannelInactive(
                        _parent,
                        reason,
                        task.Exception ?? cause);
                });
        }
    }

    private class InnerExceptionHandler : IChannelExceptionHandler
    {
        private readonly ReconnectableTcpSocketChannel _parent;

        public InnerExceptionHandler(ReconnectableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public void HandleException(IChannelHandlerContext ctx, Exception ex)
        {
            _parent._handlerSet.HandleException(_parent._ctx, ex);
        }
    }

    private class InnerInboundPipe : IChannelInboundPipe<IByteBuffer, Unit>
    {
        private readonly ReconnectableTcpSocketChannel _parent;

        public InnerInboundPipe(ReconnectableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public Result<ChannelPipeResultType, Unit> Transform(
            IChannelHandlerContext ctx,
            IByteBuffer message)
        {
            return _parent._handlerSet.HandleRead(_parent._ctx, message);
        }
    }

    private class InnerOutboundPipe : IChannelOutboundPipe<object, IByteBuffer>
    {
        private readonly ReconnectableTcpSocketChannel _parent;

        public InnerOutboundPipe(ReconnectableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public Result<ChannelPipeResultType, IByteBuffer> Transform(
            IChannelHandlerContext ctx,
            object message)
        {
            return _parent._handlerSet.HandleWrite(_parent._ctx, message);
        }

        Result<ChannelPipeResultType, IByteBuffer> IChannelOutboundPipe<object, IByteBuffer>.Transform(IChannelHandlerContext ctx, object message)
        {
            return Transform(ctx, message);
        }
    }
}
