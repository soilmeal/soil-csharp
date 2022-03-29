using System;
using System.Threading.Tasks;
using Soil.Buffers;

namespace Soil.Net.Channel.Codec;

public class LengthFieldBasedFrameDecoder : IChannelInboundPipe<IByteBuffer, IByteBuffer>
{
    private readonly int _lengthFieldLength;

    private readonly Func<IByteBuffer, long> _frameLengthExtractor;

    public LengthFieldBasedFrameDecoder()
        : this(4)
    {
    }

    public LengthFieldBasedFrameDecoder(int lengthFieldLength)
    {
        if (lengthFieldLength != 1
            && lengthFieldLength != 2
            && lengthFieldLength != 4
            && lengthFieldLength != 8)
        {
            throw new ArgumentOutOfRangeException(
                nameof(lengthFieldLength),
                lengthFieldLength,
                null);
        }

        _lengthFieldLength = lengthFieldLength;

        _frameLengthExtractor = lengthFieldLength switch
        {
            1 => ExtractByteSizeFrameLength,
            2 => ExtractInt16SizeFrameLength,
            4 => ExtractInt32SizeFrameLength,
            8 => ExtractInt64SizeFrameLength,
            _ => ThrowInvalidOperation,
        };
    }

    public IChannelInboundPipe<IByteBuffer, TNewMessage> Connect<TNewMessage>(
        IChannelInboundPipe<IByteBuffer, TNewMessage> other)
        where TNewMessage : class
    {
        return IChannelInboundPipe.Connect(this, other);
    }

    public Task<IByteBuffer?> TransformAsync(IChannelHandlerContext ctx, IByteBuffer message)
    {
        return ctx.EventLoop.StartNew(() => DoTransform(ctx, message));
    }

    private IByteBuffer? DoTransform(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long messageLength = message.ReadableBytes;
        if (messageLength < _lengthFieldLength)
        {
            return null;
        }

        long frameLength = _frameLengthExtractor.Invoke(message);
        if (frameLength <= 0)
        {
            throw new ArgumentException("buffer frame length less than or equal to zero.", nameof(message));
        }

        if (message.ReadableBytes - _lengthFieldLength < frameLength)
        {
            return null;
        }

        message.DiscardReadBytes(_lengthFieldLength);

        // DO NOT CALL IByteBuffer.WriteBytes() because that method does not change readIdx
        IByteBuffer result = ctx.Allocator.Allocate((int)frameLength);
        result.WriteBytes(message, (int)frameLength);

        return result;
    }

    private long ExtractByteSizeFrameLength(IByteBuffer message)
    {
        return message.GetByte(message.ReadIndex);
    }

    private long ExtractInt16SizeFrameLength(IByteBuffer message)
    {
        return message.GetInt16(message.ReadIndex);
    }

    private long ExtractInt32SizeFrameLength(IByteBuffer message)
    {
        return message.GetInt32(message.ReadIndex);
    }

    private long ExtractInt64SizeFrameLength(IByteBuffer message)
    {
        return message.GetInt64(message.ReadIndex);
    }

    private long ThrowInvalidOperation(IByteBuffer message)
    {
        throw new InvalidOperationException("Should not reach here");
    }
}
