using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;
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

    Socket Socket { get; }

    IEventSource EventSource { get; }

    void Bind(EndPoint endPoint);

    Task BindAsync(EndPoint endPoint);

    void Listen();

    void Listen(int backlog);

    Task ListenAsync();

    Task ListenAsync(int backlog);

    void Connect(EndPoint endPoint);

    void Connect(IPAddress address, int port);

    void Connect(IPAddress[] address, int port);

    void Connect(string host, int port);

    Task ConnectAsync(EndPoint endPoint);

    Task ConnectAsync(IPAddress address, int port);

    Task ConnectAsync(IPAddress[] address, int port);

    Task ConnectAsync(string host, int port);

    void Write(byte[] buffer);

    void Write(byte[] buffer, int size);

    void Write(byte[] buffer, int size, int offset);

    void Write(ReadOnlySpan<byte> buffer);

    Task WriteAsync(byte[] buffer);

    Task WriteAsync(byte[] buffer, int size);

    Task WriteAsync(byte[] buffer, int size, int offset);

    Task WriteAsync(ReadOnlySpan<byte> buffer);

    void Close();

    void Close(int timeout);

    Task CloseAsync();

    Task CloseAsync(int timeout);

    void Flush();

    void WriteAndFlush(byte[] buffer);

    void WriteAndFlush(byte[] buffer, int size);

    void WriteAndFlush(byte[] buffer, int size, int offset);

    void WriteAndFlush(ReadOnlySpan<byte> buffer);

    Task WriteAndFlushAsync(byte[] buffer);

    Task WriteAndFlushAsync(byte[] buffer, int size);

    Task WriteAndFlushAsync(byte[] buffer, int size, int offset);

    Task WriteAndFlushAsync(ReadOnlySpan<byte> buffer);

    Task Unregister();
}
