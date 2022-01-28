using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;
using Soil.Threading;
using Soil.Types;

namespace Soil.Net.Channel;

public class ChannelBootstrap
{
    private readonly ChannelConfiguration.Builder _configurationBuilder = new();

    public ChannelBootstrap Allocator(IByteBufferAllocator allocator)
    {
        _configurationBuilder.SetAllocator(allocator);

        return this;
    }

    public ChannelBootstrap Allocator(int bufferArrayPoolBucketSize, int byteBufferRetainSize)
    {
        return Allocator(new PooledByteBufferAllocator(
            bufferArrayPoolBucketSize,
            byteBufferRetainSize));
    }

    public ChannelBootstrap Allocator(
        int maxCapacityHint,
        int bufferArrayBucketSize,
        int byteBufferRetainSize)
    {
        return Allocator(new PooledByteBufferAllocator(
            maxCapacityHint,
            bufferArrayBucketSize,
            byteBufferRetainSize));
    }

    public ChannelBootstrap EventLoopGroup(IEventLoopGroup eventLoopGroup)
    {
        _configurationBuilder.SetEventLoopGroup(eventLoopGroup);

        return this;
    }

    public ChannelBootstrap EventLoopGroup(int maxConcurrencyCount)
    {
        if (maxConcurrencyCount < 0)
        {
            maxConcurrencyCount = Environment.ProcessorCount;
        }

        IThreadFactory threadFactory = new IThreadFactory.Builder()
                .SetPriority(System.Threading.ThreadPriority.Normal)
                .Build(new ThreadNameFormatter("event-loop-{0}"));
        IEventLoopGroup eventLoopGroup = maxConcurrencyCount == 1
            ? new SingleThreadEventLoopGroup(threadFactory)
            : new MultiThreadEventLoopGroup(maxConcurrencyCount, threadFactory);
        return EventLoopGroup(eventLoopGroup);
    }

    public ChannelBootstrap LifecycleHandler(IChannelLifecycleHandler lifecycleHandler)
    {
        _configurationBuilder.SetLifecycleHandler(lifecycleHandler);

        return this;
    }

    public ChannelBootstrap LifecycleHandler(
        Action<IChannel> activeAction,
        Action<IChannel> inactiveAction)
    {

        return LifecycleHandler(IChannelLifecycleHandler.Create(activeAction, inactiveAction));
    }

    public ChannelBootstrap ExceptionHandler(Action<IChannelHandlerContext, Exception> handler)
    {
        return ExceptionHandler(IChannelExceptionHandler.Create(handler));
    }

    public ChannelBootstrap ExceptionHandler(IChannelExceptionHandler exceptionHandler)
    {
        _configurationBuilder.SetExceptionHandler(exceptionHandler);

        return this;
    }

    public ChannelBootstrap Pipeline<TOutMsg>(
        IChannelInboundPipe<IByteBuffer, Unit> inboundPipe,
        IChannelOutboundPipe<TOutMsg, IByteBuffer> outboundPipe)
        where TOutMsg : class
    {
        return Pipeline(IChannelPipeline.Create(inboundPipe, outboundPipe));
    }

    public ChannelBootstrap Pipeline(IChannelInboundPipe<IByteBuffer, Unit> inboundPipe)
    {
        return Pipeline(inboundPipe, Constants.DefaultOutboundPipe);
    }

    public ChannelBootstrap Pipeline(Action<IChannelHandlerContext, IByteBuffer> inboundPipe)
    {
        return Pipeline(
            ChannelInboundHandler.Create(inboundPipe),
            Constants.DefaultOutboundPipe);
    }

    public ChannelBootstrap Pipeline(IChannelPipeline pipeline)
    {
        _configurationBuilder.SetPipeline(pipeline);

        return this;
    }

    public ChannelBootstrap AutoRequest(bool autoRequest)
    {
        _configurationBuilder.SetAutoRequest(autoRequest);

        return this;
    }

    public ChannelBootstrap RetryStrategy(int maxRetryCount)
    {
        return RetryStrategy(maxRetryCount, Constants.DefaultWaitMillisecondsBeforeRetry);
    }

    public ChannelBootstrap RetryStrategy(int maxRetryCount, double waitMillisecondsBeforeRetry)
    {
        return RetryStrategy(new ChannelMaxRetryCountStrategy(
            maxRetryCount,
            waitMillisecondsBeforeRetry));
    }

    public ChannelBootstrap RetryStrategy(IChannelRetryStrategy retryStrategy)
    {
        _configurationBuilder.SetRetryStrategy(retryStrategy);

        return this;
    }

    public ChannelBootstrap AddSection(IReadOnlyChannelConfigurationSection section)
    {
        _configurationBuilder.AddSection(section);

        return this;
    }

    public bool TryAddSection(IReadOnlyChannelConfigurationSection section)
    {
        return _configurationBuilder.TryAddSection(section);
    }

    public TSection GetSection<TSection>()
        where TSection : IReadOnlyChannelConfigurationSection
    {
        return _configurationBuilder.GetSection<TSection>();
    }

    public bool TryGetSection<T>([NotNullWhen(true)] out T section)
        where T : IReadOnlyChannelConfigurationSection
    {
        return _configurationBuilder.TryGetSection(out section);
    }

    public async Task<IChannel> BindAsync(EndPoint endPoint)
    {
        if (endPoint == null)
        {
            throw new ArgumentNullException(nameof(endPoint));
        }

        try
        {
            IChannel channel = Initalize(endPoint.AddressFamily);

            await channel.BindAsync(endPoint);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IChannel> StartAsync(EndPoint endPoint)
    {
        if (endPoint == null)
        {
            throw new ArgumentNullException(nameof(endPoint));
        }

        try
        {
            IChannel channel = Initalize(endPoint.AddressFamily);

            await channel.StartAsync(endPoint);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    public Task<IChannel> StartAsync(IPAddress address, int port)
    {
        return StartAsync(new IPEndPoint(address, port));
    }

    public Task<IChannel> StartAsync(string host, int port)
    {
        EndPoint endPoint = IPAddress.TryParse(host, out var address)
            ? new IPEndPoint(address, port)
            : new DnsEndPoint(host, port);

        return StartAsync(endPoint);
    }

    private IChannel Initalize(AddressFamily addressFamily)
    {
        if (_configurationBuilder.Allocator == null)
        {
            IByteBufferAllocator defaultAllocator = new PooledByteBufferAllocator();
            _configurationBuilder.SetAllocator(defaultAllocator);
        }

        if (_configurationBuilder.EventLoopGroup == null)
        {
            _configurationBuilder.SetEventLoopGroup(new MultiThreadEventLoopGroup(
                1,
                IThreadFactory.Builder.BuildDefault()));
        }

        if (_configurationBuilder.InitHandler == null)
        {
            _configurationBuilder.SetInitHandler(Constants.DefaultInitHandler);
        }

        if (_configurationBuilder.LifecycleHandler == null)
        {
            _configurationBuilder.SetLifecycleHandler(Constants.DefaultLifecycleHandler);
        }

        if (_configurationBuilder.ExceptionHandler == null)
        {
            _configurationBuilder.SetExceptionHandler(Constants.DefaultExceptionHandler);
        }

        SocketChannelConfigurationSection.SetIfAbsent(_configurationBuilder);

        ChannelConfiguration configuration = _configurationBuilder.Build();
        IChannelRetryStrategy? retryStrategy = configuration.RetryStrategy;

        IChannel channel = retryStrategy != null
            ? new RetryableTcpSocketChannel(addressFamily, configuration)
            : new TcpSocketChannel(addressFamily, configuration);

        return channel;
    }
}
