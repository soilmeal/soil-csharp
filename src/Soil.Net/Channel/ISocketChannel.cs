using System.Net.Sockets;

namespace Soil.Net.Channel;

public interface ISocketChannel : IChannel
{
    Socket Socket { get; }

    SocketShutdown ShutdownHow { get; }
}
