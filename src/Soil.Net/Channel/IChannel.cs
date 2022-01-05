using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
using Soil.Buffers;
using Soil.Net.Channel.Configuration;
using Soil.Net.Event;

namespace Soil.Net.Channel;

public interface IChannel
{
    ulong Id { get; }

    AddressFamily AddressFamily { get; }

    SocketType SocketType { get; }

    ProtocolType ProtocolType { get; }

    EndPoint? LocalEndPoint { get; }

    EndPoint? RemoteEndPoint { get; }

    bool IsBound { get; }

    bool Connected { get; }

    ChannelStatus Status { get; }

    IByteBufferAllocator Allocator { get; }

    IEventLoop EventLoop { get; }

    IChannelLifecycleHandler LifecycleHandler { get; }

    IChannelExceptionHandler ExceptionHandler { get; }

    IChannelPipeline Pipeline { get; set; }

    ChannelConfiguration Configuration { get; }

    Task BindAsync(EndPoint endPoint);

    Task StartAsync();

    Task StartAsync(int backlog);

    Task StartAsync(EndPoint endPoint);

    Task StartAsync(EndPoint endPoint, int backlog);

    Task StartAsync(IPAddress address, int port);

    Task StartAsync(IPAddress address, int port, int backlog);

    Task StartAsync(string host, int port);

    Task StartAsync(string host, int port, int backlog);

    void RequestRead(IByteBuffer? byteBuffer = null);

    Task<int> WriteAsync(object message);

    Task CloseAsync();
}
