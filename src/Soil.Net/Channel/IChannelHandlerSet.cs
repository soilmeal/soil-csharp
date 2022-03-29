using System;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

public interface IChannelHandlerSet : IChannelLifecycleHandler, IChannelExceptionHandler
{
    Task<Unit?> HandleReadAsync(IChannelHandlerContext ctx, IByteBuffer byteBuffer);

    Task<IByteBuffer> HandleWriteAsync(IChannelHandlerContext ctx, object message);

    public class Builder<TOutMsg>
        where TOutMsg : class
    {
        private IChannelLifecycleHandler _lifecycleHandler = Constants.DefaultLifecycleHandler;

        private IChannelExceptionHandler _exceptionHandler = Constants.DefaultExceptionHandler;

        private IChannelInboundPipe<IByteBuffer, Unit> _inboundPipe = Constants.DefaultInboundPipe;

        private IChannelOutboundPipe<TOutMsg, IByteBuffer>? _outboundPipe;

        public IChannelLifecycleHandler LifecycleHandler
        {
            get
            {
                return _lifecycleHandler;
            }
        }

        public IChannelExceptionHandler ExceptionHandler
        {
            get
            {
                return _exceptionHandler;
            }
        }

        public IChannelInboundPipe<IByteBuffer, Unit> InboundPipe
        {
            get
            {
                return _inboundPipe;
            }
        }

        public IChannelOutboundPipe<TOutMsg, IByteBuffer>? OutboundPipe
        {
            get
            {
                return _outboundPipe;
            }
        }

        public Builder()
        {
        }

        public Builder<TOutMsg> SetLifecycleHandler(IChannelLifecycleHandler lifecycleHandler)
        {
            _lifecycleHandler = lifecycleHandler ?? throw new ArgumentNullException(nameof(lifecycleHandler));

            return this;
        }

        public Builder<TOutMsg> SetExceptionHandler(IChannelExceptionHandler exceptionHandler)
        {
            _exceptionHandler = exceptionHandler ?? throw new ArgumentNullException(nameof(exceptionHandler));

            return this;
        }

        public Builder<TOutMsg> SetInboundPipe(IChannelInboundPipe<IByteBuffer, Unit> inboundPipe)
        {
            _inboundPipe = inboundPipe ?? throw new ArgumentNullException(nameof(inboundPipe));

            return this;
        }

        public Builder<TOutMsg> SetOutboundPipe(IChannelOutboundPipe<TOutMsg, IByteBuffer> outboundPipe)
        {
            _outboundPipe = outboundPipe ?? throw new ArgumentNullException(nameof(outboundPipe));

            return this;
        }

        public IChannelHandlerSet<TOutMsg> Build()
        {
            IChannelOutboundPipe<TOutMsg, IByteBuffer> outboundPipe = _outboundPipe ?? throw new InvalidOperationException("OutboundPipe is null");

            return new ChannelHandlerSet<TOutMsg>(
                _lifecycleHandler,
                _exceptionHandler,
                _inboundPipe,
                outboundPipe);
        }
    }
}

public interface IChannelHandlerSet<TOutMsg> : IChannelHandlerSet
    where TOutMsg : class
{
    Task<IByteBuffer> HandleWriteAsync(IChannelHandlerContext ctx, TOutMsg message);

    Task<IByteBuffer> IChannelHandlerSet.HandleWriteAsync(
        IChannelHandlerContext ctx,
        object message)
    {
        if (message is not TOutMsg castedMessage)
        {
            throw new ArgumentException($"not supported type - typename={message.GetType().FullName}");
        }

        return HandleWriteAsync(ctx, castedMessage);
    }
}
