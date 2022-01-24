using System.Collections.Generic;
using System.Net.Sockets;
using Soil.Buffers;
using Soil.Types;

namespace Soil.Net.Channel;

public class Constants
{
    public const int DefaultBacklog = 1024;

    public static readonly IChannelInitHandler DefaultInitHandler = IChannelInitHandler.Create((child) => child);

    public static readonly IChannelInboundPipe<IByteBuffer, Unit> DefaultInboundPipe = IChannelInboundPipe.Create<IByteBuffer, Unit>((_, byteBuffer) =>
    {
        byteBuffer.Release();
        return Result.Create(ChannelPipeResultType.CallNext, Unit.Instance);
    });

    public static readonly IChannelOutboundPipe<IByteBuffer, IByteBuffer> DefaultOutboundPipe = IChannelOutboundPipe.Create<IByteBuffer, IByteBuffer>((_, message) => Result.Create(ChannelPipeResultType.CallNext, message));

    public static readonly IChannelLifecycleHandler DefaultLifecycleHandler = IChannelLifecycleHandler.Create((_) => { }, (_) => { });

    public static readonly IChannelExceptionHandler DefaultExceptionHandler = IChannelExceptionHandler.Create((_, _) => { });

    public static readonly HashSet<SocketError> DefaultCloseSocketErrors = new()
    {
        SocketError.SocketError,
        SocketError.ConnectionAborted,
        SocketError.ConnectionReset,
        SocketError.NetworkDown,
        SocketError.NetworkReset,
        SocketError.Fault,
        SocketError.HostDown,
        SocketError.NoRecovery,
    };

    // TODO: create 'Soil.Util' and define 'DebugDefined' static method.
#if DEBUG
    public static readonly bool DefaultInvokeHandlerWhenCloseSocketErrorCaught = true;
#else
    public static readonly bool DefaultInvokeHandlerWhenCloseSocketErrorCaught = false;
#endif
}
