using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;
using System.Net.Sockets;
using System.Security.Cryptography.X509Certificates;
using Soil.Net.Sockets;
using Soil.ObjectPool;
using Soil.ObjectPool.Concurrent;

namespace Soil.Net.Channel.Configuration;

public class SocketChannelConfigurationSection : AbstractReadOnlyConfigurationSection<SocketChannelConfigurationSection>
{
    private readonly IObjectPool<SocketChannelAsyncEventArgs> _socketChannelAsyncEventArgsPool;

    private readonly SocketShutdown _shutdownHow;

    private readonly IReadOnlyDictionary<SocketOptionPair, SocketOptionValue> _socketOptions;

    private readonly HashSet<SocketError> _closeErrors;

    private readonly bool _invokeHandlerWhenCloseErrorCaught;

    public IObjectPool<SocketChannelAsyncEventArgs> SocketChannelEventArgsPool
    {
        get
        {
            return _socketChannelAsyncEventArgsPool;
        }
    }

    public SocketShutdown ShutdownHow
    {
        get
        {
            return _shutdownHow;
        }
    }

    public IReadOnlyDictionary<SocketOptionPair, SocketOptionValue> SocketOptions
    {
        get
        {
            return _socketOptions;
        }
    }

    public IReadOnlyCollection<SocketError> CloseErrors
    {
        get
        {
            return _closeErrors;
        }
    }

    public bool InvokeHandlerWhenCloseErrorCaught
    {
        get
        {
            return _invokeHandlerWhenCloseErrorCaught;
        }
    }

    protected override IReadOnlyDictionary<string, IReadOnlyChannelConfigurationSection> Sections
    {
        get
        {
            throw new NotSupportedException();
        }
    }

    public static void SetIfAbsent(ChannelConfiguration.Builder builder)
    {
        if (builder.TryGetSection<SocketChannelConfigurationSection>(out var section))
        {
            return;
        }

        builder.AddSection(new Builder()
            .SetChannelAsyncEventArgs(new TLSObjectPool<SocketChannelAsyncEventArgs>(
                new DefaultObjectPoolPolicy<SocketChannelAsyncEventArgs>()))
            .SetShutdownHow(SocketShutdown.Send)
            .Build());
    }

    private SocketChannelConfigurationSection(
        IObjectPool<SocketChannelAsyncEventArgs> socketChannelAsyncEventArgsPool,
        SocketShutdown shutdownHow,
        IReadOnlyDictionary<SocketOptionPair, SocketOptionValue> socketOptions,
        HashSet<SocketError> closeErrors,
        bool invokeHandlerWhenCloseErrorCaught)
    {
        _socketChannelAsyncEventArgsPool = socketChannelAsyncEventArgsPool;
        _shutdownHow = shutdownHow;
        _socketOptions = socketOptions;
        _closeErrors = closeErrors;
        _invokeHandlerWhenCloseErrorCaught = invokeHandlerWhenCloseErrorCaught;
    }

    public void ApplyAllSocketOption(Socket socket)
    {
        if (socket == null)
        {
            throw new ArgumentNullException(nameof(socket));
        }

        if (_socketOptions.Count <= 0)
        {
            return;
        }

        foreach (var (optionPair, optionValue) in _socketOptions)
        {
            switch (optionValue.Type)
            {
                case SocketOptionValueType.Int32:
                {
                    if (!optionValue.TryGetValue(out int value))
                    {
                        throw new InvalidOperationException("should not be reached here");
                    }

                    socket.SetSocketOption(optionPair.Level, optionPair.Name, value);
                    break;
                }
                case SocketOptionValueType.Bool:
                {
                    if (!optionValue.TryGetValue(out bool value))
                    {
                        throw new InvalidOperationException("should not be reached here");
                    }

                    socket.SetSocketOption(optionPair.Level, optionPair.Name, value);
                    break;
                }
                case SocketOptionValueType.Bytes:
                {

                    if (!optionValue.TryGetValue(out byte[]? value))
                    {
                        throw new InvalidOperationException("should not be reached here");
                    }

                    socket.SetSocketOption(optionPair.Level, optionPair.Name, value);
                    break;
                }
                case SocketOptionValueType.Object:
                {

                    if (!optionValue.TryGetValue(out object? value))
                    {
                        throw new InvalidOperationException("should not be reached here");
                    }

                    socket.SetSocketOption(optionPair.Level, optionPair.Name, value);
                    break;
                }
                default:
                {
                    throw new InvalidOperationException("should not be reached here");
                }
            }
        }
    }

    public bool ContainsInCloseError(SocketError socketError)
    {
        return _closeErrors.Contains(socketError);
    }

    public override TSection GetSection<TSection>()
    {
        throw new NotSupportedException();
    }

    public override bool TryGetSection<T>([NotNullWhen(true)] out T section)
    {
        throw new NotSupportedException();
    }

    public class Builder : AbstractChannelConfigurationSection<Builder>
    {
        private IObjectPool<SocketChannelAsyncEventArgs>? _socketChannelAsyncEventArgsPool;

        private SocketShutdown _shutdownHow = SocketShutdown.Both;

        private readonly Dictionary<SocketOptionPair, SocketOptionValue> _socketOptions = new();

        private HashSet<SocketError> _closeErrors = new();

        private bool _invokeHandlerWhenCloseErrorCaught = Constants.DefaultInvokeHandlerWhenCloseSocketErrorCaught;

        public IObjectPool<SocketChannelAsyncEventArgs>? SocketChannelEventArgsPool
        {
            get
            {
                return _socketChannelAsyncEventArgsPool;
            }
        }

        public SocketShutdown ShutdownHow
        {
            get
            {
                return _shutdownHow;
            }
        }

        public IReadOnlyDictionary<SocketOptionPair, SocketOptionValue> SocketOptions
        {
            get
            {
                return _socketOptions;
            }
        }

        public IReadOnlyCollection<SocketError> CloseErrors
        {
            get
            {
                return _closeErrors;
            }
        }

        public bool InvokeHandlerWhenCloseErrorCaught
        {
            get
            {
                return _invokeHandlerWhenCloseErrorCaught;
            }
        }

        protected override IDictionary<string, IReadOnlyChannelConfigurationSection> Sections
        {
            get
            {
                throw new NotSupportedException();
            }
        }

        public Builder SetChannelAsyncEventArgs(IObjectPool<SocketChannelAsyncEventArgs> channelAsyncEventArgs)
        {
            _socketChannelAsyncEventArgsPool = channelAsyncEventArgs ?? throw new ArgumentNullException(nameof(channelAsyncEventArgs));

            return this;
        }

        public Builder SetChannelAsyncEventArgs(int retainSize)
        {
            return SetChannelAsyncEventArgs(
                new DefaultObjectPoolPolicy<SocketChannelAsyncEventArgs>(),
                retainSize);
        }

        public Builder SetChannelAsyncEventArgs(
            IObjectPoolPolicy<SocketChannelAsyncEventArgs> policy,
            int retainSize)
        {
            if (policy == null)
            {
                throw new ArgumentNullException(nameof(policy));
            }

            _socketChannelAsyncEventArgsPool = retainSize > 0
                ? new TLSObjectPool<SocketChannelAsyncEventArgs>(policy, retainSize)
                : new TLSUnlimitedObjectPool<SocketChannelAsyncEventArgs>(policy);

            return this;
        }

        public Builder SetShutdownHow(SocketShutdown shutdownHow)
        {
            _shutdownHow = shutdownHow;

            return this;
        }

        public Builder SetSocketOption(
            SocketOptionLevel level,
            SocketOptionName name,
            int value)
        {
            if (!SocketOptionNameValidator.Validate(level, name))
            {
                throw new SocketException((int)SocketError.ProtocolOption);
            }

            var key = new SocketOptionPair(level, name);
            if (!_socketOptions.ContainsKey(key))
            {
                _socketOptions[key] = new SocketOptionValue();
            }

            SocketOptionValue optionValue = _socketOptions[key];
            optionValue.SetValue(value);
            return this;
        }

        public Builder SetSocketOption(
            SocketOptionLevel level,
            SocketOptionName name,
            bool value)
        {
            if (!SocketOptionNameValidator.Validate(level, name))
            {
                throw new SocketException((int)SocketError.ProtocolOption);
            }

            var key = new SocketOptionPair(level, name);
            if (!_socketOptions.ContainsKey(key))
            {
                _socketOptions[key] = new SocketOptionValue();
            }

            SocketOptionValue optionValue = _socketOptions[key];
            optionValue.SetValue(value);
            return this;
        }

        public Builder SetSocketOption(
            SocketOptionLevel level,
            SocketOptionName name,
            byte[] value)
        {
            if (!SocketOptionNameValidator.Validate(level, name))
            {
                throw new SocketException((int)SocketError.ProtocolOption);
            }

            var key = new SocketOptionPair(level, name);
            if (!_socketOptions.ContainsKey(key))
            {
                _socketOptions[key] = new SocketOptionValue();
            }

            SocketOptionValue optionValue = _socketOptions[key];
            optionValue.SetValue(value);
            return this;
        }

        public Builder SetSocketOption(
            SocketOptionLevel level,
            SocketOptionName name,
            object value)
        {
            if (!SocketOptionNameValidator.Validate(level, name))
            {
                throw new SocketException((int)SocketError.ProtocolOption);
            }

            var key = new SocketOptionPair(level, name);
            if (!_socketOptions.ContainsKey(key))
            {
                _socketOptions[key] = new SocketOptionValue();
            }

            SocketOptionValue optionValue = _socketOptions[key];
            optionValue.SetValue(value);
            return this;
        }

        public Builder AddCloseError(SocketError socketError)
        {
            _closeErrors.Add(socketError);

            return this;
        }

        public Builder SetCloseErrors(IEnumerable<SocketError> socketErrors)
        {
            if (socketErrors == null)
            {
                throw new ArgumentNullException(nameof(socketErrors));
            }

            _closeErrors = new HashSet<SocketError>(socketErrors);

            return this;
        }

        public Builder SetInvokeHandlerWhenCloseErrorCaught(bool invokeHandlerWhenCloseErrorCaught)
        {
            _invokeHandlerWhenCloseErrorCaught = invokeHandlerWhenCloseErrorCaught;

            return this;
        }

        public SocketChannelConfigurationSection Build()
        {
            IObjectPool<SocketChannelAsyncEventArgs> socketChannelAsyncEventArgsPool = _socketChannelAsyncEventArgsPool ?? throw new InvalidOperationException("set SocketChannelAsyncEventArgsPool first");

            HashSet<SocketError> closeErrors = _closeErrors.Count > 0
                ? _closeErrors
                : new HashSet<SocketError>(Constants.DefaultCloseSocketErrors);

            return new SocketChannelConfigurationSection(
                socketChannelAsyncEventArgsPool,
                _shutdownHow,
                _socketOptions,
                closeErrors,
                _invokeHandlerWhenCloseErrorCaught);
        }

        public override TSection GetSection<TSection>()
        {
            throw new NotSupportedException();
        }

        public override bool TryGetSection<TSection>([NotNullWhen(true)] out TSection section)
        {
            throw new NotSupportedException();
        }

        public override void AddSection(IReadOnlyChannelConfigurationSection section)
        {
            throw new NotSupportedException();
        }

        public override bool TryAddSection([NotNullWhen(true)] IReadOnlyChannelConfigurationSection section)
        {
            throw new NotSupportedException();
        }
    }
}
