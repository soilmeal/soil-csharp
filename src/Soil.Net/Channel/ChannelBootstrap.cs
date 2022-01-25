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
    private AddressFamily _addressFamily = System.Net.Sockets.AddressFamily.Unknown;

    private SocketType _socketType = System.Net.Sockets.SocketType.Unknown;

    private ProtocolType _protocolType = System.Net.Sockets.ProtocolType.Unknown;

    private readonly ChannelConfiguration.Builder _configurationBuilder = new();

    private bool _initialized = false;

    public ChannelBootstrap AddressFamily(AddressFamily addressFamily)
    {
        if (addressFamily != System.Net.Sockets.AddressFamily.InterNetwork
            && addressFamily != System.Net.Sockets.AddressFamily.InterNetworkV6)
        {
            throw new ArgumentOutOfRangeException(nameof(addressFamily), addressFamily, null);
        }

        _addressFamily = addressFamily;

        return this;
    }

    public ChannelBootstrap SocketType(SocketType socketType)
    {
        if (socketType != System.Net.Sockets.SocketType.Stream
            && socketType != System.Net.Sockets.SocketType.Dgram)
        {
            throw new ArgumentOutOfRangeException(nameof(socketType), socketType, null);
        }

        _socketType = socketType;

        return this;
    }

    public ChannelBootstrap ProtocolType(ProtocolType protocolType)
    {
        if (protocolType != System.Net.Sockets.ProtocolType.Tcp
            && protocolType != System.Net.Sockets.ProtocolType.Udp)
        {
            throw new ArgumentOutOfRangeException(nameof(protocolType), protocolType, null);
        }

        _protocolType = protocolType;

        return this;
    }

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

    public ChannelBootstrap ExceptionHandler(Action<IChannelHandlerContext, Exception> handler)
    {
        return ExceptionHandler(IChannelExceptionHandler.Create(handler));
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
        try
        {
            IChannel channel = Initalize();

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
        try
        {
            IChannel channel = Initalize();

            await channel.StartAsync(endPoint);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IChannel> StartAsync(IPAddress address, int port)
    {
        try
        {
            IChannel channel = Initalize();

            await channel.StartAsync(address, port);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IChannel> StartAsync(string host, int port)
    {
        try
        {
            IChannel channel = Initalize();

            await channel.StartAsync(host, port);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    private IChannel Initalize()
    {
        if (_initialized)
        {
            throw new InvalidOperationException("already initialized");
        }

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

        AddressFamily addressFamily = GetOrDefaultAddressFamily();
        SocketType socketType = GetOrDefaultSocketType();
        ProtocolType protocolType = GetOrDefaultProtocolType();

        IChannel? channel = SocketChannelFactory.Create(
            addressFamily,
            socketType,
            protocolType,
            _configurationBuilder.Build());
        if (channel == null)
        {
            throw new InvalidOperationException("not supported yet");
        }

        _initialized = true;

        return channel;
    }

    private AddressFamily GetOrDefaultAddressFamily()
    {
        return _addressFamily != System.Net.Sockets.AddressFamily.Unknown
            ? _addressFamily
            : System.Net.Sockets.AddressFamily.InterNetwork;
    }

    private SocketType GetOrDefaultSocketType()
    {
        return _socketType != System.Net.Sockets.SocketType.Unknown
            ? _socketType
            : System.Net.Sockets.SocketType.Stream;
    }

    private ProtocolType GetOrDefaultProtocolType()
    {
        return _protocolType != System.Net.Sockets.ProtocolType.Unknown
            ? _protocolType
            : System.Net.Sockets.ProtocolType.Tcp;
    }
}
