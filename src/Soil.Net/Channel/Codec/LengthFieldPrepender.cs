using System;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel.Codec;

public class LengthFieldPrepender : IChannelOutboundPipe<IByteBuffer, IByteBuffer>
{
    private readonly Func<IChannelHandlerContext, IByteBuffer, IByteBuffer> _lengtBufferGenerator;

    public LengthFieldPrepender()
        : this(4)
    {
    }

    public LengthFieldPrepender(int lengthFieldLength)
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

        _lengtBufferGenerator = lengthFieldLength switch
        {
            1 => GenerateByteSizeLengthBuffer,
            2 => GenerateInt16SizeLengthBuffer,
            4 => GenerateInt32SizeLengthBuffer,
            8 => GenerateInt64SizeLengthBuffer,
            _ => ThrowInvalidOperation,
        };
    }

    public Result<ChannelPipeResultType, IByteBuffer> Transform(
        IChannelHandlerContext ctx,
        IByteBuffer message)
    {
        IByteBuffer lengthByteBuffer = _lengtBufferGenerator(ctx, message);

        IByteBuffer result = ctx.Allocator.CompositeByteBuffer()
            .AddComponent(true, lengthByteBuffer)
            .AddComponent(true, message);

        return Result.Create(ChannelPipeResultType.CallNext, result);
    }


    private IByteBuffer GenerateByteSizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > byte.MaxValue)
        {
            throw new ArgumentException($"buffer length does not fit into a byte. length: {length}", nameof(message));
        }

        return ctx.Allocator.Allocate(1)
            .WriteByte((byte)length);
    }

    private IByteBuffer GenerateInt16SizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > short.MaxValue)
        {
            throw new ArgumentException($"buffer length does not fit into a short. length: {length}", nameof(message));
        }

        return ctx.Allocator.Allocate(2)
            .WriteInt16((short)length);
    }

    private IByteBuffer GenerateInt32SizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > int.MaxValue)
        {
            throw new ArgumentException($"buffer length does not fit into a int. length: {length}", nameof(message));
        }

        return ctx.Allocator.Allocate(4)
            .WriteInt32((int)length);
    }

    private IByteBuffer GenerateInt64SizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > long.MaxValue)
        {
            throw new ArgumentException($"buffer length does not fit into a long. length: {length}", nameof(message));
        }

        return ctx.Allocator.Allocate(8)
            .WriteInt64(length);
    }

    private IByteBuffer ThrowInvalidOperation(IChannelHandlerContext ctx, IByteBuffer message)
    {
        throw new InvalidOperationException("Should not reach here");
    }
}
