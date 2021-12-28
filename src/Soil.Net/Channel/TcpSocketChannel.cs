using System;
using System.Net;
using System.Net.Sockets;
using System.Runtime.Versioning;
using System.Threading.Tasks;
using Soil.Core.Threading.Atomic;
using Soil.Net.Event;

namespace Soil.Net.Channel;

public class TcpSocketChannel : IChannel, IDisposable
{
    private static readonly AtomicUInt64 _idGenerator = new();

    private readonly ulong _id = _idGenerator.Increment();
    public ulong Id
    {
        get
        {
            return _id;
        }
    }

    private readonly EventSource _eventSource = new();
    public IEventSource EventSource
    {
        get
        {
            return _eventSource;
        }
    }

    private readonly Socket _socket;
    public Socket Socket
    {
        get
        {
            return _socket;
        }
    }

    public AddressFamily AddressFamily
    {
        get
        {
            return _socket.AddressFamily;
        }
    }

    public SocketType SocketType
    {
        get
        {
            return _socket.SocketType;
        }
    }

    public ProtocolType ProtocolType
    {
        get
        {
            return _socket.ProtocolType;
        }
    }

    public EndPoint? LocalEndPoint
    {
        get
        {
            return _socket.LocalEndPoint;
        }
    }

    public EndPoint? RemoteEndPoint
    {
        get
        {
            return _socket.RemoteEndPoint;
        }
    }

    public bool IsBound
    {
        get
        {
            return _socket.IsBound;
        }
    }

    public bool Connected
    {
        get
        {
            return _socket.Connected;
        }
    }

    public TcpSocketChannel(SocketType socketType, ProtocolType protocolType)
        : this(new Socket(socketType, protocolType))
    {
    }

    public TcpSocketChannel(
        AddressFamily addressFamily,
        SocketType socketType,
        ProtocolType protocolType)
        : this(new Socket(addressFamily, socketType, protocolType))
    {
    }

    [SupportedOSPlatform("windows")]
    public TcpSocketChannel(SocketInformation socketInformation)
        : this(new Socket(socketInformation))
    {
    }

    public TcpSocketChannel(Socket socket)
    {
        _socket = socket;
    }

    public void Close()
    {
        _socket.Close();
    }

    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    protected virtual void Dispose(bool disposing)
    {
        if (!disposing)
        {
            return;
        }

        _socket.Dispose();
    }

    public void Bind(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public Task BindAsync(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public void Listen()
    {
        throw new NotImplementedException();
    }

    public void Listen(int backlog)
    {
        throw new NotImplementedException();
    }

    public Task ListenAsync()
    {
        throw new NotImplementedException();
    }

    public Task ListenAsync(int backlog)
    {
        throw new NotImplementedException();
    }

    public void Connect(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public void Connect(IPAddress address, int port)
    {
        throw new NotImplementedException();
    }

    public void Connect(IPAddress[] address, int port)
    {
        throw new NotImplementedException();
    }

    public void Connect(string host, int port)
    {
        throw new NotImplementedException();
    }

    public Task ConnectAsync(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public Task ConnectAsync(IPAddress address, int port)
    {
        throw new NotImplementedException();
    }

    public Task ConnectAsync(IPAddress[] address, int port)
    {
        throw new NotImplementedException();
    }

    public Task ConnectAsync(string host, int port)
    {
        throw new NotImplementedException();
    }

    public void Write(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public void Write(byte[] buffer, int size)
    {
        throw new NotImplementedException();
    }

    public void Write(byte[] buffer, int size, int offset)
    {
        throw new NotImplementedException();
    }

    public void Write(ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(byte[] buffer, int size)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(byte[] buffer, int size, int offset)
    {
        throw new NotImplementedException();
    }

    public Task WriteAsync(ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public void Close(int timeout)
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync()
    {
        throw new NotImplementedException();
    }

    public Task CloseAsync(int timeout)
    {
        throw new NotImplementedException();
    }

    public void Flush()
    {
        throw new NotImplementedException();
    }

    public void WriteAndFlush(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public void WriteAndFlush(byte[] buffer, int size)
    {
        throw new NotImplementedException();
    }

    public void WriteAndFlush(byte[] buffer, int size, int offset)
    {
        throw new NotImplementedException();
    }

    public void WriteAndFlush(ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public Task WriteAndFlushAsync(byte[] buffer)
    {
        throw new NotImplementedException();
    }

    public Task WriteAndFlushAsync(byte[] buffer, int size)
    {
        throw new NotImplementedException();
    }

    public Task WriteAndFlushAsync(byte[] buffer, int size, int offset)
    {
        throw new NotImplementedException();
    }

    public Task WriteAndFlushAsync(ReadOnlySpan<byte> buffer)
    {
        throw new NotImplementedException();
    }

    public Task Unregister()
    {
        throw new NotImplementedException();
    }
}
