using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

public class DefaultChannelPipeline : IChannelPipeline<IByteBuffer>
{
    private static readonly DefaultChannelPipeline _instance = new();

    public static DefaultChannelPipeline Instance
    {
        get
        {
            return _instance;
        }
    }

    public Result<ChannelPipeResultType, Unit> HandleRead(
        IChannelHandlerContext ctx,
        IByteBuffer byteBuffer)
    {
        return Constants.DefaultInboundPipe.Transform(ctx, byteBuffer);
    }

    public Result<ChannelPipeResultType, IByteBuffer> HandleWrite(
        IChannelHandlerContext ctx,
        IByteBuffer message)
    {
        return Constants.DefaultOutboundPipe.Transform(ctx, message);
    }
}
