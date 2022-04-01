using System;
using System.Threading.Tasks;
using Soil.Buffers;

namespace Soil.Net.Channel.Codec;

public class LengthFieldPrepender : IChannelOutboundPipe<IByteBuffer, IByteBuffer>
{
    private readonly Func<IChannelHandlerContext, IByteBuffer, IByteBuffer> _lengthBufferGenerator;

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

        _lengthBufferGenerator = lengthFieldLength switch
        {
            1 => GenerateByteSizeLengthBuffer,
            2 => GenerateInt16SizeLengthBuffer,
            4 => GenerateInt32SizeLengthBuffer,
            8 => GenerateInt64SizeLengthBuffer,
            _ => ThrowInvalidOperation,
        };
    }

    public Task<IByteBuffer> TransformAsync(IChannelHandlerContext ctx, IByteBuffer message)
    {
        return ctx.EventLoop.StartNew(() => DoTransform(ctx, message));
    }

    public IChannelOutboundPipe<IByteBuffer, TNewMessage> Connect<TNewMessage>(
        IChannelOutboundPipe<IByteBuffer, TNewMessage> other)
        where TNewMessage : class
    {
        return IChannelOutboundPipe.Connect(this, other);
    }

    private IByteBuffer DoTransform(IChannelHandlerContext ctx, IByteBuffer message)
    {
        IByteBuffer lengthByteBuffer = _lengthBufferGenerator.Invoke(ctx, message);

        return ctx.Allocator.CompositeByteBuffer()
            .AddComponent(true, lengthByteBuffer)
            .AddComponent(true, message);
    }

    private IByteBuffer GenerateByteSizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > byte.MaxValue)
        {
            throw new ArgumentException(
                $"buffer length does not fit into a byte. length: {length}",
                nameof(message));
        }

        return ctx.Allocator.Allocate(1)
            .WriteByte((byte)length);
    }

    private IByteBuffer GenerateInt16SizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > short.MaxValue)
        {
            throw new ArgumentException(
                $"buffer length does not fit into a short. length: {length}",
                nameof(message));
        }

        return ctx.Allocator.Allocate(2)
            .WriteInt16((short)length);
    }

    private IByteBuffer GenerateInt32SizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > int.MaxValue)
        {
            throw new ArgumentException(
                $"buffer length does not fit into a int. length: {length}",
                nameof(message));
        }

        return ctx.Allocator.Allocate(4)
            .WriteInt32((int)length);
    }

    private IByteBuffer GenerateInt64SizeLengthBuffer(IChannelHandlerContext ctx, IByteBuffer message)
    {
        long length = message.ReadableBytes;
        if (length > long.MaxValue)
        {
            throw new ArgumentException(
                $"buffer length does not fit into a long. length: {length}",
                nameof(message));
        }

        return ctx.Allocator.Allocate(8)
            .WriteInt64(length);
    }

    private IByteBuffer ThrowInvalidOperation(IChannelHandlerContext ctx, IByteBuffer message)
    {
        throw new InvalidOperationException("Should not reach here");
    }
}
