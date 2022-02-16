using System;
using System.Collections.Generic;
using System.Diagnostics;
using Soil.Buffers;
using Soil.Net.Event;

namespace Soil.Net.Channel.Configuration;

[DebuggerDisplay("Sections = {_sections}")]
public class ChannelConfiguration : AbstractReadOnlyConfigurationSection<ChannelConfiguration>
{
    private readonly IByteBufferAllocator _allocator;

    private readonly IEventLoopGroup _eventLoopGroup;

    private readonly IChannelInitHandler? _initHandler;

    private readonly IChannelReconnectStrategy? _reconnectStrategy;

    private readonly IChannelReconnectHandler? _reconnectHandler;

    private readonly bool _autoRequest;

    private readonly IChannelIdGenerator _idGenerator;

    private readonly IReadOnlyDictionary<string, IReadOnlyChannelConfigurationSection> _sections;

    public IByteBufferAllocator Allocator
    {
        get
        {
            return _allocator;
        }
    }

    public IEventLoopGroup EventLoopGroup
    {
        get
        {
            return _eventLoopGroup;
        }
    }

    public IChannelInitHandler? InitHandler
    {
        get
        {
            return _initHandler;
        }
    }

    public IChannelReconnectStrategy? ReconnectStrategy
    {
        get
        {
            return _reconnectStrategy;
        }
    }

    public IChannelReconnectHandler? ReconnectHandler
    {
        get
        {
            return _reconnectHandler;
        }
    }

    public bool AutoRequest
    {
        get
        {
            return _autoRequest;
        }
    }

    public IChannelIdGenerator IdGenerator
    {
        get
        {
            return _idGenerator;
        }
    }

    protected override IReadOnlyDictionary<string, IReadOnlyChannelConfigurationSection> Sections
    {
        get
        {
            return _sections;
        }
    }
    private ChannelConfiguration(
        IByteBufferAllocator allocator,
        IEventLoopGroup eventLoopGroup,
        IChannelInitHandler? initHandler,
        IChannelReconnectStrategy? reconnectStrategy,
        IChannelReconnectHandler? reconnectHandler,
        bool autoRequest,
        IChannelIdGenerator idGenerator,
        IReadOnlyDictionary<string, IReadOnlyChannelConfigurationSection> sections)
    {
        _allocator = allocator;
        _eventLoopGroup = eventLoopGroup;
        _initHandler = initHandler;
        _reconnectStrategy = reconnectStrategy;
        _reconnectHandler = reconnectHandler;
        _autoRequest = autoRequest;
        _idGenerator = idGenerator;
        _sections = sections;
    }


    [DebuggerDisplay("Sections = {_sections}")]
    [DebuggerDisplay("Allocator = {_allocator}")]
    [DebuggerDisplay("EventLoopGroup = {_eventLoopGroup}")]
    [DebuggerDisplay("InitHandler = {_initHandler}")]
    [DebuggerDisplay("LifecycleHandler = {_lifecycleHandler}")]
    [DebuggerDisplay("ExceptionHandler = {_exceptionHandler}")]
    public class Builder : AbstractChannelConfigurationSection<Builder>
    {
        private IByteBufferAllocator? _allocator;

        private IEventLoopGroup? _eventLoopGroup;

        private IChannelInitHandler? _initHandler;

        private IChannelReconnectStrategy? _reconnectStrategy;

        private IChannelReconnectHandler? _reconnectHandler;

        private bool _autoRequest = true;

        private IChannelIdGenerator? _idGenerator;

        private readonly Dictionary<string, IReadOnlyChannelConfigurationSection> _sections = new();

        public IByteBufferAllocator? Allocator
        {
            get
            {
                return _allocator;
            }
        }

        public IEventLoopGroup? EventLoopGroup
        {
            get
            {
                return _eventLoopGroup;
            }
        }

        public IChannelInitHandler? InitHandler
        {
            get
            {
                return _initHandler;
            }
        }

        public IChannelReconnectStrategy? ReconnectStrategy
        {
            get
            {
                return _reconnectStrategy;
            }
        }

        public IChannelReconnectHandler? ReconnectHandler
        {
            get
            {
                return _reconnectHandler;
            }
        }

        public bool AutoRequest
        {
            get
            {
                return _autoRequest;
            }
        }

        public IChannelIdGenerator? IdGenerator
        {
            get
            {
                return _idGenerator;
            }
        }

        protected override IDictionary<string, IReadOnlyChannelConfigurationSection> Sections
        {
            get
            {
                return _sections;
            }
        }

        public Builder()
        {
        }

        public Builder SetAllocator(IByteBufferAllocator allocator)
        {
            _allocator = allocator ?? throw new ArgumentNullException(nameof(allocator));

            return this;
        }

        public Builder SetEventLoopGroup(IEventLoopGroup eventLoopGroup)
        {
            _eventLoopGroup = eventLoopGroup ?? throw new ArgumentNullException(nameof(eventLoopGroup));

            return this;
        }

        public Builder SetInitHandler(IChannelInitHandler initHandler)
        {
            _initHandler = initHandler ?? throw new ArgumentNullException(nameof(initHandler));

            return this;
        }

        public Builder SetReconnectStrategy(IChannelReconnectStrategy reconnectStrategy)
        {
            _reconnectStrategy = reconnectStrategy ?? throw new ArgumentNullException(nameof(reconnectStrategy));

            return this;
        }

        public Builder SetReconnectHandler(IChannelReconnectHandler reconnectHandler)
        {
            _reconnectHandler = reconnectHandler ?? throw new ArgumentNullException(nameof(reconnectHandler));

            return this;
        }

        public Builder SetAutoRequest(bool autoRequest)
        {
            _autoRequest = autoRequest;

            return this;
        }

        public Builder SetIdGenerator(IChannelIdGenerator idGenerator)
        {
            _idGenerator = idGenerator ?? throw new ArgumentNullException(nameof(idGenerator));

            return this;
        }

        public ChannelConfiguration Build()
        {
            IByteBufferAllocator allocator = _allocator ?? throw new InvalidOperationException("set Allocator first");
            IEventLoopGroup eventLoopGroup = _eventLoopGroup ?? throw new InvalidOperationException("set EventLoopGroup first");
            IChannelIdGenerator idGenerator = _idGenerator ?? new DefaultChannelIdGenerator();

            return new ChannelConfiguration(
                allocator,
                eventLoopGroup,
                _initHandler,
                _reconnectStrategy,
                _reconnectHandler,
                _autoRequest,
                idGenerator,
                _sections);
        }
    }
}
