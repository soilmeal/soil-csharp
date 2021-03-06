using System;
using System.Diagnostics.CodeAnalysis;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;
using Soil.Threading;

namespace Soil.Net.Channel;

public class ServerChannelBootstrap
{
    private readonly ChannelConfiguration.Builder _masterConfigurationBuilder = new();

    private readonly ChannelConfiguration.Builder _childConfigurationBuilder = new();

    private IChannelLifecycleHandler _lifecycleHandler = Constants.DefaultLifecycleHandler;

    private IChannelExceptionHandler _exceptionHandler = Constants.DefaultExceptionHandler;

    public ServerChannelBootstrap()
    {
    }

    public ServerChannelBootstrap Allocator(IByteBufferAllocator allocator)
    {
        _masterConfigurationBuilder.SetAllocator(allocator);
        _childConfigurationBuilder.SetAllocator(allocator);

        return this;
    }

    public ServerChannelBootstrap Allocator(int bufferArrayPoolBucketSize, int byteBufferRetainSize)
    {
        return Allocator(new PooledByteBufferAllocator(
            bufferArrayPoolBucketSize,
            byteBufferRetainSize));
    }

    public ServerChannelBootstrap Allocator(
        int maxCapacityHint,
        int bufferArrayPoolBucketSize,
        int byteBufferRetainSize)
    {
        return Allocator(new PooledByteBufferAllocator(
            maxCapacityHint,
            bufferArrayPoolBucketSize,
            byteBufferRetainSize));
    }

    public ServerChannelBootstrap EventLoopGroup(IEventLoopGroup eventLoopGroup)
    {
        return EventLoopGroup(eventLoopGroup, eventLoopGroup);
    }

    public ServerChannelBootstrap EventLoopGroup(IEventLoopGroup master, IEventLoopGroup child)
    {
        _masterConfigurationBuilder.SetEventLoopGroup(master);
        _childConfigurationBuilder.SetEventLoopGroup(child);

        return this;
    }

    public ServerChannelBootstrap EventLoopGroup(int maxConcurrencyCount)
    {

        return EventLoopGroup(maxConcurrencyCount, maxConcurrencyCount);
    }

    public ServerChannelBootstrap EventLoopGroup(int masterMaxConcurrencyCount, int childMaxConcurrencyCount)
    {
        if (masterMaxConcurrencyCount < 0)
        {
            masterMaxConcurrencyCount = Environment.ProcessorCount;
        }
        if (childMaxConcurrencyCount < 0)
        {
            childMaxConcurrencyCount = Environment.ProcessorCount;
        }
        IThreadFactory masterThreadFactory = new IThreadFactory.Builder()
                .SetPriority(System.Threading.ThreadPriority.Normal)
                .Build(new ThreadNameFormatter("master-event-loop-{0}"));
        IEventLoopGroup masterEventLoopGroup = masterMaxConcurrencyCount == 1
            ? new SingleThreadEventLoopGroup(masterThreadFactory)
            : new MultiThreadEventLoopGroup(masterMaxConcurrencyCount, masterThreadFactory);

        IThreadFactory childThreadFactory = new IThreadFactory.Builder()
            .SetPriority(System.Threading.ThreadPriority.Normal)
            .Build(new ThreadNameFormatter("child-event-loop-{0}"));
        IEventLoopGroup childEventLoopGroup = childMaxConcurrencyCount == 1
            ? new SingleThreadEventLoopGroup(childThreadFactory)
            : new MultiThreadEventLoopGroup(childMaxConcurrencyCount, childThreadFactory);

        return EventLoopGroup(masterEventLoopGroup, childEventLoopGroup);
    }

    public ServerChannelBootstrap InitHandler(IChannelInitHandler initHandler)
    {
        _masterConfigurationBuilder.SetInitHandler(initHandler);

        return this;
    }

    public ServerChannelBootstrap InitHandler(Func<IChannel, IChannel> initHandlerFunc)
    {
        return InitHandler(IChannelInitHandler.Create(initHandlerFunc));
    }

    public ServerChannelBootstrap LifecycleHandler(IChannelLifecycleHandler lifecycleHandler)
    {
        _lifecycleHandler = lifecycleHandler ?? throw new ArgumentNullException(nameof(lifecycleHandler));

        return this;
    }

    public ServerChannelBootstrap ExceptionHandler(IChannelExceptionHandler exceptionHandler)
    {
        _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));

        return this;
    }

    public ServerChannelBootstrap AutoRequest(bool autoRequest)
    {
        return AutoRequest(autoRequest, autoRequest);
    }

    public ServerChannelBootstrap AutoRequest(bool master, bool child)
    {
        _masterConfigurationBuilder.SetAutoRequest(master);
        _childConfigurationBuilder.SetAutoRequest(child);

        return this;
    }

    public ServerChannelBootstrap AddMasterSection(IReadOnlyChannelConfigurationSection section)
    {
        _masterConfigurationBuilder.AddSection(section);

        return this;
    }

    public bool TryAddMasterSection(IReadOnlyChannelConfigurationSection section)
    {
        return _masterConfigurationBuilder.TryAddSection(section);
    }

    public TSection GetMasterSection<TSection>()
        where TSection : IReadOnlyChannelConfigurationSection
    {
        return _masterConfigurationBuilder.GetSection<TSection>();
    }

    public bool TryGetMasterSection<T>([NotNullWhen(true)] out T section)
        where T : IReadOnlyChannelConfigurationSection
    {
        return _masterConfigurationBuilder.TryGetSection(out section);
    }

    public ServerChannelBootstrap AddChildSection(IReadOnlyChannelConfigurationSection section)
    {
        _childConfigurationBuilder.AddSection(section);

        return this;
    }

    public bool TryAddChildSection(IReadOnlyChannelConfigurationSection section)
    {
        return _childConfigurationBuilder.TryAddSection(section);
    }

    public TSection GetChildSection<TSection>()
        where TSection : IReadOnlyChannelConfigurationSection
    {
        return _childConfigurationBuilder.GetSection<TSection>();
    }

    public bool TryGetChildSection<T>([NotNullWhen(true)] out T section)
        where T : IReadOnlyChannelConfigurationSection
    {
        return _childConfigurationBuilder.TryGetSection(out section);
    }

    public async Task<IServerChannel> BindAsync(EndPoint endPoint)
    {
        if (endPoint == null)
        {
            throw new ArgumentNullException(nameof(endPoint));
        }

        try
        {
            IServerChannel channel = Initialize(endPoint.AddressFamily);

            await channel.BindAsync(endPoint);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    public async Task<IServerChannel> StartAsync(EndPoint endPoint, int backlog)
    {
        if (endPoint == null)
        {
            throw new ArgumentNullException(nameof(endPoint));
        }

        try
        {
            IServerChannel channel = Initialize(endPoint.AddressFamily);

            await channel.StartAsync(endPoint, backlog);

            return channel;
        }
        catch
        {
            throw;
        }
    }

    public Task<IServerChannel> StartAsync(IPAddress address, int port, int backlog)
    {
        return StartAsync(new IPEndPoint(address, port), backlog);
    }

    public Task<IServerChannel> StartAsync(string host, int port, int backlog)
    {
        EndPoint endPoint = IPAddress.TryParse(host, out var address)
            ? new IPEndPoint(address, port)
            : new DnsEndPoint(host, port);

        return StartAsync(endPoint, backlog);
    }

    private IServerChannel Initialize(AddressFamily addressFamily)
    {
        if (_masterConfigurationBuilder.Allocator == null)
        {
            IByteBufferAllocator defaultAllocator = new PooledByteBufferAllocator();
            _masterConfigurationBuilder.SetAllocator(defaultAllocator);
            _childConfigurationBuilder.SetAllocator(defaultAllocator);
        }

        if (_masterConfigurationBuilder.EventLoopGroup == null)
        {
            _masterConfigurationBuilder.SetEventLoopGroup(new MultiThreadEventLoopGroup(
                1,
                IThreadFactory.Builder.BuildDefault()));
        }
        if (_childConfigurationBuilder.EventLoopGroup == null)
        {
            _childConfigurationBuilder.SetEventLoopGroup(new MultiThreadEventLoopGroup(
                Environment.ProcessorCount,
                IThreadFactory.Builder.BuildDefault()));
        }

        if (_masterConfigurationBuilder.InitHandler == null)
        {
            _masterConfigurationBuilder.SetInitHandler(Constants.DefaultInitHandler);
        }

        IChannelIdGenerator idGenerator = new DefaultChannelIdGenerator();
        _masterConfigurationBuilder.SetIdGenerator(idGenerator);
        _childConfigurationBuilder.SetIdGenerator(idGenerator);

        SocketChannelConfigurationSection.SetIfAbsent(_masterConfigurationBuilder);
        SocketChannelConfigurationSection.SetIfAbsent(_childConfigurationBuilder);

        return new TcpSocketServerChannel(
            addressFamily,
            _masterConfigurationBuilder.Build(),
            _childConfigurationBuilder.Build())
        {
            HandlerSet = new IChannelHandlerSet.Builder<IByteBuffer>()
                .SetLifecycleHandler(_lifecycleHandler)
                .SetExceptionHandler(_exceptionHandler)
                .SetOutboundPipe(Constants.DefaultOutboundPipe)
                .Build(),
        };
    }
}
