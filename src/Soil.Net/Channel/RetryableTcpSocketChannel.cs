using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;
using Soil.Types;

namespace Soil.Net.Channel;

public class RetryableTcpSocketChannel : IRetryableChannel
{
    private readonly TcpSocketChannel _channel;

    private readonly ChannelHandlerContext _ctx;

    private IChannelPipeline _pipeline;

    private readonly ChannelConfiguration _configuration;

    private int _currentRetryCount = 0;

    private EndPoint? _endPoint;

    public RetryableTcpSocketChannel(
        AddressFamily addressFamily,
        ChannelConfiguration configuration)
        : this(new Socket(addressFamily, SocketType.Stream, ProtocolType.Tcp), configuration)
    {
    }

    public RetryableTcpSocketChannel(
        Socket socket,
        ChannelConfiguration configuration)
        : this(socket, configuration, ChannelStatus.None)
    {
    }

    public RetryableTcpSocketChannel(
        Socket socket,
        ChannelConfiguration configuration,
        ChannelStatus status)
    {
        _pipeline = configuration.Pipeline;

        _configuration = configuration;

        var innerConfigurationBuilder = new ChannelConfiguration.Builder()
            .SetAllocator(configuration.Allocator)
            .SetEventLoopGroup(configuration.EventLoopGroup)
            .SetLifecycleHandler(new RetryableChannelLifecycleHandler(this))
            .SetExceptionHandler(new RetryableChannelExceptionHandler(this))
            .SetPipeline(IChannelPipeline.Create(
                new RetryableChannelInboundPipe(this),
                new RetryableChannelOutboundPipe(this)))
            .SetAutoRequest(configuration.AutoRequest);
        innerConfigurationBuilder.AddSection(
            configuration.GetSection<SocketChannelConfigurationSection>());

        _channel = new TcpSocketChannel(socket, innerConfigurationBuilder.Build(), status);

        _ctx = new ChannelHandlerContext(this);
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

    public IChannelRetryStrategy? RetryStrategy
    {
        get
        {
            return _configuration.RetryStrategy;
        }
    }

    public ChannelConfiguration Configuration
    {
        get
        {
            return _configuration;
        }
    }

    public async Task BindAsync(EndPoint endPoint)
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

    public Task CloseAsync()
    {
        return _channel.CloseAsync();
    }

    public void RequestRead(IByteBuffer? byteBuffer = null)
    {
        _channel.RequestRead(byteBuffer);
    }

    public Task StartAsync()
    {
        return _channel.StartAsync();
    }

    public Task StartAsync(int backlog)
    {
        return _channel.StartAsync(backlog);
    }

    public async Task StartAsync(EndPoint endPoint)
    {
        try
        {
            await _channel.StartAsync(endPoint)
                .ConfigureAwait(false);

            _endPoint = endPoint;
        }
        catch
        {
            bool success = await RetryAsync(endPoint);
            if (success)
            {
                _endPoint = endPoint;
                return;
            }

            throw;
        }
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

    private void RunExceptionHandler(Exception ex)
    {
        if (EventLoop.IsInEventLoop)
        {
            ExceptionHandler.HandleException(_ctx, ex);
            return;
        }

        EventLoop.StartNew(() => ExceptionHandler.HandleException(_ctx, ex));
    }

    private void RunLifecycleHandlerActive()
    {
        if (EventLoop.IsInEventLoop)
        {
            InvokeLifecycleActive();
            return;
        }

        EventLoop.StartNew(() => InvokeLifecycleActive());
    }

    private void RunLifecycleHandlerInactive()
    {
        if (EventLoop.IsInEventLoop)
        {
            InvokeLifecycleInactive();
            return;
        }

        EventLoop.StartNew(() => InvokeLifecycleInactive());
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

    private Task<bool> RunRetryAsync(EndPoint? endPoint)
    {
        if (EventLoop.IsInEventLoop)
        {
            return InvokeRetryAsync(endPoint);
        }

        return EventLoop.StartNew(() => InvokeRetryAsync(endPoint))
            .Unwrap();
    }

    private Task<bool> InvokeRetryAsync(EndPoint? endPoint)
    {
        _currentRetryCount = 0;
        return RetryAsync(endPoint);
    }

    private async Task<bool> RetryAsync(EndPoint? endPoint)
    {
        if (endPoint == null)
        {
            throw new InvalidOperationException("Call Bind() or Start() first");
        }

        IChannelRetryStrategy? strategy = _configuration.RetryStrategy;
        if (strategy == null)
        {
            return false;
        }

        double waitMilliseconds = strategy.HandleRetry(_currentRetryCount++);
        if (waitMilliseconds <= 0.0)
        {
            return false;
        }

        try
        {
            await Task.Delay(TimeSpan.FromMilliseconds(waitMilliseconds));

            await _channel.StartAsync(endPoint);

            _currentRetryCount = 0;

            return true;
        }
        catch
        {
            return await RetryAsync(endPoint);
        }
    }

    private class RetryableChannelLifecycleHandler : IChannelLifecycleHandler
    {
        private readonly RetryableTcpSocketChannel _parent;

        public RetryableChannelLifecycleHandler(RetryableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public void HandleChannelActive(IChannel channel)
        {
            _parent.RunLifecycleHandlerActive();
        }

        public void HandleChannelInactive(IChannel channel)
        {
            _parent.RunRetryAsync(_parent._endPoint)
                .ContinueWith((task) =>
                {
                    bool success = task.Result;
                    if (success)
                    {
                        return;
                    }

                    _parent.RunLifecycleHandlerInactive();
                });
        }
    }

    private class RetryableChannelExceptionHandler : IChannelExceptionHandler
    {
        private readonly RetryableTcpSocketChannel _parent;

        public RetryableChannelExceptionHandler(RetryableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public void HandleException(IChannelHandlerContext ctx, Exception ex)
        {
            _parent.RunExceptionHandler(ex);
        }
    }

    private class RetryableChannelInboundPipe : IChannelInboundPipe<IByteBuffer, Unit>
    {
        private readonly RetryableTcpSocketChannel _parent;

        public RetryableChannelInboundPipe(RetryableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public Result<ChannelPipeResultType, Unit> Transform(
            IChannelHandlerContext ctx,
            IByteBuffer message)
        {
            return _parent._pipeline.HandleRead(_parent._ctx, message);
        }
    }

    private class RetryableChannelOutboundPipe : IChannelOutboundPipe<object, IByteBuffer>
    {
        private readonly RetryableTcpSocketChannel _parent;

        public RetryableChannelOutboundPipe(RetryableTcpSocketChannel parent)
        {
            _parent = parent;
        }

        public Result<ChannelPipeResultType, IByteBuffer> Transform(
            IChannelHandlerContext ctx,
            object message)
        {
            return _parent._pipeline.HandleWrite(_parent._ctx, message);
        }

        Result<ChannelPipeResultType, IByteBuffer> IChannelOutboundPipe<object, IByteBuffer>.Transform(IChannelHandlerContext ctx, object message)
        {
            return Transform(ctx, message);
        }
    }
}
