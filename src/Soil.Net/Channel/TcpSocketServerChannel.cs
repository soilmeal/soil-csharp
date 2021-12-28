using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace Soil.Net.Channel;

public class TcpSocketServerChannel : IServerChannel
{
    public ulong Id
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public AddressFamily AddressFamily
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public SocketType SocketType
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public ProtocolType ProtocolType
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public EndPoint LocalEndPoint
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public EndPoint RemoteEndPoint
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool IsBound
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public bool Connected
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public Socket Socket
    {
        get
        {
            throw new NotImplementedException();
        }
    }

    public void Accept()
    {
        throw new NotImplementedException();
    }

    public Task AcceptAsync()
    {
        throw new NotImplementedException();
    }

    public void Bind(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public Task BindAsync(EndPoint endPoint)
    {
        throw new NotImplementedException();
    }

    public void Close()
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

    public void Flush()
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
}
