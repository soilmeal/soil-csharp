using System;
using System.Net.Sockets;
using Soil.Core.Threading.Atomic;

namespace Soil.Net.Channel;

public class TcpChannel : IChannel, IDisposable
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

    private readonly TcpClient _client;

    public TcpChannel(TcpClient client)
    {
        _client = client;
    }

    public void Close()
    {
        _client.Close();
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

        _client.Dispose();
    }
}
