using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

internal class ChannelPipeline<TOutMsg> : IChannelPipeline<TOutMsg>
    where TOutMsg : class
{

    private readonly IChannelInboundPipe<IByteBuffer, Unit> _inboundPipe;

    private readonly IChannelOutboundPipe<TOutMsg, IByteBuffer> _outboundPipe;

    internal ChannelPipeline(
        IChannelInboundPipe<IByteBuffer, Unit> inboundPipe,
        IChannelOutboundPipe<TOutMsg, IByteBuffer> outboundPipe)
    {
        _inboundPipe = inboundPipe;
        _outboundPipe = outboundPipe;
    }

    public Result<ChannelPipeResultType, Unit> HandleRead(
        IChannelHandlerContext ctx,
        IByteBuffer byteBuffer)
    {
        return _inboundPipe.Transform(ctx, byteBuffer);
    }

    public Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        TOutMsg message)
    {
        return _outboundPipe.Transform(ctx, message);
    }

    public Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        object message)
    {
        return ((IChannelPipeline<TOutMsg>)this).HandleWrite(ctx, (TOutMsg)message);
    }
}
