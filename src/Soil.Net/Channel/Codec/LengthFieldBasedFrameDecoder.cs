using System;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel.Codec;

public class LengthFieldBasedFrameDecoder
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

    public Result<ChannelPipeResultType, IByteBuffer> Transform(
        IChannelHandlerContext _,
        IByteBuffer message)
    {
        long messageLength = message.ReadableBytes;
        if (messageLength < _lengthFieldLength)
        {
            return Result.Create(ChannelPipeResultType.ContinueIO, message);
        }

        long frameLength = _frameLengthExtractor(message);
        if (frameLength <= 0)
        {
            throw new ArgumentException("buffer frame length less than or equal to zero.", nameof(message));
        }

        if (message.ReadableBytes - _lengthFieldLength < frameLength)
        {
            return Result.Create(ChannelPipeResultType.ContinueIO, message);
        }

        message.DiscardReadBytes((int)frameLength);
        return Result.Create(ChannelPipeResultType.CallNext, message);
    }

    private long ExtractByteSizeFrameLength(IByteBuffer message)
    {
        return message.GetByte(0);
    }

    private long ExtractInt16SizeFrameLength(IByteBuffer message)
    {
        return message.GetInt16(0);
    }

    private long ExtractInt32SizeFrameLength(IByteBuffer message)
    {
        return message.GetInt32(0);
    }

    private long ExtractInt64SizeFrameLength(IByteBuffer message)
    {
        return message.GetInt64(0);
    }

    private long ThrowInvalidOperation(IByteBuffer message)
    {
        throw new InvalidOperationException("Should not reach here");
    }
}
